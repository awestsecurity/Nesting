﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Ball;


[RequireComponent(typeof(AudioSource))]
public class Katamari : MonoBehaviour {

	public GameObject HUDPrefab;
	private static float displayVolume; // What's Showing in the textbox.
	private float trueVolume; // Everything added and used as the final score
	private static float percentPossible = 0.33f;  // How big we are / What we can pick up.
	private static float startingMass = 155;
	private Vector3 tuckInObject = new Vector3(0.85f,0.85f,0.85f);
	
	public static float volumeCheck; // The thingy size we can pick up
	public static Transform trans;
	public string mostCommonChild { get {return GetTopThings()[0]; } private set{} }
	private string[] childrenByQuantity;

	public float radius; // Where is the edge to attach things.
	private float maxRadius = 6.666f;
	private float maxMovePower = 20f;
	private float maxCamDistance = 81f;
	public GameObject kataCam;
	private Transform camPos;
	private CamFollow camFollow;
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

	private int flowersCollected = 0;
	private int snowmenSmashed = 0;
	
	private int cheatVolume = 9999999;
	private float prevvolume;
	private bool leveltesting = false; //avoid console errors when we skip UI creation
	
	private AudioSource speaker;
	public AudioClip waterSplash;
	public AudioClip plodNoise;
	public AudioClip[] basePickupSounds;
	
	void Start () {
		maxChildren = Settings.collectedMax;
		trans = transform;
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
		camPos = kataCam.GetComponent<Transform>();
		camFollow = kataCam.GetComponent<CamFollow>();
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
		if (Debug.isDebugBuild && Input.GetKeyDown("w")) {
			trueVolume = trueVolume * 1.5f;
		}
		
		//Fallen through ground somehow
		if (transform.position.y < -15) {
			//SceneManager.LoadScene(SceneManager.GetActiveScene().name) ;
			this.transform.position = new Vector3(68, 15, 74);
		}
		
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
			PlaySFX(plodNoise, false, rbody.velocity.magnitude/17f);
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
	//It doesn't really make sense that it's here
	void ConnectUI() {
		hud = GameObject.Find("HUD");
		if (hud == null) {
			leveltesting = true;
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
			PlaySFX(basePickupSounds[Random.Range(0,basePickupSounds.Length)], false);
			thingy.DisableCollider();
			thingy.assimilated = true;
			Transform tt = thingy.gameObject.transform;
			tt.parent = gameObject.transform;
			tt.localPosition = Vector3.Scale(tt.localPosition, tuckInObject);
			float volume = thingy.GetVolume();
			AddVolumeToKatamari(volume, thingy.modifier);
			lastAssimilated = thingy;
			collected.Add(thingy.thingyName);
			if (!leveltesting) {
				UpdateCollectedStats(thingy.thingyType);
				UIObjects.achievements.UpdateMetric("LargestThing", (int)volume);
			}
			RemoveOldChildrenAfterMax();
			if (thingy is ThingyGlow) {
				ThingyGlow l = thingy as ThingyGlow;
				StartCoroutine(AbsorbLight(l));
			}
			Destroy(thingy.GetComponent<Movement>());
			if (thingy.keyItem) {
				kataCam.GetComponent<ThingyShowcase>().SwitchThing(thingy.gameObject);
			}
		} else if (collision.gameObject.name == "Water"){
				PlaySFX(waterSplash, true);
		}
	}
	
	//For spooky level. Katamari will accumilate light from glowing things.
	IEnumerator AbsorbLight(ThingyGlow glowThingy){
		if (!glow) {
			glow = gameObject.AddComponent<Light>() as Light;
			glow.intensity = 0.1f;
			glow.range = 0.5f;
			glow.color = Color.green / 2f;
			glow.type = LightType.Point;
		}
		glowThingy.StartLightFade();
		glow.range = glow.range + 0.075f;
		float targetIntesity = Mathf.Clamp(glow.intensity += 0.015f, 0.1f, 4f);
		while (glow.intensity < targetIntesity) {
			yield return new WaitForSeconds(0.1f);
			glow.intensity += 0.005f;
		}
	}
	
	void PlaySFX(AudioClip sfx, bool playOver, float loudness = 0.666f) {
		if (Settings.sfxOn) {
			if(playOver || !speaker.isPlaying) {
				speaker.Stop();
				speaker.PlayOneShot(sfx, loudness);
			}
		}
	}
	
	//Handle the expansion given the thingy volume and modifier
	void AddVolumeToKatamari(float vol, float mod = 1) {
		trueVolume += (vol*mod / 8f);
		float adjust = vol / 35000f;  //5000000 old number
		if (collide.radius < (maxRadius/2f) ) {
			collide.radius += adjust * 0.75f;
		} else if (collide.radius < maxRadius) {
			collide.radius += adjust * 0.45f;
		} else {
			collide.radius += adjust * 0.1f;
		}
		if (ballController.m_MovePower < maxMovePower) {
			ballController.m_MovePower += adjust * 2;
		}
		if (camFollow.spacer < maxCamDistance) {
			camFollow.spacer += adjust * 2.15f;
		}
	}
	
	//Remove the a number of the smallest attached objects when we have too many
	void RemoveOldChildrenAfterMax(int removeBuffer = 10) {
		int max = (Settings.collectedMax+2)*75;
		if (this.transform.childCount > max) {
			//sort by size
			List<Thingy> stuff = transform.GetComponentsInChildren<Thingy>().ToList();
			stuff.Sort((x, y) => x.volume.CompareTo(y.volume));
			for (int i = 0; i < removeBuffer; i++) {
				//Debug.Log($"Removed {stuff[i].name}");
				stuff[i].transform.parent = null;
				stuff[i].transform.localPosition = new Vector3(999,999,999);
				//Destroy(this.transform.GetChild(1).gameObject);
			}
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
	
	//Return list of thing types by frequency
	public string[] GetTopThings(int ranksReturned = 3) {
		Dictionary<string, int> dict = ChildFrequencyDict();
		var sorted = (from entry in dict orderby entry.Value descending select entry.Key).Take(ranksReturned);
		List<string> rank = sorted.ToList();
		if (rank.Count < ranksReturned) {
			for (int i = rank.Count; i < ranksReturned; i++){
				rank.Add("Empty Space");
			}
		}
		return rank.ToArray();
	}
	
	//Add to collection stats for achievements
	private void UpdateCollectedStats(string thingType, int amount = 1) {
		if (thingType == "Mushroom") {
			UIObjects.achievements.UpdateMetric("MushroomsCollected", amount);
		} else if (thingType == "Egg") {
			UIObjects.achievements.UpdateMetric("EggFound", 1);
		} else if (thingType == "Flower") { 
			flowersCollected += 1;
			UIObjects.achievements.UpdateMetric("FlowersIn1Run", flowersCollected); 
		} else if (thingType == "SnowHead") {
			snowmenSmashed += 1;
			UIObjects.achievements.UpdateMetric("SnowmenIn1Run", snowmenSmashed); 		
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
	
	public void Cheat() {
		BirdDetails.cheat = true;
		percentPossible = 1f;
		AddVolumeToKatamari(10000);
	}
	
}