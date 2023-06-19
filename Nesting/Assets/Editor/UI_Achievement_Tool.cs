using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;


public class UI_Achievement_Tool : EditorWindow {
	
	Achievements a;
	Object jsonAchievements;
	
		string mname = "";
		bool mreplace = true;
		int mcompare = 1;	
	
	[MenuItem("Tools/Achievement Tool")]
    public static void Init() {
		UI_Achievement_Tool window = (UI_Achievement_Tool)EditorWindow.GetWindow(typeof(UI_Achievement_Tool));
        window.Show();
    }


	void OnGUI() {
		
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
		mname = EditorGUILayout.TextField("Name", mname,GUILayout.Width(100), GUILayout.ExpandWidth(true));
		EditorGUIUtility.labelWidth = 110;
		mreplace = EditorGUILayout.Toggle(" | Replace Value", mreplace,GUILayout.Width(100), GUILayout.ExpandWidth(true));
		EditorGUIUtility.labelWidth = 90;
		mcompare = EditorGUILayout.IntSlider(" | Compare", mcompare , -1, 1, GUILayout.Width(100), GUILayout.ExpandWidth(true));		
		EditorGUIUtility.labelWidth = 90;
		if (GUILayout.Button("Create!", GUILayout.Width(80))) {
			//Fetch();
		}
		EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~~~~~~~~~~~~~");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Populate", GUILayout.Width(80))) {
			Fetch();
		}

	}
	
	
	public void Fetch() {
		a = FindObjectOfType<Achievements>() as Achievements;
 //       jsonAchievements = new SerializedObject(a);
 //       metricList = jsonAchievements.FindProperty("PlaceSetting"); // Find the List in our script and create a refrence of it
	}
/*	
	private void LoadAchievementFile() {
		
		string filePath = "/Assets/Resource/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			//CreateTheFile
		}
		var jsonTextFile = Resources.Load<TextAsset>("Text/jsonAchievements");
	//	jsonAchievements = JsonUtility.FromJson<jsonAchievementStructure>(jsonTextFile);

	}
	
	private void SaveAchievementFile() {

		string filePath = "/Assets/Resource/jsonAchievements.json";
		if (!File.Exists(filePath)) {
			//CreateTheFile
		}
	//	string json = JsonUtility.ToJson(jsonAchievements);

	}
	*/
	
}
