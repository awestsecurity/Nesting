using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SimpleJSON;

public class UI_Achievement_Tool : EditorWindow {
	
	Vector2 scrollPos;
	
	string mname = "";
	bool mreplace = false;
	int mcompare = 1;	
	List<Metric> metrics = new List<Metric>();
	
	string aname = "";
	string adescription = "";
	UnityEngine.Object aicon;
	string aUnlock = "a";
	List<int> mets = new List<int>();
	List<int> goals = new List<int>();
	List<Achievement> achievements = new List<Achievement>();

	[MenuItem("Tools/Achievement Tool")]
    public static void Init() {
		UI_Achievement_Tool window = (UI_Achievement_Tool)EditorWindow.GetWindow(typeof(UI_Achievement_Tool));
        window.Show();
    }


	void OnGUI() {
	
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();	
		if (GUILayout.Button("~ Save ~", GUILayout.Width(200))) {
			SaveAchievementFile();
		}
		if (GUILayout.Button("~ Load ~", GUILayout.Width(200))) {
			LoadAchievementFile();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		//CREATE METRICS/ACHIEVEMNETS//
	
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~~~~~~~~~~~~~");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("New Metric");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 60;
		mname = EditorGUILayout.TextField("Name", mname,GUILayout.Width(200), GUILayout.ExpandWidth(true));
		EditorGUIUtility.labelWidth = 110;
		mreplace = EditorGUILayout.Toggle(" |- Replace Value", mreplace,GUILayout.Width(120), GUILayout.ExpandWidth(false));
		EditorGUIUtility.labelWidth = 90;
		mcompare = EditorGUILayout.IntSlider(" -|  Compare", mcompare , -1, 1, GUILayout.Width(200), GUILayout.ExpandWidth(true));		
		EditorGUIUtility.labelWidth = 90;
		if (GUILayout.Button("Create Metric", GUILayout.Width(120))) {
			CreateMetric(mname, mcompare, mreplace);
		}
		EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~~~~~~~~~~~~~");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("New Achievement");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 60;
		aname = EditorGUILayout.TextField("Name", aname,GUILayout.Width(200), GUILayout.ExpandWidth(true));
		EditorGUIUtility.labelWidth = 90;
		adescription = EditorGUILayout.TextField("  |  Description", adescription, GUILayout.Width(400), GUILayout.ExpandWidth(true));
		EditorGUILayout.EndHorizontal();
		if (mets.Count == 0) {
			mets.Add(0);
			goals.Add(1);
		}
		
		string[] allMetrics = new string[metrics.Count];
		for (int j = 0; j < metrics.Count; j++) {
			allMetrics[j] = metrics[j].mName;
		}
		for (int i = 0; i < mets.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			mets[i] = EditorGUILayout.Popup("Metric:", mets[i], allMetrics);
			goals[i] = EditorGUILayout.IntField(" | Goal", goals[i], GUILayout.Width(200), GUILayout.ExpandWidth(false));
			if (GUILayout.Button(" X ", GUILayout.Width(60))) {
				mets.RemoveAt(i);
				goals.RemoveAt(i);
			}			
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Need", GUILayout.Width(150))) {
			mets.Add(0);
			goals.Add(1);
		}
		aUnlock = aUnlock.Length > 1 ? aUnlock.Substring(0,1) : aUnlock;
		aUnlock = EditorGUILayout.TextField("Unlock", aUnlock, GUILayout.Width(120));
		aicon = EditorGUILayout.ObjectField("| Icon", aicon, typeof(Sprite), false);
		if (GUILayout.Button(" Create Achievement ", GUILayout.Width(200), GUILayout.Height(50))) {
			CreateAchievement();
		}
		EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~v~*~^~*~v~~~~~~~~",  GUILayout.Width(300));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		
		/////////////////////////////
		//VIEW METRCIS/ACHIEVEMENTS//
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("Current Metrics");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		for(int i = 0; i < metrics.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(" X ", GUILayout.Width(35))) {
				metrics.RemoveAt(i);
				return;
			}	
			EditorGUILayout.LabelField(metrics[i].mName);
			EditorGUILayout.LabelField("Compare: "+metrics[i].mCompare);
			EditorGUILayout.LabelField("Replace: "+metrics[i].mReplace.ToString());
			EditorGUILayout.EndHorizontal();
		}
		
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~~~~~~~~~~~~~");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("Current Achievements");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		for(int i = 0; i < achievements.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(" X ", GUILayout.Width(35))) {
				achievements.RemoveAt(i);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();
				return;
			}	
			EditorGUILayout.LabelField(achievements[i].mName, GUILayout.Width(130));
			EditorGUILayout.LabelField(achievements[i].mUnlock, GUILayout.Width(20));
			EditorGUILayout.LabelField("- "+achievements[i].mDescription, GUILayout.Width(260));
			foreach (NameGoalPair p in achievements[i].inNeeds) {
				EditorGUILayout.LabelField(p.metricName+" - "+p.goal, GUILayout.Width(150));
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();

	}
	
	public void CreateMetric(string name, int compare, bool replace) {
		if (name == "") {
			Debug.LogWarning("Metric Name Required.");
			return;
		}
		metrics.Add(new Metric(name,compare,replace));
		mname = "";
	}
	
	public void CreateAchievement() {
		if (aname == "") {
			Debug.LogWarning("Achievement Name Required.");
			return;
		}
		NameGoalPair[] pair = new NameGoalPair[mets.Count];
		for(int i = 0; i < mets.Count; i++) {
			pair[i] = new NameGoalPair(metrics[mets[i]].mName, goals[i]);
		}
		string icon = aicon.name;
		Achievement a = new Achievement(aname, pair, adescription, icon);
		if (aUnlock.Length == 1) {
			a.SetBirdUnlock(aUnlock);
		}
		achievements.Add(a);
		aname = "";
	}
	
	private void LoadAchievementFile() {
		
		string filePath = Application.dataPath+"/Resources/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			Debug.LogWarning("Missing Achievements JSON File");
		}
		TextAsset jsonTextFile = Resources.Load<TextAsset>("jsonAchievements");
		JSONObject json = (JSONObject)JSON.Parse(jsonTextFile.text);
		foreach(var item in json["metrics"]) {
			Metric m = JsonUtility.FromJson<Metric>(item.Value);
			string[] names = metrics.Select(o => o.mName).ToArray();
			if (names.Contains(m.mName)) {
				continue;
			} else {
				metrics.Add(m);
			}
		}			
		
		achievements = new List<Achievement>();
		foreach(var item in json["achievements"]) {
			Achievement a = JsonUtility.FromJson<Achievement>(item.Value);
			string[] names = achievements.Select(o => o.mName).ToArray();
			if (names.Contains(a.mName)) {
				Debug.LogWarning(a.mName + " excluded");
				continue;
			} else {
				achievements.Add(a);
			}
		}

	}
	
	private void SaveAchievementFile() {

		string filePath = Application.dataPath+"/Resources/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			Debug.LogWarning("Missing Achievements JSON File");
		}
		JSONObject json = new JSONObject();
		json.Add("metrics");	
		foreach ( Metric m in metrics ) {
			json["metrics"].Add(JsonUtility.ToJson(m));
		}
		json.Add("achievements");
		foreach ( Achievement a in achievements ) {
			json["achievements"].Add(JsonUtility.ToJson(a));
		}
		//Debug.Log(json.ToString());
		File.WriteAllText(filePath, json.ToString());
	}
	
}


