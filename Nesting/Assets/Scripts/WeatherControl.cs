using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherControl : MonoBehaviour
{
	
	public GameObject[] cloudPrefabs;
	public int startingClouds = 3;
	[Range(5,20)]
	public int maxClouds;
	[Range(0.1f,2f)]
	public float windSpeed = 0.1f; //Cloud movement speed
	
	private List<Transform> clouds = new List<Transform>();
	
	private int limitx = 10;
	private int limity = 10;
	private Vector3 windDirection;
	
	private float counter;
	private int delay;
	
	public float fogMaxDensity = 0.05f;
	
    // Start is called before the first frame update
    void Start()
    {
        limitx = BirdDetails.mapx * 2 + 10;
		limity = BirdDetails.mapy * 2 + 10;
		windDirection = Vector3.forward;
		delay = 30;
		counter = delay;
		for (int i = 0; i < startingClouds; i++) {
			Vector3 pos = new Vector3(Random.Range(-10, limitx), 50, Random.Range(-10, limity));
			GameObject cld = MakeCloud(pos);
			cld.transform.parent = this.transform;
			clouds.Add(cld.transform);
		}
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
		if ( clouds.Count < maxClouds && counter >= delay ) {
			counter = 0;
			Vector3 position = NewPosition();
			GameObject cld = MakeCloud(position);
			cld.transform.parent = this.transform;
			clouds.Add(cld.transform);
		}			
		
		foreach (Transform c in clouds){
			if (c.position.z > limity) {
				c.position = NewPosition();
			} else {
				c.Translate(windDirection * Time.deltaTime * windSpeed, Space.World);
			}
		}
		
		if (Katamari.volumeCheck > 0) {
			float fogDensity = fogMaxDensity - Katamari.volumeCheck / 10000 * 0.003f;
			RenderSettings.fogDensity = fogDensity;
		}
		
    }
	
	private Vector3 NewPosition() {
		return new Vector3(Random.Range(-10, limitx), 50, -20);
	}
	
	private GameObject MakeCloud(Vector3 position) {
		GameObject c = cloudPrefabs[Random.Range(0,cloudPrefabs.Length)];
		GameObject newCloud = Instantiate(c, position, Quaternion.identity) as GameObject;
		return newCloud;
	}
}
