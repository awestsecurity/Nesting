using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Ball;

[RequireComponent(typeof(AudioSource))]
public class Katamari : MonoBehaviour {

	public GameObject HUDPrefab;
	private static float displayVolume; // How big we are / What we can pick up.
	private float trueVolume; // Display Volume
	private static float percentPossible = 0.57f;  // How big we are / What we can pick up.
	
	public static float volumeCheck;
	public string mostCommonChild { get {return GetMostCommonChild(); } private set{} }
	private string[] childrenByQuantity;

	public float radius; // Where is the edge to attach things.
	public Transform camPos;
	public CamFollow camFollow;
	public int maxChildren = 222;
	
	private GameObject hud;
	private Text sizeDisplay;
	private Text birdDisplay;
	
	private SphereCollider collide;
	private Thingy lastAssimilated;
	private List<string> collected = new List<string>();
	private Ball ballController;
	private Rigidbody rbody;
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
		volumeCheck = trueVolume * percentPossible;
		collide = gameObject.GetComponent<SphereCollider>();
		ballController = gameObject.GetComponent<Ball>();
		speaker = gameObject.GetComponent<AudioSource>();
		rbody = gameObject.GetComponent<Rigidbody>();
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
		//	fullscreen = true;
		//	Screen.fullScreen = true;
		//	Cursor.lockState = CursorLockMode.Locked;
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Screen.fullScreen = false;
			Cursor.lockState = CursorLockMode.None;
		}
		//Fallen through ground somehow
		//if (transform.position.y < -15) SceneManager.LoadScene(SceneManager.GetActiveScene().name) ;
		
		//AdjustCameraPosition();
		birdDisplay.text = BirdDetails.birdname;
		sizeDisplay.text = "Mass: "+displayVolume;
		if (displayVolume < trueVolume) displayVolume += Mathf.Round((trueVolume - displayVolume) / 30f);
		if (trueVolume > BirdDetails.score && trueVolume < (BirdDetails.score*1.5f)) {
			BirdDetails.score = trueVolume;
		}
		
		//Debug.Log (rbody.velocity.magnitude);
		if (!speaker.isPlaying && ballController.grounded) {
			PlaySFX(plodNoise, rbody.velocity.magnitude/17f);
		}
		
		//Cheat Detection
		if (trueVolume > prevvolume * 1.5f) BirdDetails.cheat = true;
		prevvolume = trueVolume;
		if ( trueVolume >= cheatVolume ) BirdDetails.cheat = true;
	}
	
	//Hold off on the gravity to allow easy settling
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
			tt.localPosition = new Vector3 (tt.localPosition.x * 0.85f, tt.localPosition.y * 0.85f, tt.localPosition.z * 0.85f);
			float volume = thingy.GetVolume();
			AddVolumeToKatamari(volume, thingy.modifier);
			lastAssimilated = thingy;
			collected.Add(thingy.thingyName);
			RemoveOldChildrenAfterMax();
			Destroy(thingy.GetComponent<Movement>());
		} else if (collision.gameObject.name == "Water"){
			PlaySFX(waterSplash);
		}
	}
	
	void PlaySFX(AudioClip sfx, float loudness = 0.666f) {
		if (Settings.sfxOn) {
			speaker.PlayOneShot(sfx, loudness);
		}
	}
	
	//Handle the expansion given the thingy volume and modifier
	void AddVolumeToKatamari(float v, float m = 1) {
		trueVolume += (v*m / 40f);
		float adjust = v / 350000f;  //5000000 old number
		collide.radius += adjust *0.75f;
		ballController.m_MovePower += adjust*2;
		camFollow.spacer += adjust*2;
	}
	
	//Remove the oldest attached object when we have too many
	void RemoveOldChildrenAfterMax() {
		if (this.transform.childCount > maxChildren) {
			Destroy(this.transform.GetChild(1).gameObject);
		}		
	}
	
	
	bool SmallEnoughToGrab (Thingy t) {
		return (t.GetVolume() < (trueVolume * percentPossible)) ? true : false ;
	}
	
	void AdjustCameraPosition () {
		float i = collide.radius * 0.8f;
		Vector3 v = new Vector3 (0, i, i * -1.9f);
		camPos.localPosition = v;
		camPos.LookAt(this.transform);
	}
	
	string GetMostCommonChild() {
		Dictionary<string, int> dict = ChildFrequencyDict();
		string mostCommonValue = "whoops";
		int highestCount = 0;
		foreach (KeyValuePair<string, int> pair in dict) {
		   if (pair.Value > highestCount) {
			   mostCommonValue = pair.Key;
			   highestCount = pair.Value;
		   }
		}
		return mostCommonValue;
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
		return rank;
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