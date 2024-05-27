using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConformToGround : MonoBehaviour
{
	public float heightOffset = 0;
	RaycastHit hit;
	
    // Start is called before the first frame update
    void Start()
    {
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 25, 1 << 8)) {
			this.transform.position = new Vector3(hit.point.x, hit.point.y+heightOffset, hit.point.z);
		} else {
			StartCoroutine(Place());
		}
	}
	
	IEnumerator Place() {
		yield return new WaitForSeconds(0.1f);
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 25, 1 << 8)) {
			this.transform.position = new Vector3(hit.point.x, hit.point.y+heightOffset, hit.point.z);
		} else {
			Destroy(gameObject);
		}
	}

}
