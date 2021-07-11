using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityButtons : MonoBehaviour
{

	private Button b;
	private Text t;
	private string[] shadowOptions = {" Best ", " Minimal ", " None "};
	private int i = 0;
	
    void Start()
    {
        b = this.gameObject.GetComponent<Button>();
        t = this.gameObject.transform.GetChild(0).GetComponent<Text>();
		b.onClick.AddListener(delegate {ChangeShadowSetting(); });
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
}
