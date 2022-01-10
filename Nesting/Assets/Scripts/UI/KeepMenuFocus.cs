using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Place this on a menu that get's activated/deactivated. I.E. a pause menu or start menu.
//It's not the most efficient solution so shouldn't be running during gameplay.
//It prevents Unity's EventSystem from retruning null on a stray mouse click.

public class KeepMenuFocus : MonoBehaviour {

	public GameObject defaultButton;
	private GameObject lastSelected;
	private bool defaultSet = false;
	private EventSystem eventSystem;
	
	private AudioSource speaker;
	public AudioClip selectNoise;
	
	void Start() {
		eventSystem = EventSystem.current;
		speaker = gameObject.GetComponent<AudioSource>();
		if (defaultButton != null) {
			lastSelected = defaultButton;
			defaultSet = true;
		} else {
			SearchForFirstButton(transform);
		}
	}
	
	void OnEnable() {
		eventSystem = EventSystem.current;
		if (!defaultSet) {
			SearchForFirstButton(transform);
		} 
		if (defaultSet) {
			eventSystem.SetSelectedGameObject(defaultButton);
		}
	}
	
	void Update() {
		eventSystem = EventSystem.current;
		if (lastSelected == null) {
			lastSelected = defaultButton;
		}
		
		//Debug.Log(eventSystem.currentSelectedGameObject);
		
		//Use the slower activeInHierarchy to avoid false positives from activeSelf. i.e. button is active, but parent menu is innactive
		if(eventSystem.currentSelectedGameObject == null || !lastSelected.activeSelf) {
			if (lastSelected.gameObject.activeSelf && lastSelected.GetComponent<Button>() != null && lastSelected.GetComponent<Button>().interactable) {
				eventSystem.SetSelectedGameObject(lastSelected);
			} else {
				defaultSet = false;
				//Debug.Log("Get New Button.");
				SearchForFirstButton(transform);
			}
		} else {
			if (lastSelected != eventSystem.currentSelectedGameObject) {
				lastSelected = eventSystem.currentSelectedGameObject;
				if (Settings.sfxOn) { speaker.PlayOneShot(selectNoise, 0.5f); }
			}
			//lastSelected = eventSystem.currentSelectedGameObject;
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
			lastSelected = found;
			eventSystem.SetSelectedGameObject(found);
			defaultSet = true;
		}
	}

}