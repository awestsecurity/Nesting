using UnityEngine;
using System.Collections;

public class PlaceRandObjects : MonoBehaviour {

	public GameObject box;
	public int amount;
	private float min = 1f;

	// Use this for initialization
	void Start () {
	
		for (int i = 0; i < amount; i++) {
			Vector3 pos = new Vector3(Random.Range(min,BirdDetails.mapx),2,Random.Range(min,BirdDetails.mapy));
			GameObject o = Instantiate(box, pos, Quaternion.identity) as GameObject;	
			o.transform.Rotate(0,GetRandom()*360,0);
			o.transform.parent = gameObject.transform;
		}
	}
	
	float GetRandom() {
		return Random.value;
	}
	
}
