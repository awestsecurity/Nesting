using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdMenu : MonoBehaviour{
	
	public Transform rectangle;
	public GameObject buttonPrefab;
	
	public void MakeBirdMenu() {
		int[] templates = BirdDetails.ownedBirds;
		string[] images = BirdDetails.GetBirdImages(templates);
		for ( int i = 0; i < templates.Length; i++ ) {
			//Add button to menu
			GameObject b = Instantiate(buttonPrefab) as GameObject;
			b.transform.SetParent(rectangle);
			b.name = templates[i].ToString();
			b.transform.localScale = new Vector3(1,1,1);
			//Add bird image to button
			RawImage img = b.GetComponent<RawImage>();
			img.texture = Resources.Load(images[i]+"-o") as Texture2D;
			//Add on click function
			Button button = b.GetComponent<Button>();
			int id = templates[i];
			button.onClick.AddListener(delegate {AssignChosenBird(id); });
		}
	}
	
	public void AssignChosenBird(int templateId) {
		this.gameObject.SetActive(false);
		BirdDetails.SetBird(templateId);
		UIObjects.sceneCon.StartLoad(1);
		
	}
    
}
