using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSwitch : MonoBehaviour
{
	
	public string defaultName = "Pause";
	public bool keepEnabled = false;

	public void EnableAndSwitchScreen(string name) {
		gameObject.SetActive(true);
		foreach(Transform child in this.transform) {
			GameObject obj = child.gameObject;
			bool on = ( name == obj.name ) ? true : false ;
			obj.SetActive(on);
		}
	}
	
	public void SwitchScreen(string name) {
		GameObject screenToSwitchTo = null;
		foreach(Transform child in this.transform) {
			GameObject obj = child.gameObject;
			if ( name == obj.name ) {
				screenToSwitchTo = obj;
			}
			else {
				obj.SetActive(false);
			}
		}
		if (screenToSwitchTo != null) { 
			screenToSwitchTo.SetActive(true); 
		}
		keepEnabled = true;
	}
	
	public void Close() {
		//IMPORTANT this has to deactivate before children for event system to pick a new button.
		if (!keepEnabled) {
			gameObject.SetActive(false);
		}
		GameObject screenToSwitchTo = null;
		foreach(Transform child in this.transform) {
			GameObject obj = child.gameObject;
			if ( defaultName == obj.name ) {
				screenToSwitchTo = obj;
			}
			else {
				obj.SetActive(false);
			}
		}
		if (screenToSwitchTo != null) { 
			screenToSwitchTo.SetActive(true); 
		}
	}
	
	void OnDisable() {
		keepEnabled = false;
	}
}
