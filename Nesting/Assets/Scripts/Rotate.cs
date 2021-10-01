using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float speed;
	public bool clockwise;
	private float t;
	
    // Update is called once per frame
    void Update()
    {
		t += Time.deltaTime * speed;
        transform.Rotate(Vector3.up, t);
    }
}
