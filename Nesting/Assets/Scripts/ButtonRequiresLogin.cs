using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRequiresLogin : MonoBehaviour{

	bool active = false;
	Button button;

	void Start() {
		button = this.gameObject.GetComponent<Button>();
		button.interactable = false;
	}
	
	void Update() {
		if (!active && BirdDetails.ownedBirds != null) {
			if (BirdDetails.ownedBirds.Length > 0) {
				button.interactable = true;
				active = true;
			}
		}
	}

}