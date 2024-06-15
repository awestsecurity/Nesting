using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Cheats : MonoBehaviour
{
    
	public Text text;
	
	private bool listen = false;
	private string cheat;
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cheat") && !listen) {
			listen = true;
			text.gameObject.SetActive(true);
		} else if (Input.GetButtonDown("Cheat") && listen) {
			EnterCheat(cheat);
		}
		
		if (listen) {
			for (int i = (int)KeyCode.A; i < (int)KeyCode.Z; i++) {
				if (Input.GetKeyDown ((KeyCode)i)) {
				cheat = cheat+((char)i);
			}
		}
			text.text = cheat;
		}
    }
	
	private void EnterCheat(string s){
		if (cheat == "birdsbirdsbirds") {
			PlayerPrefs.SetString("666", "abcdefghijklmnopqrstuvwxyz");
		} else if (cheat == "notme") {
			BirdDetails.cheat = false;
		} else if (cheat == "achieve") {
			UIObjects.achievements.UpdateMetric("TimesPlayedSpring",10);
			UIObjects.achievements.UpdateMetric("TimesPlayedWinter",10);
			UIObjects.achievements.UpdateMetric("TimesPlayedMarsh",10);
		} else if (cheat == "scoreup") {
			GameObject.Find("Katamari").GetComponent<Katamari>().Cheat();
		}
		Debug.Log($"Enter Cheat: {s}");
		cheat = "";
		text.gameObject.SetActive(false);
		listen = false;
		
		
	}
}
