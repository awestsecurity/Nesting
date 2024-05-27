using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]
public class Thingy : MonoBehaviour {

	public string thingyName;
	public string thingyType;
	
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
	private bool checkThroughGround = true;
	protected bool primed = false;
	protected bool delay = false;
	protected float actualVolume;
	protected bool ai;

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
		if (checkThroughGround) {StartCoroutine(GroundCheck());}
		PrimeObject();
	}
	
	protected virtual void PrimeObject(){
		//used for platform specific code on inheriting classes.
	}
	
	/// <summary>
	/// Delays size checking so falling objects settle before collider is set to trigger.
	/// </summary>
	IEnumerator Delay() {
		yield return new WaitForSeconds(2);
		delay = true;
	}
	
	IEnumerator GroundCheck() {
		yield return new WaitForSeconds(3.5f);
		if (transform.position.y < -8) {
			///// The raycast is never hitting for some reason even though it's the same code from the conform to ground script that works
			//RaycastHit hit;
			//transform.rotation = Quaternion.identity;
			//if (Physics.Raycast(transform.position, Vector3.up, out hit, 60, 1 << 8)) {
			//	Vector3 pos = new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z);
			//	Debug.LogWarning($"{thingyName} fell through and got back.");
			//} else {
			#if (UNITY_EDITOR)
				Debug.LogWarning($"{thingyName} fell through and can't get back.");
			#endif
			//}
		}
		checkThroughGround = false;
	}
	
	protected virtual void Update () {
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
	protected void PrimeForKatamari() {
		primed = true;
		body.isKinematic = true;
		if (!GetComponent<CharacterController>()) {
			collide.isTrigger = true;
		}
	}
	
	/// <summary>
	/// Returns gravity effects until object hits something or is picked up.
	/// </summary>
	protected IEnumerator TemporaryGravity() {
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
