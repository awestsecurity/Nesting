using UnityEngine;
using System.Collections;

public class PlaceRandObjects : MonoBehaviour {

	public GameObject box;
	public int amount;
	private float min = 10;
	private float maxX = 90;
	private float maxZ = 90;

	// Use this for initialization
	void Start () {
	
		for (int i = 0; i < amount; i++) {
			Vector3 pos = new Vector3(Random.Range(min,maxX),0,Random.Range(min,maxZ));
			GameObject o = Instantiate(box, pos, Quaternion.identity) as GameObject;	
			o.transform.Rotate(0,GetRandom()*360,0);
			o.transform.parent = gameObject.transform;
		}
	}
	
	float GetRandom() {
		return Random.value;
	}
	
}
