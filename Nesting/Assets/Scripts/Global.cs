using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class Global : MonoBehaviour {}

public static class BirdDetails {
	public static TextAsset birdsfile;
	public static int[] ownedBirds;
	public static int birdid = 1111;
	public static string birdname = "The Vulgar Seagull";
	public static string birdstatus = "LC";
	public static string birdimg = "AmericanRobin";
	
	public static int mapx = 25;
	public static int mapy = 25;
	public static float mapz = 0.5f;
	
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
		birdimg = birdData[stringid]["image"].Value;
		Debug.Log($"Bird: {BirdDetails.birdname} - Status: {BirdDetails.birdstatus} - IMG: {BirdDetails.birdimg} - ID:{BirdDetails.birdid}");
	}
	
	public static string[] GetBirdImages(int[] templateIDs) {
		if (birdsfile == null) { 
			birdsfile = (TextAsset)Resources.Load("birds");
		}
		string[] images = new string[templateIDs.Length];
		var birdData = JSON.Parse(birdsfile.text);
		for (int i = 0; i < templateIDs.Length; i++) {
			images[i] = birdData[templateIDs[i].ToString()]["image"].Value;
		}
		return images;
	}
	
}

//These get assigned by the Network Request Object as it's the first thing to do anything in production
public static class UIObjects {
	public static Popup popup;
	public static SceneControl sceneCon;
	public static GameObject pauseMenu;
	public static BirdMenu birdMenu;
	public static NetworkRequest network;
}

public static class Settings {
	public static bool shadows = true;
	public static bool skipText = false;	
}
// QualitySettings.shadows = ShadowQuality.All;
// QualitySettings.shadows = ShadowQuality.HardOnly;
// QualitySettings.shadows = ShadowQuality.Disable; 

