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
			Debug.LogError("Couldn't find announcement object in globals.");
			popup = GameObject.Find("Announcement").GetComponent<Popup>();
			if (popup == null) {
				return;
			}
		} 
		foreach ( string m in messages ) {
			popup.AddMessage(m);
		}
		popup.LaunchMessagePanel();
    }

}
