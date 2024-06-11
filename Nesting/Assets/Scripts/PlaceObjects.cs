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
	
	public bool structuralObjects = false;
	public List<LevelObject> PlaceSetting = new List<LevelObject>(1);
	
	private int margin = 1;

	// Use this for initialization
	void Start () {
		if (structuralObjects) {
			margin = 5;
			InstantiateEverything();
		} else {
			StartCoroutine(SecondRound());
		}

	}	
	
	void InstantiateEverything() {
		int xmin = -BirdDetails.hexBuffer + margin;
		int xmax = BirdDetails.mapx * 2 + BirdDetails.hexBuffer -margin;
		int ymax = BirdDetails.mapy * 2 - margin;
		foreach (LevelObject thing in PlaceSetting) {
			for (int i = 0; i < thing.amount; i++) {
				Vector3 pos = new Vector3(Random.Range(xmin, xmax),2f,Random.Range(min,ymax));
				if (thing.ground) {
					RaycastHit hit;
					if (Physics.Raycast(pos, -Vector3.up, out hit, 10, 1 << 8)) {
						pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
					} else {
						Debug.LogWarning($"Couldn't find ground when placing. {pos}");
					}
				}
				GameObject o = Instantiate(thing.prefab, pos, Quaternion.identity) as GameObject;	
				o.transform.Rotate(0,Random.value*360,0);
				o.transform.parent = gameObject.transform;
			}
		}
	}
	
	//Used to make sure giant things get placed first.
	IEnumerator SecondRound() {
		yield return new WaitForEndOfFrame();
		InstantiateEverything();
	}
	
	void AddNew(){
        PlaceSetting.Add(new LevelObject());
    }

	void Remove(int index){
        PlaceSetting.RemoveAt(index);
    }
}