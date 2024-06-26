﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;


public class NetworkRequest : GenericSingleton<NetworkRequest>
{
	public string account_name;
	public bool airplaneMode; // no network, skip for play testing
	public bool offWax;
	public bool submitScores = false; // stop submitting high scores to the database
	
	public GameObject ui;
	public Popup popup;
	public SceneControl sceneCon;
	public GameObject pause;
	public BirdMenu menu;
	public MusicPlayer musicPlayer;
	public GameObject titleButtons;
	public Achievements achievements;
	
	private readonly string collection_name = "1forthebirds";
	private string request_owned_templates;
	private string request_template_info;
	private bool birdsLoaded = false;
//	private string waxLogin = "https://all-access.wax.io/cloud-wallet/login";
//	private string waxAccess = "https://api-idm.wax.io/v1/accounts/auto-accept/";
//  private string getAllAssets = "https://wax.api.atomicassets.io/atomicassets/v1/assets?owner=fuqqw.wam&collection_name=1forthebirds&page=1&limit=100&order=desc&sort=asset_id";
	
	void Start() {
		UIObjects.ui = ui;
		UIObjects.popup = popup;
		popup.gameObject.SetActive(true);
		UIObjects.sceneCon = sceneCon;
		UIObjects.pauseMenu = pause;
		UIObjects.birdMenu = menu;
		UIObjects.network = gameObject.GetComponent<NetworkRequest>();
		UIObjects.musicPlayer = musicPlayer;
		UIObjects.achievements = achievements;
		if (Debug.isDebugBuild && !offWax) {
			if (airplaneMode) {
				account_name = "Anonymous";
				StartCoroutine(SkipLogin());
			} else if ( account_name == null || account_name == "" ) {
				//account_name = "fuqqw.wam";
				submitScores = false;
			} else {
				SetLogin(account_name);
			}
		} else if (offWax) {
			submitScores = false;
			account_name = "Birdy";
			//StartCoroutine(SkipLogin());
			FetchBirdsPlayerPrefs();
			menu.MakeBirdMenu();
			popup.Reset();
			birdsLoaded = true;
		}
		Settings.LoadSettings();
	}
	
	public void FetchBirdsPlayerPrefs() {
		string birdsOwned = PlayerPrefs.GetString("666", "a");
		BirdDetails.ownedBirds = new int[birdsOwned.Length];
		int i = 0;
		foreach (char c in birdsOwned) {
			int birdid = 93834;
			if (c == 'a') { birdid = 93834; } //Northern Mockingbird LC
			else if (c == 'b') { birdid = 279598; } //Rock Pigeon LC
			else if (c == 'c') { birdid = 109281; } //Sedge Wren LC
			//
			else if (c == 'd') { birdid = 93847; } //Bachman's Sparrow NT
			else if (c == 'e') { birdid = 95058; } //Loggerhead Shrike NT
			//
			else if (c == 'f') { birdid = 118985; } //Little Woodstar VU
			else if (c == 'g') { birdid = 109280; } //Saint Helena Plover VU
			//
			else if (c == 'h') { birdid = 93873; } //BlackRail EN
			else if (c == 'i') { birdid = 104715; } //Javan Hawk-eagle EN
			//
			else if (c == 'j') { birdid = 104679; } //Bachman's Warbler CR
			else if (c == 'k') { birdid = 104713; } //Guam Kingfisher EW
			else if (c == 'l') { birdid = 104678; } //Marianne White-eye EX
			BirdDetails.ownedBirds[i] = birdid;
			i ++;
		}
	}
	
	// To be called from the outside js code.
    void SetLogin(string a) {
		account_name = a;
		popup.AddMessage($"Loading {account_name}'s birds.");
		popup.LaunchMessagePanel();
		submitScores = true;
		request_owned_templates = $"https://wax.api.atomicassets.io/atomicassets/v1/accounts/{account_name}/{collection_name}";
		StartCoroutine(GetAssets());
	}
	
	IEnumerator SkipLogin() {
		yield return new WaitForSeconds(2);
		sceneCon.StartLoad(Settings.levelSelected);
	}
	
	IEnumerator GetAssets() {
		BirdDetails.ownedBirds = new int[0];
		UnityWebRequest www = UnityWebRequest.Get(request_owned_templates);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {
            //Debug.Log(www.result);
			popup.Reset();
			popup.AddMessage($"Connection Error. Check network or try again in a couple minutes.");
			popup.LaunchMessagePanel();			
        }
		else {
            // Show results as text
			Debug.Log($"Received JSON for Account {account_name}");
            string response = www.downloadHandler.text;
			var data = JSON.Parse(response);
			bool t = data["success"].AsBool;
			if (t) {
				var templates = data["data"]["templates"];
				BirdDetails.ownedBirds = new int[templates.Count];
				for (int i=0;i<templates.Count;i++) {
					int template = data["data"]["templates"][i]["template_id"].AsInt;
					BirdDetails.ownedBirds[i] = template;
				}
				menu.MakeBirdMenu();
				popup.Reset();
			} else {
				//No Inventory
				Debug.Log("Found No Birds");
			}
			birdsLoaded = true;
		}
		
		if ( BirdDetails.ownedBirds.Length == 0 ) {
			Debug.Log("User has no birds.");
			popup.Reset();
			popup.AddMessage($"{account_name} doesn't appear to have any birds.");
			popup.LaunchMessagePanel();
		} else if ( BirdDetails.ownedBirds.Length == 1 ) {
			BirdDetails.SetBird(BirdDetails.ownedBirds[0]);
			//Debug.Log($"User has one bird. Playing as {BirdDetails.birdname}");
			yield return new WaitForEndOfFrame();
			//sceneCon.StartLoad(1);
		} else {
			//Debug.Log("User has more birds. Let them choose.");
			yield return new WaitForEndOfFrame();
		}
	}
	
	public void ChooseRandomBird() {
		menu.SelectRandomBird();
		//if (BirdDetails.ownedBirds.Length >= 1) {
		//	int bird = BirdDetails.ownedBirds[Random.Range(0,BirdDetails.ownedBirds.Length)];
		//	BirdDetails.SetBird(bird);
		//	sceneCon.StartLoad(1);
		//}
	}

	public void PostHighScore() {
		int score = (int)BirdDetails.score;
		StartCoroutine(SendHighScore(score));
	}

	IEnumerator SendHighScore(int score) {
		
		string secret = "greycatbird";
		string url = "https://www.forthebirds.space/play/api/add.php";
		string safename = BirdDetails.birdname.Replace("'",string.Empty);
		string hash = Md5Sum($"{safename}{score}{secret}");

		WWWForm form = new WWWForm();
        form.AddField("bird", safename);
		form.AddField("level", Settings.levelSelected);
        form.AddField("size", score);
        form.AddField("account", account_name);
        form.AddField("name", account_name);
        form.AddField("hash", hash);
		
		if (!BirdDetails.cheat && submitScores) {
			UnityWebRequest www = UnityWebRequest.Post(url,form);
			yield return www.SendWebRequest();
			if(www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {
				Debug.Log("SendScores: "+www.result);
			}
			else {
				string response = www.downloadHandler.text;
				Debug.Log($"score sent. {response}");
			}
		}
	}
	
	public IEnumerator GetHighScoresString() {
		if (!offWax && !airplaneMode) {
			int numScores = 10;
			string url = "https://www.forthebirds.space/play/api/get.php";
			string safename = BirdDetails.birdname.Replace("'",string.Empty);
			string get_url = $"{url}?bird={safename}&amount={numScores}&level={Settings.levelSelected}";
			UnityWebRequest www = UnityWebRequest.Get(get_url);
			yield return www.SendWebRequest();
			if(www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {
				Debug.Log("GetScores: "+www.result);
				BirdDetails.highscores = "Scores Conn Error";
			} else {
				string response = www.downloadHandler.text;
				BirdDetails.highscores = response;
			}
		} else {
			int i = PlayerPrefs.GetInt(BirdDetails.birdname+"1", 0);
			int j = PlayerPrefs.GetInt(BirdDetails.birdname+"3", 0);
			int k = PlayerPrefs.GetInt(BirdDetails.birdname+"4", 0);
			BirdDetails.highscores = $"Your Best \n Spring Meadow: {i.ToString()}\n Winter Wonder: {j.ToString()}\n Sleepy Marsh: {k.ToString()}\n ";
		}
	}

	public string Md5Sum(string strToEncrypt) {
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
	 
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
	 
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
	 
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
	 
		return hashString.PadLeft(32, '0');
	}
	
}