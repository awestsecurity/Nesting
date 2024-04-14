using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using SimpleJSON;
using System.Linq;



//Goals
//Make an editor script (AFTER UPGRADING UNITY to 2021 at least) for easy achievement creation
public class Achievements : GenericSingleton<Achievements> {
	
	public int checkFrequency = 6;
	//public string saveFile;
	public GameObject uiTemplate;
	public AudioClip congratsSound;
	private AudioSource speaker;
	
	public Dictionary<string, Metric> metrics = new Dictionary<string, Metric>();
	private List<Achievement> completedAchievements = new List<Achievement>();
	private List<Achievement> incompleteAchievements = new List<Achievement>();

    void Start() {
		
		LoadMetricsAndAchievements();
		speaker = transform.GetComponent<AudioSource>();
		//Loops
		InvokeRepeating("CheckAchievements", 10, checkFrequency);
		InvokeRepeating("SaveMetricsAndAchivements", 21, checkFrequency);
    }

	void CheckAchievements() {
		List<Achievement> incompletes = new List<Achievement>(incompleteAchievements);
		foreach (Achievement a in incompletes) {
			if (a.CheckDone()) {
				completedAchievements.Add(a);
				PlayerPrefs.SetInt(a.mName, 1);
				incompleteAchievements.Remove(a);
				//Debug.Log("Achievement Complete! "+a.mName);
				speaker.PlayOneShot(congratsSound);
				DisplayAchievement(a);
			}
		}
		if (incompletes.Count == 0) {
			UnlockBird("l"); // The Extinct Bird
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
	
	public List<Achievement> GetIncompleteAchievments(){
		return incompleteAchievements;
	}
	
	public void UnlockBird(string b = "a"){
		string birdsOwned = PlayerPrefs.GetString("666", "a");
		if (!birdsOwned.Contains(b)) {
			birdsOwned = birdsOwned + b;
			PlayerPrefs.SetString("666", birdsOwned);
			//Debug.Log("Unlocked a new bird!");
		}
	}
	
	private void SaveMetricsAndAchivements() {
		foreach (var entry in metrics) {
			Metric m = entry.Value;
			PlayerPrefs.SetInt("m_"+m.mName,m.Get());
		}
	}
	
	private void LoadMetricsAndAchievements() {
		//Load Objects
		string filePath = Application.dataPath+"/Resources/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			Debug.LogWarning("Missing Achievements JSON File");
		}
		TextAsset jsonTextFile = Resources.Load<TextAsset>("jsonAchievements");
		JSONObject json = (JSONObject)JSON.Parse(jsonTextFile.text);
		foreach(var item in json["metrics"]) {
			Metric tempMetric = JsonUtility.FromJson<Metric>(item.Value);
			metrics.Add(tempMetric.mName, tempMetric);
		}
		foreach(var item in json["achievements"]) {
			Achievement a = JsonUtility.FromJson<Achievement>(item.Value);
			incompleteAchievements.Add(a);
		}
		
		//Load Status
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
		
		//Set Metrics In Achievement
		foreach(Achievement a in incompleteAchievements) {
			Metric[] ms = new Metric[a.inNeeds.Length];
			for(int i = 0; i < ms.Length; i++) {
				ms[i] = metrics[a.inNeeds[i].metricName];
			}
			a.SetNeeds(ms);
		}
		
	}
}

[Serializable]
public class Achievement {
	
	public string mName;
	public string mDescription;
	public bool mComplete {get; private set;}
	public string mUnlock;
	public string mIcon;// {get; private set;}
	public NameGoalPair[] inNeeds;
	public MetricGoalPair[] mNeeds {get; private set;}
	
	public Achievement(string name, NameGoalPair[] needs, string description, String icon) {
		mName = name;
		mIcon = icon;
		mComplete = false;
		inNeeds = needs;
		mDescription = description;
	}
	
	public Achievement(string name, NameGoalPair[] needs, string description, Sprite icon) {
		mName = name;
		mIcon = icon.name;
		mComplete = false;
		inNeeds = needs;
		mDescription = description;
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
				//Debug.Log("Achievement Passed!");
				if (mUnlock.Length == 1) {
					UIObjects.achievements.UnlockBird(mUnlock);
				}
			}
		}
		return mComplete;
	}
	
	public void SetNeeds(Metric[] ms) {
		mNeeds = new MetricGoalPair[inNeeds.Length];
		for(int i = 0; i < inNeeds.Length; i++) {
			int numGoal = inNeeds[i].goal;
			try {
				mNeeds[i] = new MetricGoalPair(ms[i],numGoal);
			} catch (Exception e) {
				Debug.Log(e);
			}
		}
	}
	
	public void SetBirdUnlock(string s) {
		s = s.Substring(0,1);
		mUnlock = s.ToLower();
	}
	
	public MetricGoalPair[] GetNeeds(){
		return mNeeds;
	}
	
}

[Serializable]
public class Metric {
	
	public string mName;
	public bool mReplace;
	public int mCompare;// -1 means less than, 0 means exact, 1 means greater than
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
	public Metric metric;// {get; private set;}
	public int goal;// {get; private set;}
	
	public MetricGoalPair(Metric m, int g) {
		metric = m;
		goal = g;
	}
}

[Serializable]
public struct NameGoalPair {

	public string metricName;// {get; private set;}
	public int goal;// {get; private set;}
	
	public NameGoalPair(string m, int g) {
		metricName = m;
		goal = g;
	}	
	
	public override string ToString(){
		return metricName + ":" + goal.ToString();
	}
}