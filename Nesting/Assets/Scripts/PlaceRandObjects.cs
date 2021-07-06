using UnityEngine;
using System.Collections;

public class PlaceRandObjects : MonoBehaviour {

	public GameObject box;
	public int amount;
	public bool ground = false; //Should we raycast and conform to ground surface.
	private float min = 1f;
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
	
		for (int i = 0; i < amount; i++) {
			Vector3 pos = new Vector3(Random.Range(min,BirdDetails.mapx),3f,Random.Range(min,BirdDetails.mapy));
			if (ground) {
				if (Physics.Raycast(pos, -Vector3.up, out hit, 10, 1 << 8)) {
					pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				} else {
					Debug.LogWarning("Couldn't find ground when placing.");
				}
			}
			GameObject o = Instantiate(box, pos, Quaternion.identity) as GameObject;	
			o.transform.Rotate(0,GetRandom()*360,0);
			o.transform.parent = gameObject.transform;
		}
	}
	
	float GetRandom() {
		return Random.value;
	}
	
}
