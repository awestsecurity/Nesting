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
	private string[] qualityOptions = {" Poor ", " Okey Dokey ", " Great "};
	private string[] viewDistance = {" Far ", "Middle", "Close"};
	private string[] booleanOptions = {" ON ", " OFF "};
	private int i = 1;
	private Camera cam;
	
    void Start()
    {
		AudioListener.volume = 0f;
        b = this.gameObject.GetComponent<Button>();
        t = this.gameObject.transform.GetChild(0).GetComponent<Text>();
		switch(settingID) {
			case 0:
				b.onClick.AddListener(delegate {ChangeQuality(); });
				break;
			case 1:
				b.onClick.AddListener(delegate {ChangeViewDistance(); });
				break;
			case 2:
				b.onClick.AddListener(delegate {ChangeSFX(); });
				break;
			default:
				break;
		}
    }
	
	void ChangeQuality() {
		i = (i < qualityOptions.Length - 1) ? (i+1) : 0 ;
		t.text = $"Quality: {qualityOptions[i]}";
		switch (i) {
			case 0:
				QualitySettings.SetQualityLevel(1, true);
				break;
			case 1:
				QualitySettings.SetQualityLevel(3, true);
				break;
			case 2:
				QualitySettings.SetQualityLevel(5, true);
				break;
			default:
				break;
		}		
		
	}
	
	void ChangeSFX() {
		i = (i < booleanOptions.Length - 1) ? (i+1) : 0 ;
		t.text = $"Sound: {booleanOptions[i]}";
		switch (i) {
			case 0:
				AudioListener.volume = 0.8f;
				break;
			case 1:
				AudioListener.volume = 0f;
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
