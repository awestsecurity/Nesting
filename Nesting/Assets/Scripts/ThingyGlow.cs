using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]
public class ThingyGlow : Thingy {

	public Light _light;
	private float maxIntensity;
	private Transform katamriLocation;
	private bool selfManage = false;
	private Transform katamari;
	private float distanceToIlluminate = 15;
		
	protected override void PrimeObject(){
		maxIntensity = _light.intensity;
		#if UNITY_WEBGL
			//_light.enabled = false;
			selfManage = true;
			katamari = Katamari.trans;
		#endif
	}
	
	public void StartLightFade(int sec = 9){
		if (_light) {
			StartCoroutine(FadeLight(sec));
		}
	}
	
	protected override void Update () {
		if (delay && !primed && actualVolume < (Katamari.volumeCheck)) {
			PrimeForKatamari();
		} else if (delay && !assimilated && testForGravity) {
			
			if (!Physics.Raycast(transform.position, -Vector3.up, 0.1f)) {
				StartCoroutine(TemporaryGravity());
				testForGravity = false;
			}
		}
		if (ai && assimilated) {
			ai = false;
			Destroy(gameObject.GetComponent<Wander>()); //Remove Wander first to avoid error on controller dependence
			Destroy(gameObject.GetComponent<CharacterController>());
		}
		if (selfManage && !assimilated) {
			if (!katamari) {
				katamari = Katamari.trans;
			}
			float dist = Vector3.Distance(transform.position, katamari.position);
			if (dist < distanceToIlluminate) {
				_light.enabled = true;
				_light.intensity = (maxIntensity * ((distanceToIlluminate-dist)/distanceToIlluminate));
			} else {
				_light.enabled = false;
			}
		}
	}
	
	IEnumerator FadeLight(int sec) {
		float timer = 0;
		float intensity = _light.intensity;
		while (_light.intensity > 0) {
			timer += Time.deltaTime;
			_light.intensity = (1 - (timer/sec)) * intensity;
			yield return new WaitForEndOfFrame();
		}
		KillLightComponent();
	}
	
	void KillLightComponent() {
		_light.enabled = false;
		//Destroy(_light);
	}
	
}