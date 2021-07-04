using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CamFollow : MonoBehaviour
{
	public Transform target;
	public float spacer = 5f;
	public float height = 1.5f;
	public float speed = 1f;
	
	private float buffer = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 target_vect;
		Vector3 movePosition = transform.position;
		Vector3 camPos = new Vector3(transform.position.x, 0, transform.position.z);
		Vector3 ballPos = new Vector3(target.position.x, 0, target.position.z);
		float distance = Vector3.Distance(camPos,ballPos);
		if (distance > (spacer+buffer) || distance < (-spacer-buffer)) {
			target_vect = new Vector3(target.position.x, target.position.y + height, target.position.z);
			movePosition = Vector3.Lerp(transform.position, target_vect, Time.deltaTime*speed);
			this.transform.position = movePosition;
			Debug.Log($"Distance from cam to kat: {distance}");
		} else if ( distance < spacer && distance > -spacer ) {
			Vector3 dir = CalculateNormal(transform.position, target.position, Vector3.up);
			dir.y = 0;
			target_vect = transform.position + (dir*(spacer));
			movePosition = Vector3.Lerp(transform.position, target_vect, Time.deltaTime*speed*3);
			this.transform.position = movePosition;
		}
		//HandleRotation();
    }
	
	private void HandleRotation() {
		float x = CrossPlatformInputManager.GetAxis("Mouse X");
		if (Mathf.Abs(x) > buffer) {
			//transform.RotateAround (target.position,new Vector3(0.0f,1.0f,0.0f),20 * Time.deltaTime * x);
		}
	}
	
	private Vector3 CalculateNormal(Vector3 startingPoint, Vector3 endingPoint, Vector3 point3){
		Vector3 side1 = endingPoint - startingPoint;
		Vector3 side2 = point3 - startingPoint;
		Vector3 ret = Vector3.Cross(side1, side2);
		// return the normalized direction
		return ret.normalized;
	}
	
}
