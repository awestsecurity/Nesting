using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceObjects : MonoBehaviour {
	[System.Serializable]
	public struct LevelObject {
		public int amount;
		public bool ground;
		public GameObject prefab;
	}	

	private float min = 1f;
	private RaycastHit hit;
	
	public List<LevelObject> PlaceSetting = new List<LevelObject>(1);

	// Use this for initialization
	void Start () {
		foreach (LevelObject thing in PlaceSetting) {
			for (int i = 0; i < thing.amount; i++) {
				Vector3 pos = new Vector3(Random.Range(min,BirdDetails.mapx),3f,Random.Range(min,BirdDetails.mapy));
				if (thing.ground) {
					if (Physics.Raycast(pos, -Vector3.up, out hit, 10, 1 << 8)) {
						pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
					} else {
						Debug.LogWarning("Couldn't find ground when placing.");
					}
				}
				GameObject o = Instantiate(thing.prefab, pos, Quaternion.identity) as GameObject;	
				o.transform.Rotate(0,Random.value*360,0);
				o.transform.parent = gameObject.transform;
			}
		}
	}	
	
	void AddNew(){
        PlaceSetting.Add(new LevelObject());
    }

	void Remove(int index){
        PlaceSetting.RemoveAt(index);
    }
}