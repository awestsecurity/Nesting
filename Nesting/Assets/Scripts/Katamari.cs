﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Ball;

public class Katamari : MonoBehaviour {

	public GameObject HUDPrefab;
	private static float displayVolume; // How big we are / What we can pick up.
	private float trueVolume; // Display Volume
	private static float percentPossible = 0.57f;  // How big we are / What we can pick up.
	
	public static float volumeCheck;
	public string mostCommonChild { get {return GetMostCommonChild(); } private set{} }

	public float radius; // Where is the edge to attach things.
	public Transform camPos;
	public CamFollow camFollow;
	public int maxChildren = 200;
	
	private GameObject hud;
	private Text sizeDisplay;
	private Text birdDisplay;
	private RawImage birdui;
	
	private SphereCollider collide;
	private Thingy lastAssimilated;
	private List<string> collected = new List<string>();
	private Ball ballController;
	private bool fullscreen = false;
	
	private int cheatVolume = 9999999;
	private float prevvolume;
	
	void Start () {
		ConnectUI();
		volumeCheck = trueVolume * percentPossible;
		collide = gameObject.GetComponent<SphereCollider>();
		ballController = gameObject.GetComponent<Ball>();
		Texture2D texture = Resources.Load(BirdDetails.birdimg) as Texture2D;
		birdui.texture = texture;
		displayVolume = 1;
		trueVolume = 4888;
		prevvolume = trueVolume;
		StartCoroutine(GravityDelay());
		BirdDetails.score = trueVolume;
	}

	// Update is called once per frame
	void Update () {
		volumeCheck = trueVolume * percentPossible;
		if (!fullscreen && Input.anyKeyDown) {
			fullscreen = true;
			Screen.fullScreen = true;
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (fullscreen && Input.GetKeyDown(KeyCode.Escape)) {
			Screen.fullScreen = false;
			Cursor.lockState = CursorLockMode.None;
		}
		//Fallen through ground somehow
		if (transform.position.y < -15) SceneManager.LoadScene(SceneManager.GetActiveScene().name) ;
		
		//AdjustCameraPosition();
		birdDisplay.text = BirdDetails.birdname;
		sizeDisplay.text = "Mass: "+displayVolume;
		if (displayVolume < trueVolume) displayVolume += Mathf.Round((trueVolume - displayVolume) / 30f);
		if (trueVolume > BirdDetails.score && trueVolume < (BirdDetails.score*1.5f)) {
			BirdDetails.score = trueVolume;
		}
		
		//Cheat Detection
		if (trueVolume > prevvolume * 1.5f) BirdDetails.cheat = true;
		prevvolume = trueVolume;
		if ( trueVolume >= cheatVolume ) BirdDetails.cheat = true;
	}
	
	IEnumerator GravityDelay() {
		yield return new WaitForSeconds(2);
		Rigidbody r = this.GetComponent<Rigidbody>();
		r.isKinematic = false;
	}
	
	void ConnectUI() {
		hud = GameObject.Find("HUD");
		if (hud == null) {
			hud = Instantiate(HUDPrefab);
			hud.transform.GetChild(3).gameObject.SetActive(false);
			hud.transform.GetChild(4).gameObject.SetActive(false);
			hud.transform.GetChild(5).gameObject.SetActive(false);
		}
		hud.transform.GetChild(0).gameObject.SetActive(true);

		sizeDisplay = hud.transform.GetChild(0).GetChild(1).GetComponent<Text>();
		birdDisplay = hud.transform.GetChild(0).GetChild(0).GetComponent<Text>();
		birdui = hud.transform.GetChild(3).GetChild(1).GetComponent<RawImage>();
	}
	
	void OnTriggerEnter (Collider collision) {
		Thingy thingy = collision.gameObject.GetComponent<Thingy>();
		if (thingy != null && SmallEnoughToGrab(thingy)) {
			//Debug.Log($"Picking up: {thingy.thingyName} with {thingy.GetVolume()}g");
			thingy.DisableCollider();
			thingy.assimilated = true;
			Transform tt = thingy.gameObject.transform;
			tt.parent = gameObject.transform;
			tt.localPosition = new Vector3 (tt.localPosition.x * 0.85f, tt.localPosition.y * 0.85f, tt.localPosition.z * 0.85f);
			trueVolume += (thingy.GetVolume() / 40f);
			float adjust = thingy.volume / 350000f;  //5000000 old number
			collide.radius += adjust;
			ballController.m_MovePower += adjust*2;
			camFollow.spacer += adjust*2;
			lastAssimilated = thingy;
			collected.Add(thingy.thingyName);
			RemoveOldChildrenAfterMax();
		}
	}
	
	void RemoveOldChildrenAfterMax() {
		if (this.transform.childCount > maxChildren) {
			Destroy(this.transform.GetChild(1).gameObject);
		}		
	}
	
	bool SmallEnoughToGrab (Thingy t) {
		return (t.volume < (trueVolume * percentPossible)) ? true : false ;
	}
	
	void AdjustCameraPosition () {
		float i = collide.radius * 0.8f;
		Vector3 v = new Vector3 (0, i, i * -1.9f);
		camPos.localPosition = v;
		camPos.LookAt(this.transform);
	}
	
	string GetMostCommonChild() {
		var cnt = new Dictionary<string, int>();
		foreach (string value in collected) {
		   if (cnt.ContainsKey(value)) {
			  cnt[value]++;
		   } else {
			  cnt.Add(value, 1);
		   }
		}
		string mostCommonValue = "whoops";
		int highestCount = 0;
		foreach (KeyValuePair<string, int> pair in cnt) {
		   if (pair.Value > highestCount) {
			  mostCommonValue = pair.Key;
			  highestCount = pair.Value;
		   }
		}
		return mostCommonValue;
	}
	
}