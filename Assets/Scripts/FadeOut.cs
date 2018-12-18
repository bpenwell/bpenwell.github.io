using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {
	private bool canFade;
	private Color alphaColor;
	private float timeToFade = .5f;
	// Use this for initialization
	void Start () {
		canFade = false;
		alphaColor = GetComponent<TextMesh>().color;
	}
	
	// Update is called once per frame
	void Update () {
		if(canFade){
			alphaColor = GetComponent<TextMesh>().color;
			alphaColor.a -= timeToFade * Time.deltaTime;
			GetComponent<TextMesh>().color = alphaColor;
		}
		if(GetComponent<TextMesh>().color.a == 0 && canFade == true){
			canFade = false;
			Debug.Log("disable canFade");
		}
	}

	public void TurnOnThenFade(string display){
		GetComponent<TextMesh>().color = Color.white;
		GetComponent<TextMesh>().text = display;
		float count = 0;
		while(count < timeToFade){
			count += Time.deltaTime;
		}
		canFade = true;
	}

	public void SetInvisible(){
		alphaColor.a = 0;
		GetComponent<TextMesh>().color = alphaColor;
	}
}
