using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(BoxCollider))]
public class Thingy : MonoBehaviour {

	public string thingyName;
	public float volume;
	public bool randomize;
	public float maxScale;
	
	
	Mesh mesh;
	Vector3[] vertices;
	BoxCollider boxcollide;
	MeshCollider meshcollide;
	Rigidbody body;
	
	public bool assimilated = false;
	private Transform katamariParent;
	private bool delay = false;

	// Use this for initialization
	void Start () {
		if (randomize) gameObject.transform.localScale = new Vector3 (Random.Range(0.1f,maxScale),Random.Range(0.1f,maxScale),Random.Range(0.1f,maxScale));
		mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
		if ( volume <= 0 ) volume = GetVolume();
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
		if (delay && !assimilated && !IsTrigger() && volume < (Katamari.volumeCheck * 0.95f)) {
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
	
	float GetVolume() {
		float xMin = 9999;
		float xMax = -9999;
		float yMin = 9999;
		float yMax = -9999;
		float zMin = 9999;
		float zMax = -9999;
		
		foreach (Vector3 v in vertices) {
			Vector3 point = transform.TransformPoint(v);
			xMin = (point.x < xMin)? point.x : xMin ;
			xMax = (point.x > xMax)? point.x : xMax ;
			yMin = (point.y < xMin)? point.y : yMin ;
			yMax = (point.y > xMax)? point.y : yMax ;
			zMin = (point.z < zMin)? point.z : zMin ;
			zMax = (point.z > zMax)? point.z : zMax ;
		}
		
		float a = 1 + Mathf.Abs(xMax - xMin);
		float b = 1 + Mathf.Abs(yMax - yMin);
		float c = 1 + Mathf.Abs(zMax - zMin);

		return ( a*b*c );
	}
	
	// Step toward center of katamari
	void MoveToParent () {
		transform.position = Vector3.MoveTowards(transform.position, katamariParent.position, Time.deltaTime);
	}
	
}
