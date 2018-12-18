using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public Vector3 startPosition = new Vector3(0, 3, 0);
    public Vector3 targetHeight = new Vector3(0,9,0);
    public float moveSpeed = 3f;

    private Transform me;
	// Use this for initialization
	void Start () {
        me = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 newPosition = new Vector3(0,1,0) * (moveSpeed * Time.deltaTime);
        newPosition = transform.position + newPosition;
        newPosition.y = Mathf.Clamp(newPosition.y, startPosition.y, targetHeight.y);
        transform.position = newPosition;

        if(transform.position.y >= targetHeight.y)
        {
            moveSpeed *= -1;
        }
        if(transform.position.y <= startPosition.y)
        {
            moveSpeed *= -1;
        }
	}
}
