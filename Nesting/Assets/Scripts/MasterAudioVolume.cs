using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to each scene's audio listener
[RequireComponent(typeof(AudioListener))]
public class MasterAudioVolume : MonoBehaviour
{
	private float currentVol;
	
    void Start()
    {
		//listen = gameObject.GetComponent<AudioListener>();
        currentVol = Settings.masterVolume;
		AudioListener.volume = currentVol;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentVol != Settings.masterVolume) {
			currentVol = Settings.masterVolume;
			AudioListener.volume = currentVol;
		}
    }
}
