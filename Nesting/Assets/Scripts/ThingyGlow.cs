using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]
public class ThingyGlow : Thingy {

	public Light _light;
	
	public void StartLightFade(int sec = 9){
		if (_light) {
			StartCoroutine(FadeLight(sec));
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
		Destroy(_light);
	}
	
}