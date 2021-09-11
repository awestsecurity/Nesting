using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(BoxCollider))]
public class Thingy : MonoBehaviour {

	public string thingyName;
	
	public float volume = 300; //cm3 can use blender to get measurement
	[Range(0.5f, 10.0f)]
	public float modifier = 1f; //Multiply the volume added
	public bool randomize;
	public bool rotate;
	public bool keepRatio;
	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	
	
	Mesh mesh;
	Vector3[] vertices;
	BoxCollider boxcollide;
	MeshCollider meshcollide;
	Rigidbody body;
	Transform t;
	
	public bool assimilated {get;set;}
	private bool delay = false;
	private float actualVolume;

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
		mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
		actualVolume = GetVolume();
		boxcollide = gameObject.GetComponent<BoxCollider>();
		meshcollide = gameObject.GetComponent<MeshCollider>();
		body = gameObject.GetComponent<Rigidbody>();
		StartCoroutine(Delay());
	}
	
	IEnumerator Delay() {
		yield return new WaitForSeconds(2);
		delay = true;
	}
	
	void Update () {
		if (delay && !assimilated && !IsTrigger() && actualVolume < (Katamari.volumeCheck)) {
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
		return volume * ((t.localScale.x + t.localScale.y + t.localScale.z) / 3.0f);
	}
	
	// Step toward center of katamari
	void MoveToParent () {
		t.position = Vector3.MoveTowards(t.position, this.transform.parent.position, Time.deltaTime);
	}
	
}
