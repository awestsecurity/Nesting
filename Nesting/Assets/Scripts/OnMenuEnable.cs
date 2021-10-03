using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnMenuEnable : MonoBehaviour
{
	public GameObject startSelected;
	
	private bool defaultSet = false;

    void OnEnable() {
		if (startSelected == null ) {
			SearchForFirstButton(transform);
		}
		EventSystem.current.SetSelectedGameObject(startSelected);
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
		EventSystem.current.SetSelectedGameObject(this.gameObject);
	}
}
