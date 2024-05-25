using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdMenu : MonoBehaviour{
	
	public Transform rectangle;
	public GameObject buttonPrefab;
	public Image announcementBird;
	public AutoScroll scrollView;
	
	private int[] templates;
	private string[] images;
	private string[] statuses;
	private string[] flavor;
	
	private BirdStats[] options;
	private bool compiled = false;
	private Sprite[] birdAtlas;
	private Sprite[] birdOAtlas;
	
	void OnEnable() {
		string birdsFromPrefs = PlayerPrefs.GetString("666", "a");
		int currentBirds = BirdDetails.ownedBirds.Length;
		if (compiled && currentBirds < birdsFromPrefs.Length) {
			Debug.Log("AddNewBirds");
			RefreshBirdList();
			for (int i = currentBirds; i < birdsFromPrefs.Length; i++) {
				AddBird(i);
			}
			scrollView.Refresh();
		}
	}
	
	public void MakeBirdMenu() {
		if (!compiled) {
			templates = BirdDetails.ownedBirds;
			options = new BirdStats[templates.Length];
			images = BirdDetails.GetBirdJSONStat(templates, "name");
			statuses = BirdDetails.GetBirdJSONStat(templates, "status");
			flavor = BirdDetails.GetBirdJSONStat(templates, "text");
			//float[] speeds = BirdDetails.GetBirdJSONStatAsFloat(templates, "speed");
			LoadSprites();
			birdAtlas = new Sprite[templates.Length];
			birdOAtlas = new Sprite[templates.Length];
			for ( int i = 0; i < templates.Length; i++ ) {
				//Add button to menu
				AddBird(i);
			}
			compiled = true;
		}
	}
	
	private void AddBird(int i){
		GameObject b = Instantiate(buttonPrefab) as GameObject;
		BirdStats stats = b.GetComponent<BirdStats>();
		b.transform.SetParent(rectangle);
		b.name = images[i];
		stats.name = b.name;
		stats.templateID = templates[i];
		stats.status = statuses[i];
		stats.flavortext = flavor[i];
		//stats.speed = speeds[i]; //not used
		b.transform.localScale = new Vector3(1,1,1);
		//Add bird image to button
		Image img = b.GetComponent<Image>();
		img.sprite = GetBirdSprite(images[i], true);
		
		//Add used sprites to persistent object
		birdOAtlas[i] = img.sprite;
		birdAtlas[i] = GetBirdSprite(images[i], false);
			
		//Add on click function
		Button button = b.GetComponent<Button>();
		int id = templates[i];
		string name = images[i];
		options[i] = stats;
		button.onClick.AddListener(delegate {AssignChosenBird(id,name); });
	}
	
	public void RefreshBirdList() {
		UIObjects.network.FetchBirdsPlayerPrefs();
		templates = BirdDetails.ownedBirds;
		options = new BirdStats[templates.Length];
		images = BirdDetails.GetBirdJSONStat(templates, "name");
		statuses = BirdDetails.GetBirdJSONStat(templates, "status");
		flavor = BirdDetails.GetBirdJSONStat(templates, "text");
			//float[] speeds = BirdDetails.GetBirdJSONStatAsFloat(templates, "speed");
			//LoadSprites();
		birdAtlas = new Sprite[templates.Length];
		birdOAtlas = new Sprite[templates.Length];
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
	
	public void SelectRandomBird() {
		if (options != null) {
			int choice = Random.Range(0,options.Length);
			AssignChosenBird(options[choice].templateID, options[choice].name);
		} else {
			AssignChosenBird(279598,"Rock Pigeon");
		}
	}
	
	public void AssignChosenBird(int templateId, string name) {
		this.gameObject.SetActive(false);
		BirdDetails.SetBird(templateId);
		UIObjects.sceneCon.StartLoad(1);
		announcementBird.sprite = GetBirdSprite(name,false);
	}
    
}
