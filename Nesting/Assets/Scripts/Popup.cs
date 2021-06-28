using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
 
	private GameObject panel;
	private Text text;
	private List<string> messages = new List<string>();
	private bool complete = true;
	
	void Start() {
		panel = this.gameObject;
		text = panel.transform.GetChild(0).GetComponent<Text>();
	}
	
	void Update() {
		if (complete && Input.anyKey) {
			ClearMessage();
			Disable();
		}
	}

    public void LaunchMessagePanel()
    {
		if (!complete) { return; }
		panel.SetActive(true);
        StartCoroutine(TypeText());
    }
	
	IEnumerator TypeText(float delay = 0.05f) {
		complete = false;
		text.text = "";
		for (int i = 0; i < messages.Count; i++) {
			foreach (char letter in messages[i]) {
				text.text += letter;
				if (letter.ToString() == ".") {
					yield return new WaitForSeconds(delay*2);
				}
				yield return new WaitForSeconds(delay);
			}
			yield return new WaitForSeconds(delay*3);
			while (!Input.anyKey) {
				yield return new WaitForEndOfFrame();
			}
			ClearMessage();
		}
		yield return new WaitForSeconds(delay*2);
		complete = true;
		messages.Clear();
	}
	
	public void AddMessage(string s = "you forgot the message. :/") {
		if (!complete) {
			Debug.LogError("Cannot add message while messages in progress. ");
		} else {
			messages.Add(s);
		}
	}
	
	void ClearMessage() {
		text.text = "";
	}
	
	public void Reset() {
		StopCoroutine(TypeText());
		complete = true;
		messages.Clear();
	}
	
	void Disable() {
		panel.SetActive(false);
	}
}
