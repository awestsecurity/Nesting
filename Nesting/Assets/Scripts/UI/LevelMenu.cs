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
	LevelSelectButton[] buttons;
	public LevelData[] levels;

	void OnEnable() {
		UpdateDisplay();
		eventSystem = EventSystem.current;
		eventSystem.SetSelectedGameObject(GetStartSelection().gameObject);
		lastSelected = eventSystem.currentSelectedGameObject;
	}
	
	//
	void SetupLevels() {
		buttons = GetComponentsInChildren<LevelSelectButton>(); 
		levels = new LevelData[buttons.Length];
		for(int i = 0; i < buttons.Length; i++) {
			LevelData l = new LevelData(buttons[i].levelName, buttons[i].levelID, buttons[i].icon);
			levels[i] = l;
		}
	}

    // Update is called once per frame
	// Occasionally check if WebGL will support event triggers to avoid this whole mess.
	// As of Jan 2022, an event trigger will crash the game
    void Update()   {
		if (eventSystem.currentSelectedGameObject != lastSelected) {
			//UpdateDisplay();
			GameObject newSelected = eventSystem.currentSelectedGameObject;
			if (lastSelected.tag == "LevelButton") {
				LevelSelectButton button = lastSelected.GetComponent<LevelSelectButton>();
				if (button) {
					button.ExitButton();
				}
			}
			if (newSelected.tag == "LevelButton") {	
				newSelected.GetComponent<LevelSelectButton>().IsSelected();
			}
			lastSelected = newSelected;
		}
    }
	
	public void UpdateDisplay() {
		if (buttons == null) {
			SetupLevels();
		}
		LevelSelectButton bSelected = GetStartSelection();
		selectedLevelText.text = bSelected.levelName;
		icon.sprite = bSelected.icon;
		bSelected.IsSelected();
	}
	
	private LevelSelectButton GetStartSelection() {
		foreach(LevelSelectButton b in buttons) {
			if (b.levelID == Settings.levelSelected) {
				return b;
			}
		}
		return buttons[0];
	}
}

public struct LevelData 
{
	public int levelSceneID;
	public string levelName;
	public Sprite levelIcon;
	
	public LevelData(string name, int id, Sprite icon) {
		this.levelName = name;
		this.levelSceneID = id;
		this.levelIcon = icon;
	}
}

