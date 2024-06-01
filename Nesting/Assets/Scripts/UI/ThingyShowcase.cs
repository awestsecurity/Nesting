using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingyShowcase : MonoBehaviour
{

	[Range(1f,100f)]
	public float speed = 10;
	public float timeDisplayed = 5;
	public GameObject baseObject;
	public GameObject backdrop;
	private Transform baseObjectTrans;
	private MeshFilter baseObjectMesh;
	private MeshRenderer baseObjectRender;
	private Vector3 scaleTo;
	private Vector3 originalScale;
	
    // Start is called before the first frame update
    void Start()
    {
		//baseObject = this.gameObject;
        baseObjectTrans = baseObject.transform;
		baseObjectMesh = baseObject.GetComponent<MeshFilter>();
		baseObjectRender = baseObject.GetComponent<MeshRenderer>();
		baseObjectRender.enabled = false;
		backdrop.SetActive(false);
		
		Bounds tempBounds = baseObjectMesh.mesh.bounds;
		scaleTo = tempBounds.size;
		originalScale = baseObjectTrans.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        baseObjectTrans.Rotate(Vector3.up * Time.deltaTime * speed);
    }
	
	public void SwitchThing(GameObject newThing) {
		Mesh newMesh = newThing.GetComponent<MeshFilter>().sharedMesh;
		baseObjectMesh.sharedMesh = newMesh;
		baseObjectRender.materials = newThing.GetComponent<MeshRenderer>().materials;
		ScaleToBounds();
		StartCoroutine(Display());
	}
	
	private IEnumerator Display() {
		baseObjectRender.enabled = true;
		backdrop.SetActive(true);
		yield return new WaitForSeconds(timeDisplayed);
		baseObjectRender.enabled = false;
		backdrop.SetActive(false);
	}
	
	private void ScaleToBounds() {
		Bounds bounds = baseObjectMesh.sharedMesh.bounds;
		Vector3 size = bounds.size;
		float xdif = scaleTo.x / size.x;
		float ydif = scaleTo.y / size.y;
		float zdif = scaleTo.z / size.z;
		//Debug.Log($"Object size: {size}");
		//Debug.Log($"Scale to: {scaleTo.x}, {scaleTo.y}, {scaleTo.z}");
		//Debug.Log($"results: {xdif}, {ydif}, {zdif}");

		float min;
		if (xdif < ydif) {
			min = xdif;
		} else {
			min = ydif;
		}
		if (zdif < min) {
			min = zdif;
		}
		baseObjectTrans.localScale = originalScale * min;
	}
}
