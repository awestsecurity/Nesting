using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBinding : MonoBehaviour
{
	
	public Dictionary<string, KeyCode> KeyBinds {get; private set;}
	public Dictionary<string, KeyCode> ActionBinds {get; private set;}
	
    // Start is called before the first frame update
    void Start()
    {
        KeyBinds = new Dictionary<string, KeyCode>();
        ActionBinds = new Dictionary<string, KeyCode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
