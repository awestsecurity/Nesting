using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(PlaceObjects))]

public class PlaceObjectsEditor : Editor {
 
    PlaceObjects t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;
 
    void OnEnable(){
        t = (PlaceObjects)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("PlaceSetting"); // Find the List in our script and create a refrence of it
    }
	
	public override void OnInspectorGUI(){
        //Update our list
   
        GetTarget.Update();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("~~ Things for the level ~~");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
			
		float originalLabelWidth = EditorGUIUtility.labelWidth;
	
		//Editables
		for(int i = 0; i < ThisList.arraySize; i++){
            SerializedProperty id = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty prefab = id.FindPropertyRelative("prefab");
            SerializedProperty amount = id.FindPropertyRelative("amount");
            SerializedProperty ground = id.FindPropertyRelative("ground");
			
			EditorGUIUtility.labelWidth = 100;
			prefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", prefab.objectReferenceValue, typeof(GameObject), true);
            EditorGUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 60;
			amount.intValue = EditorGUILayout.IntField("Amount",amount.intValue,GUILayout.Width(100), GUILayout.ExpandWidth(true));
			EditorGUIUtility.labelWidth = 90;
			ground.boolValue = EditorGUILayout.Toggle(" | Auto-Ground",ground.boolValue,GUILayout.Width(100), GUILayout.ExpandWidth(true));
			
			//Remove this index from the List
			if(GUILayout.Button("X", GUILayout.Width(20))){
				ThisList.DeleteArrayElementAtIndex(i);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = originalLabelWidth;
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("~~~~~~~~~~~~~~~~~~~~~");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		//Add new item
        if(GUILayout.Button("Add New")){
            t.PlaceSetting.Add(new PlaceObjects.LevelObject());
        }

		//Apply the changes to our list
        GetTarget.ApplyModifiedProperties();

	}
	
}