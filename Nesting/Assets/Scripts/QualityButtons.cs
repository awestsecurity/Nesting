using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityButtons : MonoBehaviour
{

	public int settingID = 0;
	private Button b;
	private Text t;
	private string[] shadowOptions = {" Best ", " Minimal ", " None "};
	private string[] viewDistance = {" Far ", "Middle", "Close"};
	private int i = 0;
	private Camera cam;
	
    void Start()
    {
        b = this.gameObject.GetComponent<Button>();
        t = this.gameObject.transform.GetChild(0).GetComponent<Text>();
		switch(settingID) {
			case 0:
				b.onClick.AddListener(delegate {ChangeShadowSetting(); });
				break;
			case 1:
				b.onClick.AddListener(delegate {ChangeViewDistance(); });
				break;
			default:
				break;
		}
    }

	void ChangeShadowSetting(){
		i = (i < shadowOptions.Length - 1) ? (i+1) : 0 ;
		t.text = $"Shadows: {shadowOptions[i]}";
		switch (i) {
			case 0:
				QualitySettings.shadows = ShadowQuality.All;
				break;
			case 1:
				QualitySettings.shadows = ShadowQuality.HardOnly;
				break;
			case 2:
				QualitySettings.shadows = ShadowQuality.Disable;
				break;
			default:
				break;
		}
	}
	
	void ChangeViewDistance(){
		t.text = $"Shadows: {shadowOptions[i]}";
		i = (i < viewDistance.Length - 1) ? (i+1) : 0 ;
		switch (i) {
			case 0:
				// change fog
				// change camera
				break;
			case 1:
				break;
			case 2:
				break;
			default:
				break;
		}		
	}
}
