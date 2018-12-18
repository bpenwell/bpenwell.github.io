using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howToPlay : MonoBehaviour {
	public SpriteRenderer player1;
	public SpriteRenderer player2;
	public GameObject Menu;
	public GameObject HowToPlayMenu;
    public GameObject World;
    public GameObject Player1;
    public GameObject Player2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SwitchToHowToPlay(){
		Menu.SetActive(false);
		player1.enabled = false;
		player2.enabled = false;
		HowToPlayMenu.SetActive(true);
        World.SetActive(true);
        Player1.SetActive(true);
        Player2.SetActive(true);
    }

	public void SwitchToMenu(){
		Menu.SetActive(true);
		player1.enabled = true;
		player2.enabled = true;
		HowToPlayMenu.SetActive(false);
        World.SetActive(false);
        Player1.SetActive(false);
        Player2.SetActive(false);
    }
}
