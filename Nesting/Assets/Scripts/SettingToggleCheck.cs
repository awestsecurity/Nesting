using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class SettingToggleCheck : MonoBehaviour
{
	
	public int settingID = -1;
	
    void Awake()
    {
		if (Settings.postProcessing) {
			Volume v = this.gameObject.GetComponent<Volume>();
			if (v) {
				v.enabled = Settings.postProcessing;
				//Debug.Log("set render volume to " + Settings.GetSettingByID(settingID).ToString() );
			} else {
				Debug.LogWarning("failed to find volume component - SettingToggleCheck.cs line 20");
			}
		}
    }

}
