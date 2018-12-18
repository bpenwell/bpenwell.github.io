using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicController : MonoBehaviour {
    private CameraMovement findPlayer;
    private AudioSource sound;
    public float speedupPitch = 1.2f;
    public AudioClip oneLifeRemaining;
    public AudioSource remainingAudio;

    private bool p1_speak = true;
    private bool p2_speak = true;
    
    void Start()
    {
        sound = GetComponent<AudioSource>();
        findPlayer = GetComponent<CameraMovement>();
    }
	// Update is called once per frame
	void Update () {
        if(findPlayer.Player1.GetComponent<PlayerController>().numLifes == 1 && p1_speak)
        {
            remainingAudio.clip = oneLifeRemaining;
            remainingAudio.Play();
            p1_speak = false;
        }
        if (findPlayer.Player2.GetComponent<PlayerController>().numLifes == 1 && p2_speak)
        {
            remainingAudio.clip = oneLifeRemaining;
            remainingAudio.Play();
            p2_speak = false;
        }
        if (findPlayer.Player1.GetComponent<PlayerController>().numLifes == 1 || findPlayer.Player2.GetComponent<PlayerController>().numLifes == 1)
        {
            if (sound.pitch <= speedupPitch && speedupPitch > 0)
            {
                sound.pitch += .8f * Time.deltaTime;
            }
            else if (sound.pitch >= speedupPitch && speedupPitch < 0)
            {
                sound.pitch -= 1f * Time.deltaTime;
            }
                
        }
	}
}
