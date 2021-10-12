using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basically just a struct
public class BirdStats : MonoBehaviour {
	
    public string name {get;set;}
	public string status {get;set;}
	public int templateID {get;set;}
	public float speed {get;set;}
	public string flavortext {get;set;}
	public AudioClip tune {get;set;}
	
	public override string ToString(){
		return $"Bird: {name} - Status: {status} - ID: {templateID}";
	}  
	
}
