using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapRotater : MonoBehaviour {
	public float rotateSpeed = 5f;
	public GameObject rotate;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//rotate.transform.Rotate(0,0,Time.deltaTime * rotateSpeed,0);
		transform.Rotate(0,0,Time.deltaTime * rotateSpeed,0);
	}
}
