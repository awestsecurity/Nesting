using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdMenu : MonoBehaviour{
	
	public Transform rectangle;
	public GameObject buttonPrefab;
	public Image announcementBird;
//	private Sprite[] birdAtlas;
//	private Sprite[] birdOAtlas;
	
	public void MakeBirdMenu() {
		int[] templates = BirdDetails.ownedBirds;
		string[] images = BirdDetails.GetBirdJSONStat(templates, "name");
		string[] statuses = BirdDetails.GetBirdJSONStat(templates, "status");
		string[] flavor = BirdDetails.GetBirdJSONStat(templates, "text");
		float[] speeds = BirdDetails.GetBirdJSONStatAsFloat(templates, "speed");
		LoadSprites();
		for ( int i = 0; i < templates.Length; i++ ) {
			//Add button to menu
			GameObject b = Instantiate(buttonPrefab) as GameObject;
			BirdStats stats = b.GetComponent<BirdStats>();
			b.transform.SetParent(rectangle);
			b.name = images[i];
			stats.name = b.name;
			stats.templateID = templates[i];
			stats.status = statuses[i];
			stats.flavortext = flavor[i];
			stats.speed = speeds[i];
			b.transform.localScale = new Vector3(1,1,1);
			//Add bird image to button
			Image img = b.GetComponent<Image>();
			img.sprite = GetBirdSprite(images[i], true);
			//Add on click function
			Button button = b.GetComponent<Button>();
			int id = templates[i];
			string name = images[i];
			button.onClick.AddListener(delegate {AssignChosenBird(id,name); });
		}
	}
	
	private void LoadSprites() {
		//Sprite[] birdOAtlas = Resources.LoadAll<Sprite>("BirdSpriteSheetO");
		//Sprite[] birdAtlas = Resources.LoadAll<Sprite>("BirdSpriteSheet");
		//Keep only sprites from owned templates
		//unload others
	}
	
	public Sprite GetBirdSprite(string birdname, bool outline) {
		if (outline) {
			foreach (Sprite sp in Resources.LoadAll<Sprite>("BirdSpriteSheetO")) {
				if (sp.name == birdname) {
					return sp;
				} 
			}
		} else {
			foreach (Sprite sp in Resources.LoadAll<Sprite>("BirdSpriteSheet")) {
				if (sp.name == birdname) {
					return sp;
				} 
			}
		}			

		Debug.LogWarning("No Sprite Named: "+birdname);
		return null;
	}
	
	public void AssignChosenBird(int templateId, string name) {
		this.gameObject.SetActive(false);
		BirdDetails.SetBird(templateId);
		UIObjects.sceneCon.StartLoad(1);
		announcementBird.sprite = GetBirdSprite(name,false);
	}
    
}
