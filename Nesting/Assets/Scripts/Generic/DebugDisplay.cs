using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//To use elsewhere
//Only thing to change is the dependant variable Settings.showLog.
public class DebugDisplay : MonoBehaviour
{
	public bool editorOnly;
	public float interval = 30;
	//public int targetFPS = 30;
	public Text text;
	
	private int counting = 0;
	private float addingFrameRates = 0;
	
	void Start() {
		//Application.targetFrameRate = targetFPS;
	}

    // Update is called once per frame
    void Update()
    {
		if (Settings.showLog) {
			counting += 1;
			float fps = 1f / Time.unscaledDeltaTime;
			addingFrameRates += fps;
			if (counting == interval) {
				int avg = Mathf.RoundToInt(addingFrameRates / interval);
				float pull = (UIObjects.sceneCon && UIObjects.sceneCon.katamari) ? Katamari.volumeCheck : 0f;
				text.text = $"FPS: {avg} \nPull: {pull}";
				counting = 0;
				addingFrameRates = 0;
			}
		} 
		if (!Settings.showLog) {
			text.text = "";
		}
    }
	
}
