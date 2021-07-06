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
	
	public float typeDelay = 0.05f;
	public float messageEndDelay = 0.75f;
	
	void Awake() {
		panel = this.gameObject;
		text = panel.transform.GetChild(0).GetComponent<Text>();
		panel.SetActive(false);
	}
	
	void Update() {
		//Debug.Log($"complete? {complete}");
		if (complete && Input.anyKey) {
			Disable();
		}
	}

    public bool LaunchMessagePanel() {
		if (!complete) { return false; }
		panel.SetActive(true);
        StartCoroutine(TypeText());
		return true;
    }
	
	IEnumerator TypeText() {
		complete = false;
		ClearMessage();
		string[] tmpMessages = messages.ToArray();
		messages.Clear();
		// Do for each message in the queue
		for (int i = 0; i < tmpMessages.Length; i++) {
			// Do for each letter in the message
			foreach (char letter in tmpMessages[i]) {
				text.text += letter;
				if (letter.ToString() == ".") {
					yield return new WaitForSeconds(typeDelay*2);
				}
				yield return new WaitForSeconds(typeDelay);
			}
			yield return new WaitForSeconds(messageEndDelay);
			while (!Input.anyKey) {
				yield return new WaitForEndOfFrame();
			}
			ClearMessage();
		}
		yield return new WaitForSeconds(typeDelay*2);
		complete = true;
	}
	
	public void AddMessage(string s = "you forgot the message. :/") {
		messages.Add(s);
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
