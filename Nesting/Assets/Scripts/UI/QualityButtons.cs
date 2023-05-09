using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class QualityButtons : MonoBehaviour
{

	public int settingID = 0;
	private Button b;
	private Text t;
	private string[] buttonOptions;
	private string buttonName;
	private delegate void buttonDelegate(int i);
	private buttonDelegate changeFunction;
	
	private string[] shadowOptions = {" Best ", " Minimal ", " None "};
	private string[] qualityOptions = {" Poor ", " Just OK ", " Great "};
	private string[] viewDistanceOptions = {" Far ", "Middle", "Close"};
	private string[] booleanOptions = {" OFF ", " ON "};

	private int currentSetting;
	private Camera cam;
	
    void Start()
    {
        b = this.gameObject.GetComponent<Button>();
        t = this.gameObject.transform.GetChild(0).GetComponent<Text>();
		b.onClick.AddListener(delegate {ChangeSetting(); });
		SetOptions();
		LoadPref();
		InitializeSetting();
    }
	
	private void LoadPref() {
		currentSetting = PlayerPrefs.GetInt($"{settingID}", 1);
		//Debug.Log($"{settingID} == {currentSetting}");
	}
	
	private void SetOptions() {
		switch(settingID) {
			case 0: //Overall quality
				buttonOptions = qualityOptions;
				buttonName = "Quality";
				changeFunction = new buttonDelegate(ChangeQuality);
				break;
			case 1: //Camera View Distance. Not written. May be unfair
				buttonOptions = viewDistanceOptions;
				buttonName = "Camera";
				changeFunction = new buttonDelegate(ChangeViewDistance);
				break;
			case 2: //Sound effects on/off
				buttonOptions = booleanOptions;
				buttonName = "SFX";
				changeFunction = new buttonDelegate(ChangeSFX);
				break;
			case 3: //music on/off
				buttonOptions = booleanOptions;
				buttonName = "Music";
				changeFunction = new buttonDelegate(ChangeMusic);
				break;
			case 4: //Shuffle songs
				buttonOptions = booleanOptions;
				buttonName = "Shuffle";
				changeFunction = new buttonDelegate(ToggleShuffleSongs);
				break;
			case 5: //Display info like FPS
				buttonOptions = booleanOptions;
				buttonName = "Debug";
				changeFunction = new buttonDelegate(ToggleShowLog);
				break;
			case 6: //Post Processing On/Off
				buttonOptions = booleanOptions;
				buttonName = "Render FX";
				changeFunction = new buttonDelegate(TogglePostProcessing);
				break;
			default:
				break;		
		}
	}
	
	private void InitializeSetting() {
		t.text = $"{buttonName}: {buttonOptions[currentSetting]}";	
		//changeFunction(currentSetting);	
		//Debug.Log($"{settingID} == {currentSetting} | {buttonName}: {buttonOptions[currentSetting]}");
	}
	
	private void ChangeSetting(int moveNext = 1) {
		currentSetting = (currentSetting < buttonOptions.Length - 1) ? (currentSetting + moveNext) : 0 ;
		t.text = $"{buttonName}: {buttonOptions[currentSetting]}";	
		changeFunction(currentSetting);
		PlayerPrefs.SetInt($"{settingID}", currentSetting);
	}
	
	//Possible delagates below
	void ChangeQuality(int i) {
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
	
	void ChangeSFX(int i) {
		Settings.sfxOn = (i > 0) ? true : false;
	}
	
	void ChangeMusic(int i) {
		Settings.musicOn = (i > 0) ? true : false;
	}
	
	void ToggleShuffleSongs(int i) {
		Settings.shuffle = (i > 0) ? true : false;
	}
	
	void ToggleShowLog(int i) {
		Settings.showLog = (i > 0) ? true : false;
	}
	
	void TogglePostProcessing(int i) {
		Settings.postProcessing = (i > 0) ? true : false;
		Camera cam = Camera.main;
		Volume fx = cam.GetComponentInChildren<Volume>(true);
		if (fx) {
			fx.enabled = Settings.postProcessing;
		}
	}

	void ChangeShadowSetting(int i){
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
	
	void ChangeViewDistance(int i){
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
