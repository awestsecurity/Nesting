using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{

	private GameObject lastSelected;
	private EventSystem eventSystem;
	public Text selectedLevelText;
	public Image icon;
	public Sprite[] levelIcons;
	public LevelData[] levels;

	void Start() {
		levels = new LevelData[3];
		levels[0] = new LevelData("Spring Meadow", 1, 0);
		levels[1] = new LevelData("Winter Wonder", 3, 1);
		levels[2] = new LevelData("Sleepy Marsh", 4, 2);
		UpdateDisplay();
	}

	void OnEnable() {
		eventSystem = EventSystem.current;
		lastSelected = eventSystem.currentSelectedGameObject;
		UpdateDisplay();
	}

    // Update is called once per frame
	// Occasionally check if WebGL will support event triggers to avoid this whole mess.
	// As of Jan 2022, an event trigger will crash the game
    void Update()   {
		if ( eventSystem.currentSelectedGameObject != lastSelected) {
			GameObject newSelected = eventSystem.currentSelectedGameObject;
			if (lastSelected.name == "LevelButton") {
				LevelSelectButton button = lastSelected.GetComponent<LevelSelectButton>();
				if (button) {
					button.ExitButton();
				}
			}
			if (newSelected.name == "LevelButton") {	
				newSelected.GetComponent<LevelSelectButton>().IsSelected();
			}
			lastSelected = newSelected;
		}
    }
	
	public void UpdateDisplay() {
		foreach(LevelData l in levels) {
			if (l.levelSceneID == Settings.levelSelected) {
				selectedLevelText.text = l.levelName;
				icon.sprite = levelIcons[l.levelIconID];
				break;
			}
		}
	}
}

public struct LevelData 
{
	public int levelSceneID;
	public string levelName;
	public int levelIconID;
	
	public LevelData(string name, int id, int icon) {
		this.levelName = name;
		this.levelSceneID = id;
		this.levelIconID = icon;
	}
	
}

