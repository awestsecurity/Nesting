 using System;
 using System.Collections.Generic;
 using UnityEngine;
 
 /// <summary> Unity 2020
 /// Util to help determine current input.
 /// Works for keyboard, mouse, and joystick buttons
 /// A slow operations. Only recommend using on menus(keybinding) or in testing environments
 /// </summary>
 public class InputReader : MonoBehaviour {
	private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));
	 
	 
	// If any key is down it will check every available key code for a match.
	void Update()
	{
		if (Input.anyKeyDown){
			
			//Following line works for just keyboard keys.
			//if (Input.inputString != "") Debug.Log(Input.inputString);

			foreach (KeyCode keyCode in keyCodes)
			{
				if (Input.GetKey(keyCode)) {
					Debug.Log("KeyCode down: " + keyCode);
					break;
				}
			}
		}
	}
	
	void LogJoystick() {
		string[] data = Input.GetJoystickNames();
		for(int i = 0; i < data.Length; i++) { 
			Debug.Log(Input.GetJoystickNames()[i]);
		}
	}
 }