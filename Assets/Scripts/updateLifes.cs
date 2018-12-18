using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateLifes : MonoBehaviour {
    public GameObject[] life;
    public int maxLifes = 3;

    private int currentLifes;
    // Use this for initialization
	void Start () {
        currentLifes = maxLifes;
    }
	
	// Update is called once per frame
	void Update () {
        if (tag == "Player1")
        {
            if (Camera.main.GetComponent<CameraMovement>().Player1 == null)
            {
                life[0].SetActive(false);
            }
            else if (Camera.main.GetComponent<CameraMovement>().Player1.GetComponent<PlayerController>().numLifes != currentLifes)
            {
                currentLifes = Camera.main.GetComponent<CameraMovement>().Player1.GetComponent<PlayerController>().numLifes;
                for (int i = 0; i < maxLifes; i++)
                {
                    if (i < currentLifes)
                    {
                        life[i].SetActive(true);
                    }
                    else
                    {
                        life[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            if (Camera.main.GetComponent<CameraMovement>().Player2 == null)
            {
                life[0].SetActive(false);
            }
            else if (Camera.main.GetComponent<CameraMovement>().Player2.GetComponent<PlayerController>().numLifes != currentLifes)
            {
                currentLifes = Camera.main.GetComponent<CameraMovement>().Player2.GetComponent<PlayerController>().numLifes;
                for (int i = 0; i < maxLifes; i++)
                {
                    if (i < currentLifes)
                    {
                        life[i].SetActive(true);
                    }
                    else
                    {
                        life[i].SetActive(false);
                    }
                }
            }
        }
    }
}
