﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

//Handle scene transitions
//Keep play time
//Manage Loading screen
//Set random seed

public class SceneControl : GenericSingleton<SceneControl>
{
   	private bool isLoading;

	public GameObject background;
	public Popup popup;
	public GameObject timerDisplay;
	public Text textbox;
	public Text highscore;
	public GameObject titleScreen;
	public GameObject katamari {get;set;} //katamari sets itself here.
	private GameObject titleButtons;

	private string[] facts;
	private bool playing = false;
	private bool paused = false;
	private bool priorState;
	private int activeScene;

	public float timeremaining {get; private set;}
	public Text timeDisplay;
	private float lastTimeStamp = 0;
	private bool cheatDetected = false;

	public GameObject firstUIElemOnPause;
	private EventSystem events;

	void Start() {
		timeremaining = 555;
		TextAsset file = (TextAsset)Resources.Load("BirdFacts");
		facts = file.text.Split('\n');
		events = EventSystem.current;
		titleButtons = titleScreen.transform.GetChild(0).gameObject;
	}

	public bool StartLoad(int sceneindex) {
		//Debug.Log("trying to load scene: "+sceneindex);
		if (isLoading) {
			return false;
		} else {
			StartCoroutine(PreLoadScene(sceneindex));
			return true;
		}
	}

	void Update() {
		//PLAY TIMER
		if (playing) {
			timeremaining = timeremaining - Time.deltaTime;
			if (lastTimeStamp <= timeremaining) {
				cheatDetected = true;
				BirdDetails.cheat = true;
			}
			int min = (int)Mathf.Floor(timeremaining / 60);
			int sec = (int)timeremaining % 60;
			timeDisplay.text = $"{min}:{sec.ToString("00")}";
			lastTimeStamp = timeremaining;

			if(Input.GetKeyDown("q") && Debug.isDebugBuild) {
				timeremaining -= 100;
			}
		}
		if (playing && (timeremaining <= 0 || cheatDetected)) {
			//UIObjects.achievements.UpdateMetric
			EndPlay();	//move to endgame scene
		}

		//PAUSING
		if (CrossPlatformInputManager.GetButtonDown("Pause") && activeScene == 1) {
			if (!paused) priorState = playing; //This is needed if we allow pausing on non-game scenes.
			paused = !paused;
			if (paused) {
				PauseGame();
		        events.SetSelectedGameObject(firstUIElemOnPause);
				if (BirdDetails.highscores != null) {
					highscore.text = $"Best {BirdDetails.birdname}s \n {BirdDetails.highscores}";
				}
				//Cursor.lockState = CursorLockMode.None;
			} else {
				PauseGame(false);
				//playing = priorState;
				//Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}

	//Pause Game
	private void PauseGame(bool p = true) {
		playing = !p;
		UIObjects.pauseMenu.SetActive(p);
		Time.timeScale = ( p ) ? 0 : 1;
	}

	private void EndPlay() {
		playing = false;
		if (Settings.levelSelected == 1) {UIObjects.achievements.UpdateMetric("TimesPlayedSpring");}
		else if (Settings.levelSelected == 3) {UIObjects.achievements.UpdateMetric("TimesPlayedWinter");}
		else if (Settings.levelSelected == 4) {UIObjects.achievements.UpdateMetric("TimesPlayedMarsh");}
		UIObjects.achievements.UpdateMetric("TotalMass", (int)BirdDetails.score);
		UIObjects.network.PostHighScore();
		UIObjects.achievements.UpdateMetric("EggFound", 0);
		StartLoad(2);
	}

	public void EndPlayEarly() {
		//Debug.Log("Quit");
		PauseGame(false);
		paused = false;
		playing = false;
		timeremaining = 555;
		UIObjects.achievements.UpdateMetric("EggFound", 0);
		StartLoad(0);
	}

	IEnumerator PreLoadScene(int sceneindex, float delay = 1.25f) {
		if (sceneindex != 1) {
			Random.InitState(Mathf.RoundToInt(Time.deltaTime));
		} 
		if (sceneindex != 0) { titleScreen.SetActive(false); }
		float minTime = 5f;
		popup.Reset();
		int i = sceneindex;
		if (sceneindex == 1) {
			i = Settings.levelSelected;
			UIObjects.achievements.UpdateMetric("Level", i);
			int rarity = StatusToNumber();
			UIObjects.achievements.UpdateMetric("BirdRarity", rarity);
		}
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(i);
		isLoading = true;
		background.SetActive(true);
		string randfact = facts[Random.Range(0,facts.Length)];
		textbox.text = $"Loading...   Did you know, {randfact}";
		while (!loadingOperation.isDone)
        {
			minTime -= Time.deltaTime;
            yield return null;
        }
		float t = Time.realtimeSinceStartup+delay;
		while (Time.realtimeSinceStartup < t || minTime > 0) {
			minTime -= Time.deltaTime;
			t -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		StartCoroutine(FadeScreen());
		isLoading = false;
		activeScene = sceneindex;
		textbox.text = "";

		// IF WE ARE LOADING THE GAME PLAY SCENE
		if (sceneindex == 1) {
			timeremaining = GetPlaytime();
			lastTimeStamp = timeremaining + 0.1f;
			timerDisplay.SetActive(true); //enable timer
			Cursor.lockState = CursorLockMode.Locked;
			playing = true;
			StartCoroutine(UIObjects.network.GetHighScores());
		} else if (sceneindex == 0) {
			//Title Scene
			titleScreen.SetActive(true);
			titleButtons.SetActive(true);
			Random.InitState(Mathf.RoundToInt(Time.deltaTime));
			if (katamari) {
				Debug.Log($"katamari destroyed? {katamari.name}");
				Destroy(katamari);
			}
		}
	}

	IEnumerator FadeScreen(bool fadeout = true, float fadeTime = 0.85f) {
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

	public void ChangeScene(int index){
		StartLoad(index);
	}

	private float GetPlaytime() {
		switch(BirdDetails.birdstatus) {
			case "LC":
				return 220; //250
			case "NT":
				return 330;
			case "VU":
				return 440;
			case "EN":
				return 590;
			case "CR":
				return 750;
			case "EW":
				return 900;
			case "EX":
				return 9999;
			case "DD":
				return 5555;
			default:
				return 60;
		}
	}
	
	private int StatusToNumber() {
		switch(BirdDetails.birdstatus) {
			case "LC":
				return 1;
			case "NT":
				return 2;
			case "VU":
				return 3;
			case "EN":
				return 4;
			case "CR":
				return 5;
			case "EX":
				return 6;
			case "DD":
				return 7;
			default:
				return 0;
		}
	}

}
