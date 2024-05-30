using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingyShowcase : MonoBehaviour
{

	[Range(1f,100f)]
	public float speed = 10;
	public float timeDisplayed = 5;
	public GameObject baseObject;
	private Transform baseObjectTrans;
	private MeshFilter baseObjectMesh;
	private MeshRenderer baseObjectRender;
	private Vector3 scaleTo;
	
    // Start is called before the first frame update
    void Start()
    {
		//baseObject = this.gameObject;
        baseObjectTrans = baseObject.transform;
		baseObjectMesh = baseObject.GetComponent<MeshFilter>();
		baseObjectRender = baseObject.GetComponent<MeshRenderer>();
		baseObjectRender.enabled = false;
		
		Bounds tempBounds = baseObjectMesh.sharedMesh.bounds;
		scaleTo = tempBounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        baseObjectTrans.Rotate(Vector3.up * Time.deltaTime * speed);
    }
	
	public void SwitchThing(GameObject newThing) {
		Mesh newMesh = newThing.GetComponent<MeshFilter>().sharedMesh;
		baseObjectMesh.sharedMesh = newMesh;
		//MeshRenderer newRender = newThing.GetComponent<MeshRenderer>();
		//for (int i = 0; i <)
		baseObjectRender.materials = newThing.GetComponent<MeshRenderer>().materials;
		ScaleToBounds();
		StartCoroutine(Display());
	}
	
	private IEnumerator Display() {
		baseObjectRender.enabled = true;
		yield return new WaitForSeconds(timeDisplayed);
		baseObjectRender.enabled = false;
	}
	
	private void ScaleToBounds() {
		Bounds bounds = baseObjectMesh.sharedMesh.bounds;
		Vector3 size = bounds.size;
		float xdif = scaleTo.x / size.x;
		float ydif = scaleTo.y / size.y;
		float zdif = scaleTo.z / size.z;
		float min;
		if (xdif < ydif) {
			min = xdif;
		} else {
			min = ydif;
		}
		if (zdif < min) {
			min = zdif;
		}
		baseObjectTrans.localScale = baseObjectTrans.localScale * min;
	}
}
