using UnityEngine;
using System.Collections;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// Original script from https://gist.github.com/mminer/1331271
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Wander : MonoBehaviour
{
	public float speed = 1;
	public float directionChangeInterval = 3;
	public float maxHeadingChange = 30;

	public float oddsOfResting = 0.1f; // 0 never, 1 always
	public float restMaxSeconds = 8f;
	public float restMinSeconds = 0f;

	private CharacterController controller;
	private float heading;
	private Vector3 targetRotation;
	private bool resting;

	void Awake ()
	{
		controller = GetComponent<CharacterController>();

		// Set random initial rotation
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);

		StartCoroutine(NewHeading());
	}

	void Update ()
	{
		if (!resting) {
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
			var forward = transform.TransformDirection(Vector3.forward);
			controller.SimpleMove(forward * speed);
		}
	}

	/// <summary>
	/// Repeatedly calculates a new direction to move towards.
	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
	/// </summary>
	IEnumerator NewHeading ()
	{
		while (true) {
			NewHeadingRoutine();
			yield return new WaitForSeconds(directionChangeInterval);
			if (!resting && Random.value < oddsOfResting) StartCoroutine(Rest());
		}
	}

	/// <summary>
	/// Stops movement for a random amount of time.
	/// </summary>
	IEnumerator Rest() {
		resting = true;
		yield return new WaitForSeconds(Random.Range(restMinSeconds,restMaxSeconds));
		resting = false;
	}

	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeadingRoutine ()
	{
		var floor = transform.eulerAngles.y - maxHeadingChange;
		var ceil  = transform.eulerAngles.y + maxHeadingChange;
		heading = Random.Range(floor, ceil);
		targetRotation = new Vector3(0, heading, 0);
	}
}