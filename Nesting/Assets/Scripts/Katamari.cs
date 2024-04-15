﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Ball;

[RequireComponent(typeof(AudioSource))]
public class Katamari : MonoBehaviour {

	public GameObject HUDPrefab;
	private static float displayVolume; // What's Showing in the textbox.
	private float trueVolume; // Everything added and used as the final score
	private static float percentPossible = 0.33f;  // How big we are / What we can pick up.
	private static float startingMass = 105;
	private Vector3 tuckInObject = new Vector3(0.85f,0.85f,0.85f);
	
	public static float volumeCheck;
	public string mostCommonChild { get {return GetTopThings()[0]; } private set{} }
	private string[] childrenByQuantity;

	public float radius; // Where is the edge to attach things.
	public Transform camPos;
	public CamFollow camFollow;
	public int maxChildren = 234;
	
	private GameObject hud;
	private Text sizeDisplay;
	private Text birdDisplay;
	
	private SphereCollider collide;
	private Thingy lastAssimilated;
	private List<string> collected = new List<string>();
	private Ball ballController;
	private Rigidbody rbody;
	private Light glow;
	private bool fullscreen = false;
	
	private int cheatVolume = 9999999;
	private float prevvolume;
	
	private AudioSource speaker;
	public AudioClip waterSplash;
	public AudioClip plodNoise;
	public AudioClip[] basePickupSounds;
	
	void Start () {
		//revrse singleton
		if (!SetKatamariInSceneControl()) {
			Debug.LogWarning("Testing Mode?: Scene Con not found.");
		}
		ConnectUI();
		volumeCheck = VolumeCheck();
		collide = gameObject.GetComponent<SphereCollider>();
		ballController = gameObject.GetComponent<Ball>();
		speaker = gameObject.GetComponent<AudioSource>();
		rbody = gameObject.GetComponent<Rigidbody>();
		displayVolume = 1;
		trueVolume = 1;
		prevvolume = trueVolume;
		StartCoroutine(GravityDelay());
		BirdDetails.score = trueVolume;
	}

	// Update is called once per frame
	void Update () {
		volumeCheck = VolumeCheck();
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Screen.fullScreen = false;
			Cursor.lockState = CursorLockMode.None;
		}
		
		//Fallen through ground somehow
		if (transform.position.y < -15) SceneManager.LoadScene(SceneManager.GetActiveScene().name) ;
		
		//AdjustCameraPosition();
		birdDisplay.text = BirdDetails.birdname;
		sizeDisplay.text = string.Format("Mass: {0:#.00}", displayVolume);
		if (displayVolume < trueVolume) displayVolume += ((trueVolume - displayVolume) / 40f);
		//Anti-Cheat added for huge increases in a single frame
		if (trueVolume > BirdDetails.score && trueVolume < (BirdDetails.score*30f)) {
			BirdDetails.score = trueVolume;
		}
		
		//Debug.Log (rbody.velocity.magnitude);
		if (!speaker.isPlaying && ballController.grounded) {
			PlaySFX(plodNoise, rbody.velocity.magnitude/17f);
		}
		
		//More Cheat Detection
		if (trueVolume > prevvolume * 1.5f) BirdDetails.cheat = true;
		prevvolume = trueVolume;
		if ( trueVolume >= cheatVolume ) BirdDetails.cheat = true;
	}
	
	//Hold off on the gravity to allow easy settling of objects and player
	IEnumerator GravityDelay() {
		yield return new WaitForSeconds(2);
		Rigidbody r = this.GetComponent<Rigidbody>();
		r.isKinematic = false;
	}
	
	//Get UI for displaying stats
	//Create if there isn't one
	void ConnectUI() {
		hud = GameObject.Find("HUD");
		if (hud == null) {
			hud = Instantiate(HUDPrefab);
			hud.transform.GetChild(2).GetChild(0).gameObject.SetActive(false); //Deactivate titlescreen
			//hud.transform.GetChild(3).gameObject.SetActive(false); //Deactivate announcment
			hud.transform.GetChild(3).gameObject.SetActive(false); //Deactivate Loading Screen
			hud.transform.GetChild(2).GetChild(3).gameObject.SetActive(false); //Bird Menu
		}
		GameObject statPanel = hud.transform.GetChild(0).gameObject;
		statPanel.SetActive(true); //Activate Stat Panel
		birdDisplay = statPanel.transform.GetChild(0).GetComponent<Text>();
		sizeDisplay = statPanel.transform.GetChild(1).GetComponent<Text>();
	}
	
	//Collided with something. We only care if it is a "Thingy" (can be picked up)
	void OnTriggerEnter (Collider collision) {
		Thingy thingy = collision.gameObject.GetComponent<Thingy>();
		if (thingy != null && SmallEnoughToGrab(thingy)) {
			//Debug.Log($"Picking up: {thingy.thingyName} with {thingy.GetVolume()}g");
			PlaySFX(basePickupSounds[Random.Range(0,basePickupSounds.Length)]);
			thingy.DisableCollider();
			thingy.assimilated = true;
			Transform tt = thingy.gameObject.transform;
			tt.parent = gameObject.transform;
			tt.localPosition = Vector3.Scale(tt.localPosition, tuckInObject);
			float volume = thingy.GetVolume();
			AddVolumeToKatamari(volume, thingy.modifier);
			lastAssimilated = thingy;
			collected.Add(thingy.thingyName);
			UpdateCollectedStats(thingy.thingyName);
			RemoveOldChildrenAfterMax();
			if (thingy is ThingyGlow) {
				var l = thingy as ThingyGlow;
				AbsorbLight(l);
			}
			Destroy(thingy.GetComponent<Movement>());
		} else if (collision.gameObject.name == "Water"){
			PlaySFX(waterSplash);
		}
	}
	
	//For spooky level. Katamari will accumilate light from glowing things.
	IEnumerator AbsorbLight(ThingyGlow l){
		if (!glow) {
			glow = gameObject.AddComponent<Light>() as Light;
			glow.intensity = 0.1f;
			glow.range = 0.1f;
			glow.color = Color.green / 2f;
			glow.type = LightType.Point;
		}
		l.StartLightFade();
		int target = 1;
		while (glow.intensity < target) {
			yield return new WaitForSeconds(0.1f);
			glow.intensity += 0.1f;
		}
	}
	
	void PlaySFX(AudioClip sfx, float loudness = 0.666f) {
		if (Settings.sfxOn) {
			speaker.PlayOneShot(sfx, loudness);
		}
	}
	
	//Handle the expansion given the thingy volume and modifier
	void AddVolumeToKatamari(float vol, float mod = 1) {
		trueVolume += (vol*mod / 8f);
		float adjust = vol / 35000f;  //5000000 old number
		collide.radius += adjust * 0.75f;
		ballController.m_MovePower += adjust * 2;
		camFollow.spacer += adjust * 2.15f;
	}
	
	//Remove the oldest attached object when we have too many
	void RemoveOldChildrenAfterMax() {
		if (this.transform.childCount > maxChildren) {
			Destroy(this.transform.GetChild(1).gameObject);
		}		
	}
	
	
	bool SmallEnoughToGrab (Thingy t) {
		return (t.GetVolume() < VolumeCheck()) ? true : false ;
	}
	
	//This is the number that tests what objects can be picked up. 
	float VolumeCheck() {
		return (startingMass + trueVolume) * percentPossible;
	}
	
	void AdjustCameraPosition () {
		float i = collide.radius * 0.8f;
		Vector3 v = new Vector3 (0, i, i * -1.9f);
		camPos.localPosition = v;
		camPos.LookAt(this.transform);
	}
	
	Dictionary<string, int> ChildFrequencyDict() {
		var cnt = new Dictionary<string, int>();
		foreach (string value in collected) {
		   if (cnt.ContainsKey(value)) {
			  cnt[value]++;
		   } else {
			  cnt.Add(value, 1);
		   }
		}
		return cnt;
	}
	
	//switch to list and it all gets easier
	public string[] GetTopThings(int amount = 3) {
		Dictionary<string, int> dict = ChildFrequencyDict();
		string[] rank = new string[amount];
		for(int i = 0; i < amount; i++) {
		   rank[i] = "empty space";
		}
		int highest = 0;
		foreach (KeyValuePair<string, int> pair in dict) {
		   if (pair.Value > highest) {
			   if (rank[0] == "empty space") {
					rank[0] = pair.Key;
			   } else if (rank[1] == "empty space") {
					rank[1] = rank[0];
					rank[0] = pair.Key;
			   } else {
					rank[2] = rank[1];
					rank[1] = rank[0];
					rank[0] = pair.Key;
				}
			   highest = pair.Value;
		   }

		}		
		if (dict.ContainsKey("Flower")) {
			UpdateCollectedStats("Flower", dict["Flower"]);
		}
		if (dict.ContainsKey("SnowHead")) {
			UpdateCollectedStats("SnowHead", dict["SnowHead"]);
		}
		return rank;
	}
	
	//Add to collection stats for achievements
	private void UpdateCollectedStats(string thingName, int amount = 1) {
		if (thingName == "Mushroom") {
			UIObjects.achievements.UpdateMetric("MushroomsCollected", amount);
		} else if (thingName == "Egg") {
			UIObjects.achievements.UpdateMetric("EggFound", 1);
		} else if (thingName == "Flower") { 
			UIObjects.achievements.UpdateMetric("FlowersIn1Run", amount); 
		} else if (thingName == "SnowHead") {
			UIObjects.achievements.UpdateMetric("SnowmenIn1Run", amount); 		
		}
	}
	
	bool SetKatamariInSceneControl() {
		if (UIObjects.sceneCon) {
			UIObjects.sceneCon.katamari = this.gameObject;
			return true;
		} else {
			return false;
		}
	}
	
}