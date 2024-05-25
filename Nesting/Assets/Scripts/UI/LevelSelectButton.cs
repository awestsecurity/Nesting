using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectButton : MonoBehaviour
{
	///BUG: Event trigger crashes WebGL so....
	public Text levelNameDisplay;
	public string levelName;
	public int levelID = 0;
	//public bool firstSelected = false;
	public Sprite icon;
	public Color defaultColor;
	public Color selectedColor;
		
    // Start is called before the first frame update
    void Start()
    {
        levelNameDisplay = gameObject.GetComponentsInChildren<Text>()[0];
		if (!levelNameDisplay) {
			Debug.LogError("Failed to fetch level button text");
		}
		if (Settings.levelSelected == levelID) {
			IsSelected();
		} else {
			ExitButton(); //defualt display at start
		}
	}

	//Alter text to look selected
   public void IsSelected() {
		levelNameDisplay.color = selectedColor;
		levelNameDisplay.text = "> "+levelName;
	}
	
	//Return text to default appearance.
	public void ExitButton() {
		levelNameDisplay.color = defaultColor;
		levelNameDisplay.text = levelName;	
	}
	
	//Apply selection to the master setting
	public void SetLevelSelection() {
		if (levelID < 1) {
			Debug.LogError($"Level ID is invalid for {levelName}");
		}
		Settings.levelSelected = levelID;
		PlayerPrefs.SetInt("levelSelected", levelID);
	}
	
}
