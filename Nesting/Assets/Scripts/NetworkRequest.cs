using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;


public class NetworkRequest : MonoBehaviour
{
	public string account_name;
	public bool AirplaneMode; // no network, skip for play testing
	public Popup popup;
	public SceneControl sceneCon;
	private readonly string collection_name = "1forthebirds";
	private string request_owned_templates;
	private string request_template_info;
//	private string waxLogin = "https://all-access.wax.io/cloud-wallet/login";
//	private string waxAccess = "https://api-idm.wax.io/v1/accounts/auto-accept/";
//  private string getAllAssets = "https://wax.api.atomicassets.io/atomicassets/v1/assets?owner=fuqqw.wam&collection_name=1forthebirds&page=1&limit=100&order=desc&sort=asset_id";
	private int[] owned_templates;
	
	void Start() {
		UIObjects.popup = popup;
		UIObjects.sceneCon = sceneCon;
		if (account_name == "fuqqw.wam" && !AirplaneMode) {
			SetLogin(account_name);
		} else if (AirplaneMode) {
			StartCoroutine(SkipLogin());
		}
	}

	// To be called from the outside js code.
    void SetLogin(string a) {
		account_name = a;
		popup.AddMessage($"Loading {account_name}'s birds.");
		popup.LaunchMessagePanel();
		request_owned_templates = $"https://wax.api.atomicassets.io/atomicassets/v1/accounts/{account_name}/{collection_name}";
		StartCoroutine(GetAssets());
	}
	
	IEnumerator SkipLogin() {
		yield return new WaitForSeconds(3);
		sceneCon.StartLoad(1);
	}
	
	IEnumerator GetAssets() {
		Debug.Log($"Getting Assets for Account {account_name}");
		UnityWebRequest www = UnityWebRequest.Get(request_owned_templates);
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
		else {
            // Show results as text
			Debug.Log($"Received JSON for Account {account_name}");
            string response = www.downloadHandler.text;
			var data = JSON.Parse(response);
			bool t = data["success"].AsBool;
			if (t) {
				Debug.Log("Success");
				var templates = data["data"]["templates"];
				owned_templates = new int[templates.Count];
				for (int i=0;i<templates.Count;i++) {
					int template = data["data"]["templates"][i]["template_id"].AsInt;
					owned_templates[i] = template;
				}
			} else {
				//No Inventory
				Debug.Log("Found No Birds");
			}
		}
		int bird = owned_templates[Random.Range(0,owned_templates.Length)];
		TextAsset birdData = (TextAsset)Resources.Load("birds");
		var birds = JSON.Parse(birdData.text);
		Debug.Log(bird.ToString());
		BirdDetails.birdid = bird;
		BirdDetails.birdname = birds[bird.ToString()]["name"].Value;
		BirdDetails.birdstatus = birds[bird.ToString()]["status"].Value;
		BirdDetails.birdimg = birds[bird.ToString()]["image"].Value;
		Debug.Log($"Bird: {BirdDetails.birdname} - Status: {BirdDetails.birdstatus} - IMG: {BirdDetails.birdimg} - ID:{BirdDetails.birdid}");
		string name = BirdDetails.birdname;
		string status = BirdDetails.birdstatus;
		yield return new WaitForSeconds(1.5f);
		sceneCon.StartLoad(1);
	}
	
}