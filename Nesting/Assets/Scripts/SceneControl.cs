using System.Collections;
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

public class SceneControl : MonoBehaviour
{
    
	private bool isLoading;

	public GameObject background;
	public Popup popup;
	public GameObject timerDisplay;
	public Text textbox;
	public Text highscore;
	public GameObject titleScreen;
	
	private string[] facts;
	private bool playing = false;
	private bool paused = false;
	
	public float timeremaining {get; private set;}
	private float lastTimeStamp = 0;
	private bool cheatDetected = false;
	public Text timeDisplay;
	
	public GameObject firstUIElemOnPause;
	private EventSystem events;
	
	void Start() {
		timeremaining = 555;
		AudioListener.volume = 0f;
		TextAsset file = (TextAsset)Resources.Load("BirdFacts");
		facts = file.text.Split('\n');
		events = EventSystem.current;
	}
	
	public bool StartLoad(int sceneindex) {
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
		}
		if (playing && (timeremaining <= 0 || cheatDetected)) {
			EndPlay();
			//move to endgame scene
		}
		
		//PAUSING

		if (CrossPlatformInputManager.GetButtonDown("Pause")) {
			paused = !paused;
			bool priorState = playing;
			if (paused) {
				playing = false;
				UIObjects.pauseMenu.SetActive(true);
		        events.SetSelectedGameObject(firstUIElemOnPause);
				if (BirdDetails.highscores != null) {
					highscore.text = $"Best {BirdDetails.birdname}s \n {BirdDetails.highscores}";
				}
				Cursor.lockState = CursorLockMode.None;
				Time.timeScale = 0;
			} else {
				playing = priorState;
				UIObjects.pauseMenu.SetActive(false);
				Cursor.lockState = CursorLockMode.Locked;
				Time.timeScale = 1;
			}
		}
	}
	
	private void EndPlay() {
		//Time.timeScale = 0.1f;
		Debug.Log("TimeOver");
		playing = false;
		UIObjects.network.PostHighScore();
		StartLoad(2);
	}
	
	IEnumerator PreLoadScene(int sceneindex, float delay = 1.25f) {
		if (sceneindex != 1) {
			Random.InitState(Mathf.RoundToInt(Time.deltaTime));
		}
		float minTime = 5f;
		popup.Reset();
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneindex);
		isLoading = true;
		background.SetActive(true);
		titleScreen.SetActive(false);
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
		textbox.text = "";
		
		// IF WE ARE LOADING THE GAME PLAY SCENE
		if (sceneindex == 1) {
			timeremaining = GetPlaytime();
			lastTimeStamp = timeremaining + 0.1f;
			timerDisplay.SetActive(true); //enable timer
			Cursor.lockState = CursorLockMode.Locked;
			playing = true;
			StartCoroutine(UIObjects.network.GetHighScores());
		} else if (sceneindex == 2) {
			//End Scene
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
	
	private float GetPlaytime() {
		switch(BirdDetails.birdstatus) {
			case "LC":
				return 250;
			case "NT":
				return 350;
			case "VU":
				return 450;	
			case "EN":
				return 600;
			case "CR":
				return 800;
			case "EW":
				return 999;
			case "EX":
				return 9999;
			default:
				return 60;
		}
	}
	
}
