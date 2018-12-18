
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fireballController : MonoBehaviour {

    public float pushSpeed = 10f;
    public float knockbackForce = 100f;
    public float attackDamage = 2f;
    public float maxAttackDamage = 10f;
    public float acceleration = 10f;
    public Vector3 maxScale;

    private bool leftFacing;
    private Rigidbody2D rb2D;
    private CameraMovement updateUI;
    private KillPlayer playerWins;
    private Vector3 baseScale;
    Scene m_Scene;

    [Header("Sounds")]
    public AudioClip explosion;
    private AudioSource Sound;
    private AudioSource explosionSound;
    

    public GameObject parent;
    // Use this for initialization
    void Start () {
        m_Scene = SceneManager.GetActiveScene();
        baseScale = new Vector3(1,1,1);
        Debug.Log("Starting up fireball");
        Physics2D.IgnoreLayerCollision(8,8);
        Physics2D.IgnoreLayerCollision(9,9);
        if(m_Scene.name == "Battle")
        {
            playerWins = GameObject.FindWithTag("Die").GetComponent<KillPlayer>();
            explosionSound = Camera.main.transform.GetChild(0).GetComponent<AudioSource>();
        }
        Sound = GetComponent<AudioSource>();
        updateUI = Camera.main.GetComponent<CameraMovement>();
        transform.localScale = baseScale;
        //parent = transform.parent.gameObject;
        //transform.parent = null;
        rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.gravityScale = 0;
        if (gameObject.tag == "rightFireball")
        {
            leftFacing = false;
        }
        else if (gameObject.tag == "leftFireball")
        {
            leftFacing = true;
        }
        var vel = rb2D.velocity;
        if (leftFacing)
        {
            vel.x = -pushSpeed;
        }
        else
        {
            vel.x = pushSpeed;
        }
        StartCoroutine(ScaleDamage());
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(m_Scene);
        if(rb2D == null){
            rb2D = GetComponent<Rigidbody2D>();
        }
        if (leftFacing)
        {
            rb2D.AddForce(Vector2.left * acceleration);
        }
        else
        {
            rb2D.AddForce(Vector2.right * acceleration);
        }
        //scale size over time
        transform.localScale = Vector3.Lerp(transform.localScale, maxScale, Time.deltaTime / 2);
        //scale damage over time
    }
    //if fireball hits a player that is not the creator do damage
    //if fireball hit terrain, destroy with explosion!
    void OnTriggerEnter2D(Collider2D hit)
    {
        Debug.Log("Trigger");
        //trying to ignore other fireballs
        if(hit.gameObject.tag == "rightFireBall" || hit.gameObject.tag == "leftFireBall"){
            //Physics2D.IgnoreCollision(hit.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        if(hit.gameObject.tag == "Player1" && parent.tag != "Player1")
        {
            if(hit.gameObject.GetComponent<PlayerController>().canBeDamaged){
                hit.transform.gameObject.GetComponent<PlayerController>().health -= attackDamage;
                hit.GetComponent<Collider2D>().gameObject.GetComponent<PlayerController>().damageText.TurnOnThenFade("-"+ attackDamage);
                if (leftFacing)
                {
                    hit.transform.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * knockbackForce);
                    if (m_Scene.name == "Battle")
                    {
                        updateUI.Player1_UIhealthSlider.value = hit.transform.gameObject.GetComponent<PlayerController>().health;
                        updateUI.Player1_UIhealthNum.text = updateUI.Player1_UIhealthSlider.value.ToString();
                    }
                    if(hit.transform.gameObject.GetComponent<PlayerController>().health <= 0)
                    {
                        hit.transform.gameObject.GetComponent<PlayerController>().numLifes--;
                        if(hit.transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                        {
                            playerWins.Player1Wins();
                        }
                        parent.GetComponent<PlayerController>().KillPlayer(hit.transform.gameObject.GetComponent<Rigidbody2D>());
                    }
                }
                else
                {
                    hit.transform.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce);
                    if(m_Scene.name == "Battle")
                    {
                        updateUI.Player1_UIhealthSlider.value = hit.transform.gameObject.GetComponent<PlayerController>().health;
                        updateUI.Player1_UIhealthNum.text = updateUI.Player1_UIhealthSlider.value.ToString();
                    }
                    if (hit.transform.gameObject.GetComponent<PlayerController>().health <= 0)
                    {
                        hit.transform.gameObject.GetComponent<PlayerController>().numLifes--;
                        if (hit.transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                        {
                            playerWins.Player1Wins();
                        }
                        parent.GetComponent<PlayerController>().KillPlayer(hit.transform.gameObject.GetComponent<Rigidbody2D>());
                    }
                }
                if(m_Scene.name == "Battle")
                {
                    explosionSound.clip = explosion;
                    explosionSound.Play();
                }
                Destroy(gameObject);
            }
            else{
                //absorb sound here?
                Destroy(gameObject);
                hit.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                hit.gameObject.GetComponent<PlayerController>().canBeDamaged = true;
                hit.gameObject.GetComponent<PlayerController>().blockCube.SetActive(false);
                hit.gameObject.GetComponent<PlayerController>().canBlock = false;
                hit.gameObject.GetComponent<PlayerController>().callBlockReady();
            }   
        }
        if (hit.gameObject.tag == "Player2" && parent.tag != "Player2")
        {
            if(hit.gameObject.GetComponent<PlayerController>().canBeDamaged){
                hit.transform.gameObject.GetComponent<PlayerController>().health -= attackDamage;
                hit.GetComponent<Collider2D>().gameObject.GetComponent<PlayerController>().damageText.TurnOnThenFade("-"+ attackDamage);
                if (leftFacing)
                {
                    hit.transform.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * knockbackForce);
                    if(m_Scene.name == "Battle")
                    {
                        updateUI.Player2_UIhealthSlider.value = hit.transform.gameObject.GetComponent<PlayerController>().health;
                        updateUI.Player2_UIhealthNum.text = updateUI.Player2_UIhealthSlider.value.ToString();
                    }
                    if (hit.transform.gameObject.GetComponent<PlayerController>().health <= 0)
                    {
                        hit.transform.gameObject.GetComponent<PlayerController>().numLifes--;
                        if (hit.transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                        {
                            playerWins.Player2Wins();
                        }
                        parent.GetComponent<PlayerController>().KillPlayer(hit.transform.gameObject.GetComponent<Rigidbody2D>());
                    }
                }
                else
                {
                    hit.transform.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce);
                    if (m_Scene.name == "Battle")
                    {
                        updateUI.Player2_UIhealthSlider.value = hit.transform.gameObject.GetComponent<PlayerController>().health;
                        updateUI.Player2_UIhealthNum.text = updateUI.Player2_UIhealthSlider.value.ToString();
                    }
                    if (hit.transform.gameObject.GetComponent<PlayerController>().health <= 0)
                    {
                        hit.transform.gameObject.GetComponent<PlayerController>().numLifes--;
                        if (hit.transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                        {
                            playerWins.Player2Wins();
                        }
                        parent.GetComponent<PlayerController>().KillPlayer(hit.transform.gameObject.GetComponent<Rigidbody2D>());
                    }
                }
                if (m_Scene.name == "Battle")
                {
                    explosionSound.clip = explosion;
                    explosionSound.Play();
                }
                Destroy(gameObject);
            }
            else{
                //absorb sound here?
                Destroy(gameObject);
                hit.gameObject.GetComponent<PlayerController>().canBeDamaged = true;
                hit.gameObject.GetComponent<PlayerController>().blockCube.SetActive(false);
                hit.gameObject.GetComponent<PlayerController>().canBlock = false;
                hit.gameObject.GetComponent<PlayerController>().callBlockReady();
                //Set block to disabled
            }
        }


        if(hit.gameObject.tag == "Ground" || hit.gameObject.tag == "fireballDie")
        {
            explosionSound.clip = explosion;
            explosionSound.loop = false;
            Sound.Play();
            Destroy(gameObject);
        }
    }
    
    IEnumerator ScaleDamage(){
        int currentCount = 0;
        float loopsRequiredToHitMax = maxAttackDamage - attackDamage;
        while(currentCount < loopsRequiredToHitMax){
            yield return new WaitForSeconds(.2f);
            if(attackDamage < maxAttackDamage){
                attackDamage++;
            }
            currentCount++;
        }
    }
}
