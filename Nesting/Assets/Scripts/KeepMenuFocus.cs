using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Place this on a menu that get's activated/deactivated. I.E. a pause menu or start menu.
//It's not the most efficient solution so shouldn't be running during gameplay.
//It prevents Unity's EventSystem from retruning null on a stray mouse click.

public class KeepMenuFocus : MonoBehaviour {

	//
	public GameObject defaultButton;
	private GameObject lastSelected;
	private bool defaultSet = false;
	
	void Start() {
		if (defaultButton != null) {
			lastSelected = defaultButton;
			defaultSet = true;
		} else {
			SearchForFirstButton(transform);
		}
	}
	
	void OnEnable() {
		if (!defaultSet) {
			SearchForFirstButton(transform);
		}
		EventSystem.current.SetSelectedGameObject(defaultButton);
	}
	
	void Update() {
		
		if (lastSelected == null) {
			lastSelected = defaultButton;
		}
		
		if(EventSystem.current.currentSelectedGameObject == null) {
			if (lastSelected.gameObject.activeSelf && lastSelected.GetComponent<Button>() != null && lastSelected.GetComponent<Button>().interactable) {
				EventSystem.current.SetSelectedGameObject(lastSelected);
			}
		}
		else {
			lastSelected = EventSystem.current.currentSelectedGameObject;
		}
	}
	
	//If menu is built dynamically and you cannot pre-set default button. This will search for a button child and set the first one to default.
	//Recursive search will get ALL children. 
	private void SearchForFirstButton(Transform t) {
		GameObject found = null;
		foreach(Transform child in t) {
			if (defaultSet) break;
			if (child.GetComponent<Button>() != null) {
				found = child.gameObject;
				break;
			}
			SearchForFirstButton(child);
		}
		
		if (found != null) { 
			defaultButton = found;
			defaultSet = true;
		}
	}

}