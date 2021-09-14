using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
	private AudioSource speaker;
	public AudioClip song;
	private bool on = false;
	
	void Start() {
		speaker = gameObject.GetComponent<AudioSource>();
		SetSong();
	}
	
    void Update() {
        if (on != Settings.musicOn) {
			on = Settings.musicOn;
			if (on) {
				speaker.Play();
			} else {
				speaker.Stop();
			}
		}
    }
	
	void SetSong() {
		speaker.clip = song;
	}
}
