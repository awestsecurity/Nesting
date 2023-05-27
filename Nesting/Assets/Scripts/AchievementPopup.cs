using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AchievementPopup : MonoBehaviour {

	public Text title;
	public Text description;
	public Sprite graphic;
	
	private Transform t;
	private CanvasGroup image;
	private int secondsVisible = 4;
	private int heightGoal = 140;
	
	public void SetAchievement(Achievement a) {
		title.text = a.mName;
		description.text = a.mDescription;
	}
	
	public void Display(){
		t = gameObject.transform;
		image = gameObject.GetComponent<CanvasGroup>();
		StartCoroutine(MoveHoldFade());
	}
	
	IEnumerator MoveHoldFade() {
		while(t.position.y < heightGoal) {
			Vector3 updatedPos = new Vector3(t.position.x, t.position.y+3, t.position.z);
			t.position = updatedPos;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(secondsVisible);
		float a = image.alpha;
		while(a > 0) {
			a -= 0.05f;
			image.alpha = a;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject);
	}

}