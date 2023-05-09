using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class SettingToggleCheck : MonoBehaviour
{
	
	public int settingID = -1;
	
    void Awake()
    {
		if (settingID == 6) {
			Volume v = this.gameObject.GetComponent<Volume>();
			if (v) {
				v.enabled = Settings.GetSettingByID(settingID);
				Debug.Log("set render volume to " + Settings.GetSettingByID(settingID).ToString() );
			} else {
				Debug.LogWarning("failed to find volume component");
			}
		}
    }

}
