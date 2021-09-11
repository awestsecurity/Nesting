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
	
	void Start() {
		lastSelected = defaultButton;
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

}