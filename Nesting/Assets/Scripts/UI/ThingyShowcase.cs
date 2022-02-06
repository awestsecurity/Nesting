using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingyShowcase : MonoBehaviour
{

	[Range(0.1f,5f)]
	public float speed = 1;
	public GameObject test;
	
	private Transform t;
	
    // Start is called before the first frame update
    void Start()
    {
        t = gameObject.transform;
		SwitchThing(test);
    }

    // Update is called once per frame
    void Update()
    {
        t.Rotate(Vector3.up * Time.deltaTime * speed);
    }
	
	public void SwitchThing(GameObject newThing) {
		GameObject o = Instantiate(newThing, t);
		o.transform.position = Vector3.zero;
	}
}
