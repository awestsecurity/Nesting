using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

}

public static class BirdDetails {
	public static int birdid = 12457;
	public static string birdname = "The Vulgar Seagull";
	public static string birdstatus = "EN";
	public static string birdimg = "AmericanRobin";
	
	public static int mapx = 25;
	public static int mapy = 25;
	public static float mapz = 0.5f;
}

//These get assigned by the Network Request Object as it's the first thing to do anything in production
public static class UIObjects {
	public static Popup popup;
	public static SceneControl sceneCon;
}

