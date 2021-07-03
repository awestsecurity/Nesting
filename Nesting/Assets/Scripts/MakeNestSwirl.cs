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

	private string mostlyThis;
	private List<Transform> innerRing = new List<Transform>();
	private List<Transform> middleRing = new List<Transform>();
	private List<Transform> outerRing = new List<Transform>();

	public Vector3 center;
	public float swirlSpeed;
	public float katamariWidth;
	
    // Start is called before the first frame update
    void Start()
    {
        if (katamari == null) { katamari = GameObject.Find("Katamari"); }
		if (ui == null) { ui = GameObject.Find("HUD"); }
		ui.transform.GetChild(1).gameObject.SetActive(false); //hide announcement
		ui.transform.GetChild(3).gameObject.SetActive(false); //hide timer
		katamari.transform.position = Vector3.zero;
		RemoveKatamriControl();
		SplitKatamari();
		popup = ui.transform.GetChild(1).GetComponent<Popup>();
		popup.AddMessage($"It's awefully {mostlyThis}y. I don't know. It's your nest, but maybe you should work on that for the next one.");
		popup.LaunchMessagePanel();
    }

    // Update is called once per frame
    void Update()
    {
        foreach ( Transform t in innerRing ){
			t.RotateAround(center, Vector3.up, Time.deltaTime * swirlSpeed);
		}
        foreach ( Transform t in middleRing ){
			t.RotateAround(center, Vector3.up, Time.deltaTime * (swirlSpeed / 1.25f));
		}
        foreach ( Transform t in outerRing ){
			t.RotateAround(center, Vector3.up, Time.deltaTime * (swirlSpeed / 1.5f));
		}
    }
	
	void RemoveKatamriControl() {
		Katamari k = katamari.GetComponent<Katamari>();
		mostlyThis = k.mostCommonChild;
		Destroy (katamari.GetComponent<Rigidbody>());
		Destroy (katamari.GetComponent<Ball>());
		Destroy (katamari.GetComponent<BallUserControl>());
		Destroy (katamari.GetComponent<SphereCollider>());
		Destroy (k);
	}
	
	void SplitKatamari() {
		birdamari = katamari.transform.GetChild(0);
		birdamari.parent = this.transform;
		birdamari.position = new Vector3(0,2,-2.5f);
		int numchilds = katamari.transform.childCount - 1;
		int step = numchilds /3;
		float innerRingWidth = katamariWidth/3f;
		float middleRingWidth = (katamariWidth/3f)*2;
		float outerRingWidth = katamariWidth;
		for (int i = 0; i < step; i++) {
			Vector3 randomDirection = new Vector3(Random.Range(-0.5f,0.5f)*innerRingWidth,Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f)*innerRingWidth);
			Transform child = katamari.transform.GetChild(i);
			child.position = randomDirection; 
			innerRing.Add(child);
		}
		for (int i = step; i < step*2; i++) {
			Vector3 randomDirection = new Vector3(Random.Range(-0.5f,0.5f)*middleRingWidth,Random.Range(-0.25f,0.25f),Random.Range(-0.5f,0.5f)*middleRingWidth);
			Transform child = katamari.transform.GetChild(i);
			child.position = randomDirection; 
			middleRing.Add(child);
		}
		for (int i = step*2; i < numchilds; i++) {
			Vector3 randomDirection = new Vector3(Random.Range(-0.5f,0.5f)*outerRingWidth,0,Random.Range(-0.5f,0.5f)*outerRingWidth);
			Transform child = katamari.transform.GetChild(i);
			child.position = randomDirection; 
			outerRing.Add(child);
		}
	}
}