using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : GenericSingleton<MusicPlayer>
{
	private AudioSource speaker;
	public AudioClip menuSong;
	public AudioClip endSong;
	public AudioClip[] songList;
	
	public Text songCredit;
	private int creditDisplayTime = 10;
	
	private bool on = true;
	private int prevSceneIndex = 0;
	private int nextSongIndex = 0;
	private bool playList;
	
	void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	void OnEnable() {
		if (speaker == null) {
			speaker = gameObject.GetComponent<AudioSource>();
			SetSong(menuSong);
		}
		on = Settings.musicOn;
		PlayStop();
	}
	
	public void PrimeLevelSong(AudioClip s) {
		if (!Settings.shuffle) {
			int index = System.Array.IndexOf(songList, s);
			nextSongIndex = index;
		} else {
			nextSongIndex = GetRandSongI();
		}
	}
	
	IEnumerator PopSongCredits() {
		songCredit.text = speaker.clip.name;
		songCredit.gameObject.SetActive(true);
		yield return new WaitForSeconds(creditDisplayTime);
		songCredit.gameObject.SetActive(false);
	}
	
    void Update() {
        if (on != Settings.musicOn) {
			on = Settings.musicOn;
			PlayStop();
		}
		
		if (on && !speaker.isPlaying && Application.isFocused) {
			nextSongIndex = GetRandSongI();
			//Debug.Log($"next song: {nextSongIndex}, of {songList.Length}");
			SetSong(songList[nextSongIndex]);
			nextSongIndex += 1;
			if (nextSongIndex >= songList.Length) {
				nextSongIndex = 0; 
			}
			speaker.Play();
			StartCoroutine(PopSongCredits());
		}
    }
	
	void PlayStop() {
		if (on) {
			speaker.Play();
		} else {
			speaker.Stop();
		}
	}
	
	void SetSong(AudioClip song) {
		speaker.clip = song;
	}
	
	//return a random song index that is different than the current song.
	int GetRandSongI() {
		var rand = new System.Random();
		int prev = nextSongIndex > 0 ? nextSongIndex-1 : songList.Length-1 ;
		int next = rand.Next(songList.Length);
		while (next == prev) {
			next = rand.Next(songList.Length);
		}
		return next;
	}
	
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		int index = scene.buildIndex;
		if (index == prevSceneIndex) {
			return;
		} else {
			prevSceneIndex = index;
		}
        //Debug.Log("OnSceneLoaded: " + index);
		if ( index == 0 ) {
			SetSong(menuSong);
			speaker.loop = true;
			if (on) {
				speaker.Play();
			}
		}
		if ( index != 0 && index != 2 ) {
			//This is the play level
			speaker.loop = false;
			StartCoroutine(PopSongCredits());
		}
		//Removed the end play song for now
		//Uncomment to have a specific song loop during the nest swirl
		if ( index == 2 ) {
			//SetSong(endSong);
			//speaker.loop = true;
			//if (on) {
			//	speaker.Play();
			//}
		}
    }
	
}
