using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSingleton : GenericSingleton<HUDSingleton>
{
	public GameObject title;
	public GameObject pause;
	
	//Can/Should only be one
	void Reset(){
		title.SetActive(true);
		pause.SetActive(false);
	}
}
