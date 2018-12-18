using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillPlayer : MonoBehaviour {

    public Transform Player1SpawnLoc;
    public Transform Player2SpawnLoc;

    public Rigidbody2D Player1;
    public Rigidbody2D Player2;

    public CameraMovement replaceReferences;

    public Text Player1_Wins;
    public Text Player2_Wins;
	public GameObject ReturnToMenu;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player1")
        {
            if(replaceReferences.Player1.GetComponent<PlayerController>().numLifes != 1){
                Rigidbody2D newPlayer1 = (Rigidbody2D)Instantiate(Player1, Player1SpawnLoc.position, Player1SpawnLoc.rotation);
                Player1 = newPlayer1;
                newPlayer1.GetComponent<PlayerController>().enabled = true;
                newPlayer1.GetComponent<BoxCollider2D>().enabled = true;
                newPlayer1.GetComponent<Animator>().enabled = true;
                newPlayer1.GetComponent<AudioSource>().enabled = true;
                replaceReferences.Player1 = newPlayer1.transform;
                replaceReferences.Player1.GetComponent<PlayerController>().numLifes--;
                replaceReferences.Player1.GetComponent<PlayerController>().health = 100;
                replaceReferences.Player1_UIhealthSlider.value = 100;
                //take off 1 life
                replaceReferences.Player1_UIhealthNum.text = "100";
            }
            else if(replaceReferences.Player1.GetComponent<PlayerController>().numLifes == 1){
                replaceReferences.Player1.GetComponent<PlayerController>().health = 0;
                replaceReferences.Player1_UIhealthSlider.value = 0;
                replaceReferences.Player1.GetComponent<PlayerController>().numLifes--;
                //take off 1 life
                replaceReferences.Player1_UIhealthNum.text = "0";
                Player2Wins();
            }
            //replaceReferences.Player1.GetComponent<PlayerController>().waitForDashThenKill(replaceReferences.Player1.GetComponent<Rigidbody2D>());
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "Player2")
        {
            if(replaceReferences.Player2.GetComponent<PlayerController>().numLifes != 1){
                Rigidbody2D newPlayer2 = (Rigidbody2D)Instantiate(Player2, Player2SpawnLoc.position, Player2SpawnLoc.rotation);
                Player2 = newPlayer2;
                newPlayer2.GetComponent<PlayerController>().enabled = true;
                newPlayer2.GetComponent<BoxCollider2D>().enabled = true;
                newPlayer2.GetComponent<Animator>().enabled = true;
                newPlayer2.GetComponent<AudioSource>().enabled = true;
                replaceReferences.Player2 = newPlayer2.transform;
                replaceReferences.Player2.GetComponent<PlayerController>().numLifes--;
                replaceReferences.Player2.GetComponent<PlayerController>().health = 100;
                replaceReferences.Player2_UIhealthSlider.value = 100;
                //take off 1 life
                replaceReferences.Player2_UIhealthNum.text = "100";
            }
            else if(replaceReferences.Player2.GetComponent<PlayerController>().numLifes == 1){
                replaceReferences.Player2.GetComponent<PlayerController>().health = 0;
                replaceReferences.Player2_UIhealthSlider.value = 0;
                replaceReferences.Player2.GetComponent<PlayerController>().numLifes--;
                //take off 1 life
                replaceReferences.Player2_UIhealthNum.text = "0";
                Player1Wins();
            }
            //replaceReferences.Player2.GetComponent<PlayerController>().waitForDashThenKill(replaceReferences.Player2.GetComponent<Rigidbody2D>());
            Destroy(col.gameObject);
        }
    }

    public void Player1Wins(){
        Debug.Log("Player 1 wins!");
        Player1_Wins.enabled = true;
        ReturnToMenu.SetActive(true);
    }

    public void Player2Wins(){
        Debug.Log("Player 2 wins!");
        Player2_Wins.enabled = true;
        ReturnToMenu.SetActive(true);
    }
}
