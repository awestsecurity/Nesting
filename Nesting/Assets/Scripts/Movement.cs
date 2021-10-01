using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	[Range(0.5f,10f)]
	public float radius = 1;
	[Range(0.1f,1f)]
	public float moveSpeed = 1;
	[Range(0.1f,5f)]
	public float rotateSpeed = 2.25f;
	[Range(0.1f,5.5f)]
	public float height = 1;
	
	private float time;
	private float speed;
	private Transform t;
	private Vector3 startPos;
	
	void Start() {
		t = gameObject.transform;
		startPos = new Vector3 (t.position.x, 1, t.position.z);
		speed = moveSpeed/radius;
	}

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speed;
		float x = Mathf.Cos(time) * radius;
		float z = Mathf.Sin(time) * radius;
		float y = Mathf.Abs( Mathf.Sin(time*4) );
		Vector3 target = startPos + new Vector3 (x,y,z); 
		t.position = target;
		
		/// Oh yes this is bad, but I spent all day getting nowhere and need to move on.
		float rate = Time.deltaTime / speed * -rotateSpeed;
		t.Rotate(Vector3.up * rate);
    }
}
