using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goals
//Make an editor script (AFTER UPGRADING UNITY to 2021 at least) for easy achievement creation

public class Achievements : MonoBehaviour
{
	
	public int checkFrequency = 6;
	public string saveFile;
	
	private Dictionary<string, Metric> metrics = new Dictionary<string, Metric>();
	private List<Achievement> completedAchievements = new List<Achievement>();
	private List<Achievement> incompleteAchievements = new List<Achievement>();
	

    void Start()
    {
		
		metrics.Add("MushroomsCollected", new Metric("MushroomsCollected", 1, false));
		metrics.Add("TimesPlayedMeadow", new Metric("TimesPlayedMeadow", 1, false));
		metrics.Add("TimesPlayedWinter", new Metric("TimesPlayedWinter", 1, false));
		metrics.Add("TimesPlayedMarsh", new Metric("TimesPlayedMarsh", 1, false));
		metrics.Add("TotalWeight", new Metric("TotalWeight", 1, false));
		metrics.Add("EggFound-Meadow", new Metric("EggFound-Meadow", 0));
		metrics["EggFound-Meadow"].SetTarget(1);
		metrics.Add("FlowersIn1Run", new Metric("FlowersIn1Run", 1, true));
		metrics.Add("Level", new Metric("Level", 0, replace: true));
		
		MetricGoalPair[] pair = new MetricGoalPair[] {new MetricGoalPair(metrics["MeadowEggFound"], 1)};
		incompleteAchievements.Add(new Achievement("Treasure Hunter 1", 0, pair));
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["TimesPlayedWinter"], 5)};
		incompleteAchievements.Add(new Achievement("PolarSwim", 0, pair));
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["FlowersIn1Run"], 50), new MetricGoalPair(metrics["Level"], 1)};
		incompleteAchievements.Add(new Achievement("Gardener 1", 0, pair));
		
		//Loops
		InvokeRepeating("CheckAchievments", 10, checkFrequency);
    }

	void CheckAchievments() {
		foreach (Achievement a in incompleteAchievements) {
			if (a.CheckDone()) {
				completedAchievements.Add(a);
				incompleteAchievements.Remove(a);
				Debug.Log("Achievement Complete! "+a.mName);
			}
		}
	}
	
	public void UpdateMetric(string name, int v){
		metrics[name].Set(v);
	}
	
	public List<Achievement> GetCompletedAchievments(){
		return completedAchievements;
	}
	
	private void SaveAchievements() {
		
	}

	private void LoadAchievements() {
		
	}
}

public class Achievement {
	
	public string mName {get; private set;}
	private string mDescription = "null";
	public bool mComplete {get; private set;}
	
	private int mIconID;
	private MetricGoalPair[] mNeeds;
	private int[] mTargets;
	
	public Achievement(string name, int iconID, MetricGoalPair[] needs) {
		mName = name;
		mIconID = iconID;
		mComplete = false;
		mNeeds = needs;
	}
	
	public void SetDescription(string s) {
		mDescription = s;
	}
	
	public bool CheckDone() {
		if (mComplete){
			return true;
		} else {
			bool pass = true;
			foreach (MetricGoalPair m in mNeeds) {
				if (m.metric.Check(m.goal)) {
					continue;
				} else {
					pass = false;
				}
			}
			if (pass) {
				mComplete = true;
			}
		}
		return mComplete;
	}
	
}

public class Metric {
	
	private string mName;
	public bool mComplete {get; private set;}

	private int mTargetValue;
	private bool mReplace; 
	private int mCompare; // -1 means less than, 0 means exact, +1 means greater than
	private int mCurrentValue;
	
	public Metric(string name, int compare = 1, bool replace = true) {
		mName = name;
		mTargetValue = 99999;
		mCompare = compare;
		mComplete = false;
		mReplace = replace;
		mCurrentValue = 0;
	}
	
	public void SetTarget(int v) {
		mTargetValue = v;
	}
	
	public void Set(int v) {
		int temp = 0;
		if (mCompare < 0) {
			temp = v < mCurrentValue ? v : mCurrentValue;
			mCurrentValue = mReplace ? temp : mCurrentValue - temp;
		} else if (mCompare == 0) {
			mCurrentValue = v == mTargetValue ? v : mCurrentValue;
		} else {
			temp = v > mCurrentValue ? v : mCurrentValue;
			mCurrentValue = mReplace ? temp : mCurrentValue + temp;
		}			
	}
	
	//If not complete, check requirements for completion and set complete flag accordingly
	public bool Check(int val = -1) {
		if (mComplete) {
			return true;
		}
		else {
			val = val == -1 ? mTargetValue : val;
			if (mCompare < 0) {
				mComplete = mCurrentValue < val;
			} else if (mCompare == 0) {
				mComplete = mCurrentValue == val;
			} else {
				mComplete = mCurrentValue > val;
			}	
			return mComplete;
		}
	}
}

public struct MetricGoalPair {
	public Metric metric {get; private set;}
	public int goal {get; private set;}
	
	public MetricGoalPair(Metric m, int g) {
		metric = m;
		goal = g;
	}
}