using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class Global : MonoBehaviour {}

public static class BirdDetails {
	public static TextAsset birdsfile;
	public static int[] ownedBirds;
	public static int birdid = 1111; //1111
	public static string birdname = "The Vulgar Seagull";
	public static string birdstatus = "LC";
	public static string birdimg = "Guam Kingfisher";
	
	public static int mapx = 125;
	public static int mapy = 125;
	public static float mapz = 0.5f;
	public static int hexBuffer = 100;
	
	public static float score = 0;
	public static string highscores;
	public static bool cheat = false;
	
	public static void SetBird(int templateID) {
		if (birdsfile == null) { 
			birdsfile = (TextAsset)Resources.Load("birds");
		}
		var birdData = JSON.Parse(birdsfile.text);
		birdid = templateID;
		string stringid = birdid.ToString();
		birdname = birdData[stringid]["name"].Value;
		birdstatus = birdData[stringid]["status"].Value;
		birdimg = birdname;
		Debug.Log($"Bird: {BirdDetails.birdname} - Status: {BirdDetails.birdstatus} - ID:{BirdDetails.birdid}");
	}
	
	public static float[] GetBirdJSONStatAsFloat(int[] templateIDs, string fetch) {
		if (birdsfile == null) { 
			birdsfile = (TextAsset)Resources.Load("birds");
		}
		float[] floats = new float[templateIDs.Length];
		var birdData = JSON.Parse(birdsfile.text);
		for (int i = 0; i < templateIDs.Length; i++) {
			floats[i] = birdData[templateIDs[i].ToString()][fetch].AsFloat;
		}
		return floats;
	}
	
	public static string[] GetBirdJSONStat(int[] templateIDs, string fetch) {
		if (birdsfile == null) { 
			birdsfile = (TextAsset)Resources.Load("birds");
		}
		string[] stats = new string[templateIDs.Length];
		var birdData = JSON.Parse(birdsfile.text);
		for (int i = 0; i < templateIDs.Length; i++) {
			stats[i] = birdData[templateIDs[i].ToString()][fetch].Value;
		}
		return stats;
	}
	
}

//These get assigned by the Network Request Object as it's the first thing to do anything in production
public static class UIObjects {
	public static GameObject ui;
	public static Popup popup;
	public static SceneControl sceneCon;
	public static GameObject pauseMenu;
	public static BirdMenu birdMenu;
	public static NetworkRequest network;
	public static MusicPlayer musicPlayer;
	public static Achievements achievements;
}

public static class Settings {
	
	public static int levelSelected = 1;
	public static bool shadows = true;
	public static bool skipText = false;	
	public static bool musicOn = true;
	public static bool shuffle = true;
	public static bool sfxOn = true;
	public static bool showLog = false;
	public static bool postProcessing = true;
	public static int collectedMax = 234; //How many objects should the katarami hold and render.

	public static void LoadSettings() {
		Settings.musicOn = (PlayerPrefs.GetInt("3", 1) == 1) ? true : false ;
		Settings.sfxOn = (PlayerPrefs.GetInt("2", 1) == 1) ? true : false ;
		Settings.shuffle = (PlayerPrefs.GetInt("4", 1) == 1) ? true : false ;
		Settings.showLog = (PlayerPrefs.GetInt("5", 0) == 1) ? true : false ;
		Settings.postProcessing = (PlayerPrefs.GetInt("6", 1) == 1) ? true : false ;
		Settings.collectedMax = (PlayerPrefs.GetInt("7", 2)+2)*75;
		int[] quality = new int[] {1,3,5};
		QualitySettings.SetQualityLevel(quality[PlayerPrefs.GetInt("0", 1)], true);

		//Debug.Log($"PPrefs Music: {Settings.musicOn} and sfx {Settings.sfxOn}");
	}
	
}
