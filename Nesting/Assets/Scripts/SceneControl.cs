using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneControl : MonoBehaviour
{
    
	private bool isLoading;

	public GameObject background;
	public Popup popup;
	public GameObject timerDisplay;
	public Text textbox;
	public string message;
	
	private string[] facts;
	private bool playing = false;
	private bool paused = false;
	
	public float timeremaining {get; private set;}
	public Text timeDisplay;
	
	void Start() {
		timeremaining = 555;
		TextAsset file = (TextAsset)Resources.Load("BirdFacts");
		facts = file.text.Split('\n');
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
			int min = (int)Mathf.Floor(timeremaining / 60);
			int sec = (int)timeremaining % 60;
			timeDisplay.text = $"{min}:{sec.ToString("00")}";
		}
		if (playing && timeremaining <= 0) {
			EndPlay();
			//move to endgame scene
		}
		
		//PAUSING
		if (Input.GetKeyDown("p")) {
			paused = !paused;
			if (paused) {
				UIObjects.pauseMenu.SetActive(true);
				Cursor.lockState = CursorLockMode.None;
				Time.timeScale = 0;
			} else {
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
		float minTime = 5f;
		popup.Reset();
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
			Random.InitState(BirdDetails.birdid);
			timeremaining = GetPlaytime();
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
