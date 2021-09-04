using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMessages : MonoBehaviour
{
	private Popup popup;
	public string[] messages;

    void Start()
    {
        popup = UIObjects.popup;
		if (popup == null) {
			Debug.LogWarning("Couldn't find announcement object in globals.");
			GameObject popupO = GameObject.Find("Announcement");
			if (popupO == null) {
				Debug.LogWarning("Couldn't find announcement object in scene.");
				return;
			} else {
				popup = popupO.GetComponent<Popup>();
			}
		} 
		foreach ( string m in messages ) {
			popup.AddMessage(m);
		}
		popup.LaunchMessagePanel();
    }

}
