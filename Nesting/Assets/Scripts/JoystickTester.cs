using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// NOT FOR PRODUCTION
// Use to find names of joystick buttons

public class JoystickTester : MonoBehaviour
{
	
    private static readonly KeyCode[] keyCodes = System.Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

    void Update() {
        if (Input.anyKeyDown) {
			foreach (KeyCode keyCode in keyCodes) {
				if (Input.GetKey(keyCode)) {
					Debug.Log("KeyCode down: " + keyCode);
					break;
				}
			}
		}
    }
}
