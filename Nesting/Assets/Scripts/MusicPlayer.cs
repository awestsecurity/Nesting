using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
	private AudioSource speaker;
	public AudioClip menuSong;
	public AudioClip endSong;
	public AudioClip[] songList;
	
	private bool on = true;
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
	
    void Update() {
        if (on != Settings.musicOn) {
			on = Settings.musicOn;
			PlayStop();
		}
		
		if (on && !speaker.isPlaying) {
			SetSong(songList[nextSongIndex]);
			nextSongIndex += 1;
			if (nextSongIndex >= songList.Length) {
				nextSongIndex = 0; 
			}
			speaker.Play();
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
	
	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		int index = scene.buildIndex;
        Debug.Log("OnSceneLoaded: " + index);
		if ( index == 0 ) {
			SetSong(menuSong);
			speaker.loop = true;
			if (on) {
				speaker.Play();
			}
		}
		if ( index == 1 ) {
			//This is the play level
			speaker.loop = false;
		}
		if ( index == 2 ) {
			//End scene
			SetSong(endSong);
			speaker.loop = true;
			if (on) {
				speaker.Play();
			}
		}
    }

}
