using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneControl : MonoBehaviour
{
    
	private bool isLoading;

	public bool ready;
	public GameObject background;
	public Popup popup;
	public Text textbox;
	public string message;
	
	private string[] facts;
	private bool tutorial = true;
	private bool playing = false;
	
	public float timeremaining {get; private set;}
	public Text timeDisplay;
	
	void Start() {
		timeremaining = 555;
		TextAsset file = (TextAsset)Resources.Load("BirdFacts");
		facts = file.text.Split('\n');
	}
	
	public bool StartLoad(int sceneindex) {
		if (isLoading || ready) {
			return false;
		} else {
			StartCoroutine(PreLoadScene(sceneindex));
			return true;
		}		
	}
	
	void Update() {
		if (playing) {	
			timeremaining = timeremaining - Time.deltaTime; 
			int min = (int)Mathf.Floor(timeremaining / 60);
			int sec = (int)timeremaining % 60;
			timeDisplay.text = $"{min}:{sec.ToString("00")}";
		}
		if (playing && timeremaining <= 0) {
			EndPlay();
			//move to endgame scene
		}
	}
	
	private void EndPlay() {
		//Time.timeScale = 0.1f;
		Debug.Log("TimeOver");
		playing = false;
		StartCoroutine(PreLoadScene(2));
	}
	
	IEnumerator PreLoadScene(int sceneindex, float delay = 1.25f) {
		float minTime = 5f;
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneindex);
		isLoading = true;
		background.SetActive(true);
		string randfact = facts[Random.Range(0,facts.Length)];
		textbox.text = $"Loading...   Did you know, {randfact}";
		while (!loadingOperation.isDone)
        {
			minTime -= Time.deltaTime;
            yield return null;
        }
		ready = true;
		float t = Time.realtimeSinceStartup+delay;
		while (Time.realtimeSinceStartup < t && minTime > 0) {
			minTime -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(FadeScreen());
		textbox.text = "";
		popup.Reset();
		if (tutorial && sceneindex == 1) {
			popup.AddMessage("Oh good, you're here. Go gather things for the nest.");
			popup.AddMessage("No time waste! Go go go gogogogogogogog");
			popup.LaunchMessagePanel();
			tutorial = false;
			timeremaining = GetPlaytime();
			playing = true;
		} else if (sceneindex == 1) {
			popup.AddMessage("No time waste! We need more nests.");
			popup.AddMessage("Try harder tis time.");
			popup.LaunchMessagePanel();	
			timeremaining = GetPlaytime();
			playing = true;
		}
	}
	
	IEnumerator FadeScreen(bool fadeout = true, float fadeTime = 0.75f) {
		Image img = background.GetComponent<Image>();
		Color c = img.color;
	    float elapsedTime = 0.0f;
		while (elapsedTime < fadeTime)
		{
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.unscaledDeltaTime ;
			if (fadeout) {	c.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime); }
			else { c.a = Mathf.Clamp01(elapsedTime / fadeTime); }
			img.color = c;
		}
		background.SetActive(false);
		c.a = 1.0f;
		img.color = c;
	}
	
	private float GetPlaytime() {
		switch(BirdDetails.birdstatus) {
			case "LC":
				return 180;
			case "NT":
				return 240;
			case "VU":
				return 360;	
			case "EN":
				return 480;
			case "CR":
				return 600;
			case "EW":
				return 720;
			case "EX":
				return 840;
			default:
				return 60;
		}
	}
	
}
