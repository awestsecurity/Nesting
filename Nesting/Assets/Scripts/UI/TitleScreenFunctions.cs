using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenFunctions : MonoBehaviour
{

	public GameObject titleButtons;
	
	public void ToggleMenu(GameObject menu) {
		bool b = menu.activeSelf;
		menu.SetActive(!b);
		titleButtons.SetActive(b);		
	}
	
}
