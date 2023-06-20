using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;
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
	List<int> mets = new List<int>();
	List<int> goals = new List<int>();
	List<Achievement> achievements = new List<Achievement>();

	[MenuItem("Tools/Achievement Tool")]
    public static void Init() {
		UI_Achievement_Tool window = (UI_Achievement_Tool)EditorWindow.GetWindow(typeof(UI_Achievement_Tool));
        window.Show();
    }


	void OnGUI() {
		
		//CREATE METRICS/ACHIEVEMNETS//
		if (GUILayout.Button("~ Save ~", GUILayout.Width(200))) {
			SaveAchievementFile();
		}
		
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
		mreplace = EditorGUILayout.Toggle(" | Replace Value", mreplace,GUILayout.Width(120), GUILayout.ExpandWidth(false));
		EditorGUIUtility.labelWidth = 90;
		mcompare = EditorGUILayout.IntSlider(" | Compare", mcompare , -1, 1, GUILayout.Width(200), GUILayout.ExpandWidth(true));		
		EditorGUIUtility.labelWidth = 90;
		if (GUILayout.Button("Create!", GUILayout.Width(80))) {
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
		adescription = EditorGUILayout.TextField("| Description", adescription, GUILayout.Width(400), GUILayout.ExpandWidth(true));
		aicon = EditorGUILayout.ObjectField("| Icon", aicon, typeof(Sprite), false);
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
		if (GUILayout.Button(" Create Achievement ", GUILayout.Width(150))) {
			CreateAchievement();
		}
		EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~~~~~~~~~~~~~");
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
			if (GUILayout.Button(" X ", GUILayout.Width(60))) {
				metrics.RemoveAt(i);
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
			if (GUILayout.Button(" X ", GUILayout.Width(40))) {
				achievements.RemoveAt(i);
			}	
			EditorGUILayout.LabelField(achievements[i].mName);
			EditorGUILayout.LabelField("- "+achievements[i].mDescription, GUILayout.Width(300));
			//EditorGUILayout.EndHorizontal();
			foreach (MetricGoalPair p in achievements[i].mNeeds) {
				//EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(p.metric.mName+" - "+p.goal);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();

	}
	
	public void CreateMetric(string name, int compare, bool replace) {
		metrics.Add(new Metric(name,compare,replace));
		mname = "";
	}
	
	public void CreateAchievement() {
		MetricGoalPair[] pair = new MetricGoalPair[mets.Count];
		for(int i = 0; i < mets.Count; i++) {
			pair[i] = new MetricGoalPair(metrics[mets[i]], goals[i]);
		}
		string icon = aicon.name;
		Achievement a = new Achievement(aname, pair, adescription, icon);
		achievements.Add(a);
		aname = "";
	}
	
	private void LoadAchievementFile() {
		
		string filePath = Application.dataPath+"/Resources/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			Debug.LogWarning("Missing Achievements JSON File");
		}
		var jsonTextFile = Resources.Load<TextAsset>("Text/jsonAchievements");
	//	jsonAchievements = JsonUtility.FromJson<jsonAchievementStructure>(jsonTextFile);

	}
	
	private void SaveAchievementFile() {

		string filePath = Application.dataPath+"/Resources/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			Debug.LogWarning("Missing Achievements JSON File");
		}
		JSONObject json = new JSONObject();
		json.Add("metrics");
		foreach ( Metric m in metrics ) {
			json["metrics"].Add(m.mName);
			json["metrics"][m.mName].Add("compare", m.mCompare); 
			json["metrics"][m.mName].Add("replace", m.mReplace);
		}
		json.Add("achievements");
		foreach ( Achievement a in achievements ) {
			json["achievements"].Add(a.mName);
			json["achievements"][a.mName].Add("desc", a.mDescription);
			json["achievements"][a.mName].Add("icon", a.mIcon);
			for (int i = 0; i < a.mNeeds.Length; i++) {
				json["achievements"][a.mName].Add(a.mNeeds[i].metric.mName, a.mNeeds[i].goal);
			}
		}
		Debug.Log(json.ToString());
		File.WriteAllText(filePath, json.ToString());
	}
	
}


