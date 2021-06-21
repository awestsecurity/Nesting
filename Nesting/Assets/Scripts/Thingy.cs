using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(BoxCollider))]
public class Thingy : MonoBehaviour {

	public string thingyName;
	public float volume; //cm3 can use blender to get measurement
	public bool randomize;
	public bool rotate;
	public bool keepRatio;
	public float minScale = 0.1f;
	public float maxScale;
	
	
	Mesh mesh;
	Vector3[] vertices;
	BoxCollider boxcollide;
	MeshCollider meshcollide;
	Rigidbody body;
	
	public bool assimilated {get;set;}
	private Transform katamariParent;
	private bool delay = false;
	private float actualVolume;

	// Use this for initialization
	void Start () {
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
			gameObject.transform.localScale = new Vector3 (x,y,z);
		}
		if (rotate){
			transform.rotation = Random.rotation;
		}
		mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
		actualVolume = GetVolume();
		boxcollide = gameObject.GetComponent<BoxCollider>();
		meshcollide = gameObject.GetComponent<MeshCollider>();
		body = gameObject.GetComponent<Rigidbody>();
		StartCoroutine(Delay());
	}
	
	IEnumerator Delay() {
		yield return new WaitForSeconds(3);
		delay = true;
	}
	
	void Update () {
		if (delay && !assimilated && !IsTrigger() && actualVolume < (Katamari.volumeCheck * 0.95f)) {
			if (boxcollide) boxcollide.isTrigger = true;
			if (meshcollide) meshcollide.isTrigger = true;
			body.isKinematic = true;
		}
	}
	
	public void DisableCollider() {
		if (boxcollide != null) {boxcollide.enabled = false;}
		else {meshcollide.isTrigger = true;}
		body.isKinematic = true;
	}
	
	bool IsTrigger() {
		if (boxcollide != null) {return boxcollide.isTrigger;}
		else {return meshcollide.isTrigger;}		
	}
	
	public float GetVolume() {
		return volume * (transform.localScale.x * transform.localScale.y * transform.localScale.z / 3f);
	}
	
	// Step toward center of katamari
	void MoveToParent () {
		transform.position = Vector3.MoveTowards(transform.position, katamariParent.position, Time.deltaTime);
	}
	
}
