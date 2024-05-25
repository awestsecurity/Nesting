using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnMenuEnable : MonoBehaviour
{
	public GameObject startSelected;
	public bool overrideStart = false;
	
	private bool defaultSet = false;
	private EventSystem eventSystem;

    void OnEnable() {
		eventSystem = EventSystem.current;
		if (startSelected == null ) {
			SearchForFirstButton(transform);
		}
		if (!overrideStart) {
			eventSystem.SetSelectedGameObject(startSelected);
		}
	}
	
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
			startSelected = found;
			defaultSet = true;
		}
	}
	
	void OnDisable(){
		if (EventSystem.current != null) {
			EventSystem.current.SetSelectedGameObject(this.gameObject);
		}
	}
}
