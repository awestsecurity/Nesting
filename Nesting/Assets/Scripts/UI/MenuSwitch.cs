using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSwitch : MonoBehaviour
{

	public string defaultName = "Pause";
	public GameObject defaultObj;
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
		//Seriusly the EventSystem is needs a lot of hand-holding for multiple menus or singleton UIs
		if (!keepEnabled) {
			gameObject.SetActive(false);
		}
		GameObject screenToSwitchTo = null;
		foreach(Transform child in this.transform) {
			GameObject obj = child.gameObject;
			//Debug.Log($"{obj.name} and {defaultName}");
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

	void OnEnable() {
		if (!defaultObj.activeSelf){
			keepEnabled = true;
		}
	}

	void OnDisable() {
		keepEnabled = false;
	}
}
