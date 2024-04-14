using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Ball;


public class MakeNestSwirl : MonoBehaviour
{
	
	private GameObject katamari;
	private Transform birdamari;
	private GameObject ui;
	private Popup popup; 
	private bool restarting = false;

	private string mostlyThis;
	private string[] mostlyThese;
	private int score;
	private List<Transform> ring = new List<Transform>();
	private List<Transform> middleRing = new List<Transform>();
	private List<Transform> outerRing = new List<Transform>();
	private float[] speeds;

	public Vector3 center;
	public float swirlSpeed;
	public float katamariWidth = 1;
	public float height = 0.5f;
	
    // Start is called before the first frame update
    void Start()
    {
		
        if (katamari == null) { katamari = GameObject.Find("Katamari"); }
		if (ui == null) { ui = GameObject.Find("HUD"); }
		//ui.transform.GetChild(1).gameObject.SetActive(false); //hide announcement
		ui.transform.GetChild(1).gameObject.SetActive(false); //hide timer
		katamari.transform.position = Vector3.zero;
		katamari.transform.rotation = Quaternion.identity;
		RemoveKatamriControl();
		SplitKatamari();
		popup = UIObjects.popup;
		switch(Random.Range(0,3)) {
			case 0:
				popup.AddMessage($"It's quite {mostlyThese[0]}y with a hint of {mostlyThese[1]}. How exciting!");
				break;
			case 1:
				popup.AddMessage($"That is a lot of {mostlyThese[0]}. The {mostlyThese[1]} is a nice touch though. Excellent!");
				break;
			case 2:
				popup.AddMessage($"I love a {mostlyThese[0]}y nest. The {mostlyThese[1]} balances it nicely!");
				break;
			default:
				popup.AddMessage($"It's quite {mostlyThese[0]}y with a hint of {mostlyThese[1]}. How exciting!");
				break;
		}
		int prevScore = PlayerPrefs.GetInt(BirdDetails.birdname, 0);
		if (score > prevScore) {
			PlayerPrefs.SetInt(BirdDetails.birdname, score);
			popup.AddMessage("" + score.ToString()+ ", That's the best yet!");
		} else {
			string s = score.ToString()+ " units of the good stuff ain't bad!";
			if (prevScore > 0) {
				s = s+" Your best was "+prevScore.ToString();
			}
			popup.AddMessage(s);
		}
		popup.LaunchMessagePanel();
		//Debug.Log($"Children: {katamari.transform.childCount} Ring:{ring.Count}");
    }

    // Update is called once per frame
    void Update()
    {	
		if (!restarting) {
			for (int i = 0; i < ring.Count; i++) {
				ring[i].RotateAround(center, new Vector3(0,1,-0.2f), speeds[i]*Time.deltaTime);
			}
		}
		if (Input.GetKeyDown("r") && !restarting) {
			bool b = UIObjects.sceneCon.StartLoad(1);
			if (b) {
				Destroy(katamari);
				restarting = true;
				Debug.Log("Restart.");
			} else {
				Debug.Log("Not Restarting.");	
			}
		} else if (Input.GetKeyDown("m") && !restarting) {
			bool b = UIObjects.sceneCon.StartLoad(0);
			if (b) {
				Destroy(katamari);
				restarting = true;
				Debug.Log("Restart.");
			} else {
				Debug.Log("Not Restarting.");	
			}
		}
    }
	
	void RemoveKatamriControl() {
		Katamari k = katamari.GetComponent<Katamari>();
		mostlyThis = k.mostCommonChild;
		mostlyThese = k.GetTopThings();
		score = (int)BirdDetails.score;
		Destroy (katamari.GetComponent<Rigidbody>());
		Destroy (katamari.GetComponent<Ball>());
		Destroy (katamari.GetComponent<BallUserControl>());
		Destroy (katamari.GetComponent<SphereCollider>());
		Destroy (k);
	}
	
	void SplitKatamari() {
		birdamari = katamari.transform.GetChild(0);
		birdamari.parent = this.transform;
		birdamari.position = new Vector3(0,1,-2.5f);
		int numchilds = katamari.transform.childCount;
		speeds = new float[numchilds];
		for (int i = 0; i < numchilds; i++) {
			Vector3 randomDirection = new Vector3(Random.Range(-katamariWidth, katamariWidth),0,Random.Range(-katamariWidth, katamariWidth));
			Transform child = katamari.transform.GetChild(i);
			float dist = Vector3.Distance(center,randomDirection);
			float h = height / dist;
			randomDirection.y = Random.Range(-h,h);
			child.position = randomDirection; 
			speeds[i] = swirlSpeed / dist;
			ring.Add(child);
		}
	}
	
	public void PassOnLoadScene(int i) {
		Destroy(katamari);
		restarting = true;
		UIObjects.sceneCon.ChangeScene(i);
	}
	
}