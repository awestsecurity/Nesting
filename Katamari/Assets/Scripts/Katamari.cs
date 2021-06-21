using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Katamari : MonoBehaviour {

	
	private static float volume; // How big we are / What we can pick up.
	private float trueVolume;
	private static float percentPossible;  // How big we are / What we can pick up.
	
	public static float volumeCheck {
		get { return (volume * percentPossible); } 
		private set { percentPossible = value; }
	}

	public float radius; // Where is the edge to attach things.
	public float temp;
	public Transform camPos;
	
	public Text sizeDisplay;
	public Text birdDisplay;
	public RawImage birdui;
	
	private SphereCollider collide;
	private Thingy lastAssimilated;
	private bool fullscreen = false;
	
	void Start () {
		collide = gameObject.GetComponent<SphereCollider>();
		Texture2D texture = Resources.Load(BirdDetails.birdimg) as Texture2D;
		birdui.texture = texture;
		volume = 1;
		trueVolume = 28888;
		percentPossible = 0.40f; // old number 0.57f
	}

	// Update is called once per frame
	void Update () {
		if (!fullscreen && Input.anyKeyDown) {
			fullscreen = true;
			Screen.fullScreen = true;
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (fullscreen && Input.GetKeyDown(KeyCode.Escape)) {
			Screen.fullScreen = false;
			Cursor.lockState = CursorLockMode.None;
		}
		if (transform.position.y < -15) SceneManager.LoadScene(SceneManager.GetActiveScene().name) ;
		AdjustCameraPosition();
		birdDisplay.text = BirdDetails.birdname;
		sizeDisplay.text = "Mass: "+volume;
		if (volume < trueVolume) volume += Mathf.Round((trueVolume - volume) / 10);
	}
	
	
	void OnTriggerEnter (Collider collision) {
		Thingy thingy = collision.gameObject.GetComponent<Thingy>();
		if (thingy != null && SmallEnoughToGrab(thingy)) {
			thingy.DisableCollider();
			thingy.assimilated = true;
			Transform tt = thingy.gameObject.transform;
			tt.parent = gameObject.transform;
			tt.localPosition = new Vector3 (tt.localPosition.x * 0.8f, tt.localPosition.y * 0.8f, tt.localPosition.z * 0.8f);
			volume += (thingy.GetVolume() / 40f);
			collide.radius += (thingy.volume / 2500000f); //5000000 old number
			lastAssimilated = thingy;
		}
	}
	
	bool SmallEnoughToGrab (Thingy t) {
		return (t.volume < (volume*percentPossible)) ? true : false ;
	}
	
	void AdjustCameraPosition () {
		float i = collide.radius * 0.8f;
		Vector3 v = new Vector3 (0, i, i * -1.9f);
		camPos.localPosition = v;
		camPos.LookAt(this.transform);
	}
	
}
