using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Goals
//Make an editor script (AFTER UPGRADING UNITY to 2021 at least) for easy achievement creation
public class Achievements : MonoBehaviour {
	
	public int checkFrequency = 6;
	//public string saveFile;
	public GameObject uiTemplate;
	
	public Dictionary<string, Metric> metrics = new Dictionary<string, Metric>();
	private List<Achievement> completedAchievements = new List<Achievement>();
	private List<Achievement> incompleteAchievements = new List<Achievement>();
	

    void Start() {
		
		//Perpetual Metrics
		metrics.Add("TimesPlayedSpring", new Metric("TimesPlayedSpring", 1, false));
		metrics.Add("TimesPlayedWinter", new Metric("TimesPlayedWinter", 1, false));
		metrics.Add("TimesPlayedMarsh", new Metric("TimesPlayedMarsh", 1, false));
		metrics.Add("TotalMass", new Metric("TotalMass", 1, false));
		metrics.Add("EggFound", new Metric("EggFound", 0));
		metrics.Add("MushroomsCollected", new Metric("MushroomsCollected", 1, false));
		//
		//Challenge metrics
		metrics.Add("FlowersIn1Run", new Metric("FlowersIn1Run", 1, true));
		//
		//Temporary metrics
		metrics.Add("Level", new Metric("Level", 0, replace: true));
		metrics.Add("BirdRarity", new Metric("BirdRarity", 0, replace: true));
		//
		
		//ACHIEVEMENTS//
		MetricGoalPair[] pair = new MetricGoalPair[] {new MetricGoalPair(metrics["EggFound"], 1), new MetricGoalPair(metrics["Level"], 1)};
		incompleteAchievements.Add(new Achievement("Treasure Hunter 1", pair));
		
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["TimesPlayedWinter"], 5)};
		incompleteAchievements.Add(new Achievement("Polar Swim", pair, "Play winter wonderland 5 times"));
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["TimesPlayedSpring"], 8)};
		incompleteAchievements.Add(new Achievement("Promenade", pair, "Play Spring Meadow 8 times"));
		
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["FlowersIn1Run"], 50), new MetricGoalPair(metrics["Level"], 1)};
		incompleteAchievements.Add(new Achievement("Gardener 1", pair, "Collect 50 flowers in a single Meadow run"));
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["BirdRarity"], 4), new MetricGoalPair(metrics["Level"], 3)};
		incompleteAchievements.Add(new Achievement("Risky Operation", pair, "Take an endangered bird out in the winter"));

		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["MushroomsCollected"], 100)};
		incompleteAchievements.Add(new Achievement("Forager I", pair, "Collect 100 Mushrooms"));
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["MushroomsCollected"], 500)};
		incompleteAchievements.Add(new Achievement("Forager II", pair, "Collect 500 Mushrooms"));
		pair = new MetricGoalPair[] {new MetricGoalPair(metrics["MushroomsCollected"], 2000)};
		incompleteAchievements.Add(new Achievement("Forager III", pair, "Collect 500 Mushrooms"));

		LoadMetricsAndAchievements();
		//Loops
		InvokeRepeating("CheckAchievements", 10, checkFrequency);
		InvokeRepeating("SaveMetricsAndAchivements", 21, checkFrequency);
    }

	void CheckAchievements() {
		List<Achievement> incompletes = new List<Achievement>(incompleteAchievements);
		foreach (Achievement a in incompletes) {
			if (a.CheckDone()) {
				completedAchievements.Add(a);
				incompleteAchievements.Remove(a);
				Debug.Log("Achievement Complete! "+a.mName);
				DisplayAchievement(a);
			}
		}
	}
	
	void DisplayAchievement(Achievement achieved) {
		Transform badgeTransform = Instantiate(uiTemplate).transform;
		AchievementPopup badge = badgeTransform.gameObject.GetComponent<AchievementPopup>();
		badgeTransform.SetParent(UIObjects.achievements.transform);
		badgeTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60f); //Off screen center bottom
		badge.SetAchievement(achieved);
		badge.Display();
	}
	
	public void UpdateMetric(string name, int v = 1){
		if (metrics.ContainsKey(name)) {
			metrics[name].Set(v);
			//Debug.Log(name+" updated to "+metrics[name].Get());
		} else {
			Debug.LogWarning("No metric with name: "+name+". Check your metric/achievement list.");
		}
	}
	
	public List<Achievement> GetCompletedAchievments(){
		return completedAchievements;
	}
	
	private void SaveMetricsAndAchivements() {
		foreach (var entry in metrics) {
			Metric m = entry.Value;
			PlayerPrefs.SetInt("m_"+m.mName,m.Get());
		}
		foreach (Achievement a in completedAchievements) {
			PlayerPrefs.SetInt(a.mName, 1);
		}
	}
	
	private void LoadMetricsAndAchievements() {
		foreach (var entry in metrics) {
			Metric m = entry.Value;
			m.Set(PlayerPrefs.GetInt("m_"+m.mName, 0));
		}
		List<Achievement> incompletes = new List<Achievement>(incompleteAchievements);
		foreach (Achievement a in incompletes) {
			int complete = PlayerPrefs.GetInt(a.mName, 0);
			if (complete == 1) {
				completedAchievements.Add(a);
				incompleteAchievements.Remove(a);
			}
		}
	}
}

[Serializable]
public class Achievement {
	
	public string mName {get; private set;}
	public string mDescription {get; private set;}
	public bool mComplete {get; private set;}
	
	public string mIcon {get; private set;}
	public MetricGoalPair[] mNeeds {get; private set;}
	
	public Achievement(string name, MetricGoalPair[] needs, string description = "") {
		mName = name;
		mComplete = false;
		mNeeds = needs;
		mDescription = description;
		if(mNeeds.Length < 1) {	Debug.LogError("Achievement has no requirements.");	}
	}

	public Achievement(string name, MetricGoalPair[] needs, string description, String icon) {
		mName = name;
		mIcon = icon;
		mComplete = false;
		mNeeds = needs;
		mDescription = description;
		if(mNeeds.Length < 1) {	Debug.LogError("Achievement has no requirements.");	}
	}
	
	public Achievement(string name, MetricGoalPair[] needs, string description, Sprite icon) {
		mName = name;
		mIcon = icon.name;
		mComplete = false;
		mNeeds = needs;
		mDescription = description;
		if(mNeeds.Length < 1) {	Debug.LogError("Achievement has no requirements.");	}
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
	
	public MetricGoalPair[] GetNeeds(){
		return mNeeds;
	}
	
}

[Serializable]
public class Metric {
	
	public string mName {get; private set;}
	public bool mReplace {get; private set;} 
	public int mCompare {get; private set;} // -1 means less than, 0 means exact, +1 means greater than
	public int mCurrentValue {get; private set;}
	
	public Metric(string name, int compare = 1, bool replace = true) {
		mName = name;
		mCompare = compare;
		mReplace = replace;
		mCurrentValue = 0;
	}
	
	//Change the current value of the metric if better than current value
	public void Set(int v) {
		int temp = 0;
		if (mCompare < 0) { //value to decrease
			temp = v < mCurrentValue ? v : mCurrentValue;
			mCurrentValue = mReplace ? temp : mCurrentValue - v;
		} else if (mCompare == 0) { //value to replace
			mCurrentValue = v;
		} else { //value to increase
			temp = v > mCurrentValue ? v : mCurrentValue;
			mCurrentValue = mReplace ? temp : mCurrentValue + v;
		}			
	}
	
	public int Get() {
		return mCurrentValue;
	}
	
	//If not complete, check requirements for completion and set complete flag accordingly
	public bool Check(int val) {
		bool mComplete = false;
		if (mCompare < 0) {
			mComplete = mCurrentValue <= val;
		} else if (mCompare == 0) {
			mComplete = mCurrentValue == val;
		} else {
			mComplete = mCurrentValue >= val;
		}	
		return mComplete;
	}
}

[Serializable]
public struct MetricGoalPair {
	public Metric metric {get; private set;}
	public int goal {get; private set;}
	
	public MetricGoalPair(Metric m, int g) {
		metric = m;
		goal = g;
	}
}