using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;


public class NetworkRequest : GenericSingleton<NetworkRequest>
{
	public string account_name;
	public bool airplaneMode; // no network, skip for play testing
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
		if (Debug.isDebugBuild) {
			if (airplaneMode) {
				account_name = "Anonymous";
				StartCoroutine(SkipLogin());
			} else if ( account_name == null || account_name == "" ) {
				//account_name = "fuqqw.wam";
				submitScores = false;
			} else {
				SetLogin(account_name);
			}
		}
		LoadSettings();
	}
	
	void LoadSettings() {
		Settings.musicOn = (PlayerPrefs.GetInt("3", 1) == 1) ? true : false ;
		Settings.sfxOn = (PlayerPrefs.GetInt("2", 1) == 1) ? true : false ;
		Settings.shuffle = (PlayerPrefs.GetInt("4", 1) == 1) ? true : false ;
		Settings.showLog = (PlayerPrefs.GetInt("5", 1) == 1) ? true : false ;
		Settings.postProcessing = (PlayerPrefs.GetInt("6", 1) == 1) ? true : false ;
		int[] quality = new int[] {1,3,5};
		QualitySettings.SetQualityLevel(quality[PlayerPrefs.GetInt("0", 1)], true);

		//Debug.Log($"PPrefs Music: {Settings.musicOn} and sfx {Settings.sfxOn}");
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
		//Debug.Log($"Getting Assets for Account {account_name}");
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
	
	public IEnumerator GetHighScores() {
		int numScores = 10;
		string url = "https://www.forthebirds.space/play/api/get.php";
		string safename = BirdDetails.birdname.Replace("'",string.Empty);
		string get_url = $"{url}?bird={safename}&amount={numScores}&level={Settings.levelSelected}";
		
		if (!airplaneMode) {
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
			BirdDetails.highscores = "Airplane Mode - No Scores";
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