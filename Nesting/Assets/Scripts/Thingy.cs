using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]
public class Thingy : MonoBehaviour {

	public string thingyName;
	
	public float volume = 300; //cm3 can use blender to get measurement
	[Range(0.5f, 10.0f)]
	public float modifier = 1f; //Multiply the volume added
	public bool randomize;
	public bool rotate;
	public bool keepRatio;
	public bool testForGravity = false; //Will the object be floating at any point and need to enable gravity
	public float minScale = 0.8f;
	public float maxScale = 1.2f;

	Collider collide;
	Rigidbody body;
	Transform t;
	
	public bool assimilated {get;set;}
	private bool primed = false;
	private bool delay = false;
	private float actualVolume;
	private bool ai;

	// Use this for initialization
	void Start () {
		t = transform;
		if (randomize) {
			float x, y, z;
			if (keepRatio) { 
				x = y = z = Random.Range(minScale,maxScale); 
			}
			else { 
				x = Random.Range(minScale,maxScale); 
				y = Random.Range(minScale,maxScale);
				z = Random.Range(minScale,maxScale);
			}
			t.localScale = new Vector3 (x,y,z);
		}
		if (rotate){
			t.rotation = Random.rotation;
		}
		actualVolume = GetVolume();
		collide = gameObject.GetComponent<Collider>(); //box or mesh colliders used
		body = gameObject.GetComponent<Rigidbody>();
		if (gameObject.GetComponent<CharacterController>() != null) ai = true;
		StartCoroutine(Delay());
	}
	
	/// <summary>
	/// Delays size checking so falling objects settle before collider is set to trigger.
	/// </summary>
	IEnumerator Delay() {
		yield return new WaitForSeconds(2);
		delay = true;
	}
	
	void Update () {
		if (delay && !primed && actualVolume < (Katamari.volumeCheck)) {
			PrimeForKatamari();
		} else if (delay && !assimilated && testForGravity) {
			
			if (!Physics.Raycast(transform.position, -Vector3.up, 0.1f)) {
				StartCoroutine(TemporaryGravity());
				testForGravity = false;
			}
		}
		if (ai && assimilated) {
			ai = false;
			Destroy(gameObject.GetComponent<Wander>()); //Remove Wander first to avoid error on controller dependence
			Destroy(gameObject.GetComponent<CharacterController>());
		}
	}
	
	/// <summary>
	/// Sets neccessary settings so object will be recognized and picked up by birdamari.
	/// </summary>
	private void PrimeForKatamari() {
		primed = true;
		body.isKinematic = true;
		if (!GetComponent<CharacterController>()) {
			collide.isTrigger = true;
		}
	}
	
	/// <summary>
	/// Returns gravity effects until object hits something or is picked up.
	/// </summary>
	private IEnumerator TemporaryGravity() {
		yield return new WaitForSeconds(0.5f);
		delay = false;
		bool k = body.isKinematic;
		bool g = body.useGravity;
		bool t = collide.isTrigger;
		body.isKinematic = false;
		body.useGravity = true;
		collide.isTrigger = false;
		while (!Physics.Raycast(transform.position, -Vector3.up, 0.1f) && !assimilated) {
			yield return new WaitForEndOfFrame();
		}
		body.isKinematic = k;
		body.useGravity = g;
		collide.isTrigger = t;
	}
	
	public void DisableCollider() {
		collide.enabled = false;
		body.isKinematic = true;
	}
	
	bool IsTrigger() {
		return collide.isTrigger;	
	}
	
	public float GetVolume() {
		return volume * ((t.localScale.x + t.localScale.y + t.localScale.z) / 3.0f);
	}
	
	/// <summary>
	/// Step towards birdamari center.
	/// </summary>
	void MoveToParent () {
		t.position = Vector3.MoveTowards(t.position, this.transform.parent.position, Time.deltaTime);
	}
	
}
