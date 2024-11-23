using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEmission : MonoBehaviour {

	void Update () {
		Renderer renderer = GetComponent<Renderer> ();
		Material mat = renderer.material;

		float low = 4.0f;
		float high = 1.0f;
		float emission = low + Mathf.PingPong (Time.time, high - low);
		Color baseColor = Color.white;

		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);

		mat.SetColor ("_EmissionColor", finalColor);
	}
}