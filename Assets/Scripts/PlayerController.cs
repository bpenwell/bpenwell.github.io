using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Player Controls")]
    public KeyCode jump = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode strike = KeyCode.Space;
    public KeyCode dashLeft = KeyCode.Q;
    public KeyCode dashRight = KeyCode.E;
    public KeyCode block = KeyCode.F;
    public KeyCode fireball = KeyCode.R;
    [Header("Player Properties")]
    public int numLifes;
    public float speed = 50f;
    public float jumpForce = 1000f;
    public float jumpSpeed = 10f;
    public float maxJumpSpeed = 10f;
    public float maxSpeed = 7f;
    public float health = 100f;
    public float attackDamage = 7f;
    public float knockbackForce = 350f;
    public float blockRegenWaitTime = 2f;
    public float fireballWait = 1f;
    public float dashCooldownReset;
    public float dashCooldownTime;
    public float startDashTime;
    public float dashSpeed;
    public bool canBeDamaged = true;
    public GameObject blockCube;
    public float attackCooldownTime;
    public float stunTime = 1f;
    [Header("Prefabs")]
    public GameObject rightFireballPrefab;
    public GameObject leftFireballPrefab;
    //2 for player1, 3 for player2
    public int numJumps;
    public Transform Player1SpawnLoc;
    public Transform Player2SpawnLoc;

    [Header("UI elements")]
    public KillPlayer playerWins;
    public bool canBlock = true;
    public FadeOut blockedText;
    public FadeOut stunnedText;
    public FadeOut damageText;

    //private elements
    private float onPlatformSize = 0.666666f;
    private bool isStunned = false;
    private bool dashOnCooldown = false;
    private float dashTime = 0.1f;
    private bool attackAvailable = true;
    private bool fireballAvailable = true;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprite;
    private Animator anim;
    private CameraMovement updateUI;
    private bool isgrounded = false;
    private bool canDoubleJump = false;
    private int timesJumped;
    private Vector3 offsetPlayer = new Vector3(.55f,0,0);
    private Vector3 offsetPlayer2 = new Vector3(.55f, .55f,0);
    private Vector3 offsetPlayer3 = new Vector3(.55f,-.55f,0);
    private bool playerNum;
    Scene m_Scene;

    [Header("Seperate Animations")]
    public Animator rightDashAnim;
    public Animator leftDashAnim;

    [Header("Sounds")]
    public AudioClip blockSound;
    public AudioClip attackMiss;
    public AudioClip attackHit;
    public AudioClip dash;

    private AudioSource Sound;

	// Use this for initialization
	void Start () {
        m_Scene = SceneManager.GetActiveScene();
        damageText.SetInvisible();
		blockedText.SetInvisible();
		stunnedText.SetInvisible();
        playerNum = false;
        blockCube.SetActive(false);
        rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        updateUI = Camera.main.GetComponent<CameraMovement>();
        Sound = GetComponent<AudioSource>();
        if(m_Scene.name == "Battle")
            playerWins = GameObject.FindWithTag("Die").GetComponent<KillPlayer>();

        blockRegenWaitTime = 2f;
        fireballWait = 1f;
        dashCooldownReset = 1f;
        dashCooldownTime = 1f;

    startDashTime = dashTime;
        dashCooldownReset = dashCooldownTime;
        canBeDamaged = true;
        if(tag == "Player1")
        {
            playerNum = true;
        }
    	transform.localScale = new Vector3(1,1,1);
    }
    
    // Update is called once per frame
    void Update() {
        var vel = rb2D.velocity;
        anim.SetFloat("Speed", vel.magnitude);
        //Dash cooldown
        if(dashOnCooldown == true){
            dashCooldownTime -= Time.deltaTime;
        }
        //Setup dash cooldown
        if (isgrounded)
        {
            if(dashCooldownTime <= 0){
                dashOnCooldown = false;
                dashCooldownTime = dashCooldownReset;
            }
            
            timesJumped = 0;
            anim.SetBool("Jump", false);
        }
        //Attack
        if ((Input.GetKeyDown(strike) || (Input.GetButtonDown("P1_Attack") && playerNum) || (Input.GetButtonDown("P2_Attack") && !playerNum)) && attackAvailable && canBeDamaged && !isStunned)
        {
            attackAvailable = false;
            anim.SetBool("Attack", true);
            Invoke("turnAttackOff", .5f);
            StartCoroutine(waiter());
            float count = 0;
            float attackAnimationTime = .5f;
            bool hitSomeone = false;
            while(count < attackAnimationTime && !hitSomeone){
                RaycastHit2D[] hits = new RaycastHit2D[3];
            
                if(sprite.flipX == true){
                    for(int i=0;i<hits.Length;i++){
                        if(i==0)
                            hits[i] = Physics2D.Raycast(transform.position - offsetPlayer, transform.TransformDirection(Vector2.left), 2.0f);
                        if(i==1)
                            hits[i] = Physics2D.Raycast(transform.position - offsetPlayer2, transform.TransformDirection(Vector2.left), 2.0f);
                        if(i==2)
                            hits[i] = Physics2D.Raycast(transform.position - offsetPlayer3, transform.TransformDirection(Vector2.left), 2.0f);
                    }
                    bool didAttackHit = false;
                    for(int i=0;i<hits.Length;i++){
                        //if any collider is hit
                        if(hits[i].collider != null && (hits[i].collider.gameObject.tag == "Player1" || hits[i].collider.gameObject.tag == "Player2")){
                            didAttackHit = true;
                            //if other player blocking
                            if(hits[i].collider.gameObject.GetComponent<PlayerController>().canBeDamaged == false)
                            {
                                //destroy block
                                hits[i].collider.gameObject.GetComponent<PlayerController>().blockCube.SetActive(false);
                                //TODO: play block noise
                                hits[i].collider.gameObject.GetComponent<PlayerController>().canBlock = false;
                                hits[i].collider.gameObject.GetComponent<PlayerController>().canBeDamaged = true;
                                hits[i].collider.gameObject.GetComponent<PlayerController>().callBlockReady();
                                Sound.clip = blockSound;
                                Sound.Play();
                                //stun player
                                isStunned = true;
                                StartCoroutine(stun());
                                stunnedText.TurnOnThenFade("STUNNED!");
                                anim.SetBool("Stunned", true);
                                Invoke("turnStunOff", .5f);
                                hits[i].collider.gameObject.GetComponent<PlayerController>().blockedText.TurnOnThenFade("BLOCKED!");
                                hitSomeone = true;
                            }
                            else
                            {
                                Sound.clip = attackHit;
                                Sound.Play();
                                hits[i].transform.gameObject.GetComponent<PlayerController>().health -= attackDamage;
                                hits[i].collider.gameObject.GetComponent<PlayerController>().damageText.TurnOnThenFade("-"+ attackDamage);
                                hits[i].transform.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * knockbackForce);
                                hitSomeone = true;

                                if (hits[i].transform.gameObject.tag == "Player1")
                                {
                                    updateUI.Player1_UIhealthSlider.value = hits[i].transform.gameObject.GetComponent<PlayerController>().health;
                                    updateUI.Player1_UIhealthNum.text = updateUI.Player1_UIhealthSlider.value.ToString();
                                    if (hits[i].transform.gameObject.GetComponent<PlayerController>().health <= 0)
                                    {
                                        hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes--;
                                        if(hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                                        {
                                            playerWins.Player2Wins();
                                        }
                                        KillPlayer(hits[i].transform.gameObject.GetComponent<Rigidbody2D>());
                                    }
                                }
                                else
                                {
                                    updateUI.Player2_UIhealthSlider.value = hits[i].transform.gameObject.GetComponent<PlayerController>().health;
                                    updateUI.Player2_UIhealthNum.text = updateUI.Player2_UIhealthSlider.value.ToString();
                                    if (hits[i].transform.gameObject.GetComponent<PlayerController>().health <= 0)
                                    {
                                        hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes--;
                                        if(hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                                        {
                                            playerWins.Player1Wins();
                                        }
                                        KillPlayer(hits[i].transform.gameObject.GetComponent<Rigidbody2D>());
                                    }
                                }
                                for (int j = 0; j < hits.Length; j++)
                                {
                                    if (hits[j].collider != null)
                                    {
                                        if (j == 0)
                                            Debug.DrawRay(transform.position - offsetPlayer, transform.TransformDirection(Vector2.left) * 2.0f, Color.yellow, 2.0f);
                                        if (j == 1)
                                            Debug.DrawRay(transform.position - offsetPlayer2, transform.TransformDirection(Vector2.left) * 2.0f, Color.yellow, 2.0f);
                                        if (j == 2)
                                            Debug.DrawRay(transform.position - offsetPlayer3, transform.TransformDirection(Vector2.left) * 2.0f, Color.yellow, 2.0f);
                                    }
                                    else
                                    {
                                        if (j == 0)
                                            Debug.DrawRay(transform.position - offsetPlayer, transform.TransformDirection(Vector2.left) * 2.0f, Color.white, 2.0f);
                                        if (j == 1)
                                            Debug.DrawRay(transform.position - offsetPlayer2, transform.TransformDirection(Vector2.left) * 2.0f, Color.white, 2.0f);
                                        if (j == 2)
                                            Debug.DrawRay(transform.position - offsetPlayer3, transform.TransformDirection(Vector2.left) * 2.0f, Color.white, 2.0f);
                                    }
                                }
                            }
                            break;
                        }
                    }
                    if(didAttackHit == false)
                    {
                        Sound.clip = attackMiss;
                        Sound.Play();
                    }
                }
                else{
                    for(int i=0;i<hits.Length;i++){
                        if(i==0)
                            hits[i] = Physics2D.Raycast(transform.position + offsetPlayer, transform.TransformDirection(Vector2.right), 2.0f);
                        if(i==1)
                            hits[i] = Physics2D.Raycast(transform.position + offsetPlayer2, transform.TransformDirection(Vector2.right), 2.0f);
                        if(i==2)
                            hits[i] = Physics2D.Raycast(transform.position + offsetPlayer3, transform.TransformDirection(Vector2.right), 2.0f);
                    }
                    bool didAttackHit = false;
                    for(int i=0;i<hits.Length;i++){
                        //if any collider is hit
                        if(hits[i].collider != null && (hits[i].collider.gameObject.tag == "Player1" || hits[i].collider.gameObject.tag == "Player2")){
                            didAttackHit = true;
                            //if other player blocking
                            if (hits[i].collider.gameObject.GetComponent<PlayerController>().canBeDamaged == false)
                            {
                                //destroy block
                                hits[i].collider.gameObject.GetComponent<PlayerController>().blockCube.SetActive(false);
                                //TODO: play block noise
                                hits[i].collider.gameObject.GetComponent<PlayerController>().canBlock = false;
                                hits[i].collider.gameObject.GetComponent<PlayerController>().canBeDamaged = true;
                                hits[i].collider.gameObject.GetComponent<PlayerController>().callBlockReady();
                                Sound.clip = blockSound;
                                Sound.Play();
                                hitSomeone = true;
                                //stun player
                                isStunned = true;
                                StartCoroutine(stun());
                                stunnedText.TurnOnThenFade("STUNNED!");
                                anim.SetBool("Stunned", true);
                                Invoke("turnStunOff", .5f);
                                hits[i].collider.gameObject.GetComponent<PlayerController>().blockedText.TurnOnThenFade("BLOCKED!");
                            }
                            else
                            {
                                hits[i].transform.gameObject.GetComponent<PlayerController>().health -= attackDamage;
                                hits[i].collider.gameObject.GetComponent<PlayerController>().damageText.TurnOnThenFade("-"+ attackDamage);
                                hits[i].transform.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * knockbackForce);
                                Sound.clip = attackHit;
                                Sound.Play();
                                hitSomeone = true;

                                if (hits[i].transform.gameObject.tag == "Player1")
                                {
                                    updateUI.Player1_UIhealthSlider.value = hits[i].transform.gameObject.GetComponent<PlayerController>().health;
                                    updateUI.Player1_UIhealthNum.text = updateUI.Player1_UIhealthSlider.value.ToString();
                                    if (hits[i].transform.gameObject.GetComponent<PlayerController>().health <= 0)
                                    {
                                        hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes--;
                                        if(hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                                        {
                                            playerWins.Player2Wins();
                                        }
                                        KillPlayer(hits[i].transform.gameObject.GetComponent<Rigidbody2D>());
                                    }
                                }
                                else
                                {
                                    updateUI.Player2_UIhealthSlider.value = hits[i].transform.gameObject.GetComponent<PlayerController>().health;
                                    updateUI.Player2_UIhealthNum.text = updateUI.Player2_UIhealthSlider.value.ToString();
                                    if (hits[i].transform.gameObject.GetComponent<PlayerController>().health <= 0)
                                    {
                                        hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes--;
                                        if(hits[i].transform.gameObject.GetComponent<PlayerController>().numLifes == 0 && m_Scene.name == "Battle")
                                        {
                                            playerWins.Player1Wins();
                                        }
                                        KillPlayer(hits[i].transform.gameObject.GetComponent<Rigidbody2D>());
                                    }
                                }
                                for (int j = 0; j < hits.Length; j++)
                                {
                                    if (hits[j].collider != null)
                                    {
                                        if (j == 0)
                                            Debug.DrawRay(transform.position + offsetPlayer, transform.TransformDirection(Vector2.right) * 2.0f, Color.yellow, 2.0f);
                                        if (j == 1)
                                            Debug.DrawRay(transform.position + offsetPlayer2, transform.TransformDirection(Vector2.right) * 2.0f, Color.yellow, 2.0f);
                                        if (j == 2)
                                            Debug.DrawRay(transform.position + offsetPlayer3, transform.TransformDirection(Vector2.right) * 2.0f, Color.yellow, 2.0f);
                                    }
                                    else
                                    {
                                        if (j == 0)
                                            Debug.DrawRay(transform.position + offsetPlayer, transform.TransformDirection(Vector2.right) * 2.0f, Color.white, 2.0f);
                                        if (j == 1)
                                            Debug.DrawRay(transform.position + offsetPlayer2, transform.TransformDirection(Vector2.right) * 2.0f, Color.white, 2.0f);
                                        if (j == 2)
                                            Debug.DrawRay(transform.position + offsetPlayer3, transform.TransformDirection(Vector2.right) * 2.0f, Color.white, 2.0f);
                                    }
                                }
                            }
                            break;
                        }
                    }
                    if(didAttackHit == false)
                    {
                        Sound.clip = attackMiss;
                        Sound.Play();
                    }
                }
                //while operations
                count += Time.deltaTime;
            }
        }
        //Jump
        if ((Input.GetKeyDown(jump) || (Input.GetButtonDown("P1_Jump") && playerNum) || (Input.GetButtonDown("P2_Jump") && !playerNum)) && !anim.GetBool("Attack") && canBeDamaged && !isStunned)
        {
            if (isgrounded || (!isgrounded && timesJumped == 0))
            {
            	if(!isgrounded && timesJumped == 0){
            		timesJumped++;
            	}
                anim.SetBool("Jump", true);
                vel.y = 0;
                rb2D.velocity = vel;
                rb2D.AddForce(Vector2.up * jumpForce);
                canDoubleJump = true;
            }
            else
            {
                anim.SetBool("Jump", false);
                if (canDoubleJump)
                {
                    anim.SetBool("Jump", true);
                    timesJumped++;
                    if(numJumps <= timesJumped){
                        canDoubleJump = false;
                    }
                    vel.y = 0;
                    rb2D.velocity = vel;
                    rb2D.AddForce(Vector2.up *jumpForce);
                }
            } 
        }
        //Block
        else if((Input.GetKeyDown(fireball) || ((Input.GetButtonDown("P1_Fireball") && playerNum) || (Input.GetButtonDown("P2_Fireball") && !playerNum))) && fireballAvailable && canBeDamaged && !isStunned){
            vel = Vector3.zero;
            fireballAvailable = false;
            //change to fireball animation
            if (sprite.flipX == true)
            {
                GameObject newFireball = Instantiate(leftFireballPrefab, leftFireballPrefab.transform.position, leftFireballPrefab.transform.rotation);
                newFireball.transform.parent = transform;
                //turn on deactivated SpriteRenderer, BoxCollider2D, Animator, FireballController, and AudioSource
                newFireball.GetComponent<SpriteRenderer>().enabled = true;
                newFireball.GetComponent<BoxCollider2D>().enabled = true;
                newFireball.GetComponent<Animator>().enabled = true;
                newFireball.GetComponent<AudioSource>().enabled = true;
                newFireball.GetComponent<fireballController>().enabled = true;
            }
            else{
                GameObject newFireball = Instantiate(rightFireballPrefab, rightFireballPrefab.transform.position, rightFireballPrefab.transform.rotation);
                newFireball.transform.parent = transform;

                newFireball.GetComponent<SpriteRenderer>().enabled = true;
                newFireball.GetComponent<BoxCollider2D>().enabled = true;
                newFireball.GetComponent<Animator>().enabled = true;
                newFireball.GetComponent<fireballController>().enabled = true;
                newFireball.GetComponent<AudioSource>().enabled = true;
            }
            StartCoroutine(Fireballwaiter());
            //anim.SetBool("Attack", true);
            //Invoke("turnAttackOff", .5f);
            //StartCoroutine(waiter());
        }
        else if((Input.GetKey(block) || (Input.GetButton("P1_Block") && playerNum) || (Input.GetButton("P2_Block") && !playerNum)) && isgrounded && attackAvailable && canBlock && !isStunned)
        {
            vel = Vector3.zero;
            canBeDamaged = false;
            blockCube.SetActive(true);
        }
        //Dash left
        else if((Input.GetKeyDown(dashLeft) || (Input.GetButtonDown("P1_DashLeft") && playerNum) || (Input.GetButtonDown("P2_DashLeft") && !playerNum)) && !dashOnCooldown && canBeDamaged && !isStunned)
        {
            Sound.clip = dash;
            Sound.Play();
            sprite.flipX = true; 
            dashOnCooldown = true;  
            StartCoroutine(DashLeft());
            leftDashAnim.SetBool("Dash", true);
            Invoke("turnLeftDashOff", .5f);
        }
        //Dash right
        else if((Input.GetKeyDown(dashRight) || (Input.GetButtonDown("P1_DashRight") && playerNum) || (Input.GetButtonDown("P2_DashRight") && !playerNum)) && !dashOnCooldown && canBeDamaged && !isStunned)
        {
            Sound.clip = dash;
            Sound.Play();
            sprite.flipX = false;
            dashOnCooldown = true;
            StartCoroutine(DashRight());
            rightDashAnim.SetBool("Dash", true);
            Invoke("turnRightDashOff", .5f);
        }
        //Mid-air move left
        else if((Input.GetKey(left) || (Input.GetAxis("P1_Horizontal") <= -.5 && playerNum) || (Input.GetAxis("P2_Horizontal") <= -.5 && !playerNum)) && !isgrounded && canBeDamaged && !isStunned)
        {
            if(vel.x > 0){
                vel.x = -vel.x;
            }
            if(-vel.x < maxJumpSpeed){
                rb2D.AddForce(Vector2.left * jumpSpeed);
            }
            sprite.flipX = true;
        }
        //Mid-air move right
        else if((Input.GetKey(right) || (Input.GetAxis("P1_Horizontal") >= .5 && playerNum) || (Input.GetAxis("P2_Horizontal") >= .5 && !playerNum)) && !isgrounded && canBeDamaged && !isStunned)
        {
            if (vel.x < 0){
                vel.x = -vel.x;
            }
            if(vel.x < maxJumpSpeed){
                rb2D.AddForce(Vector2.right * jumpSpeed);
            }
            sprite.flipX = false;
        }
        //On the ground move left
        else if ((Input.GetKey(left) || (Input.GetAxis("P1_Horizontal") <= -.5 && playerNum) || (Input.GetAxis("P2_Horizontal") <= -.5 && !playerNum)) && vel.magnitude < maxSpeed && !anim.GetBool("Attack") && canBeDamaged && !isStunned)
        {
            if (isgrounded){
                rb2D.AddForce(Vector2.left * speed);
                sprite.flipX = true;
            }
        }
        //On the ground move right
        else if ((Input.GetKey(right) || (Input.GetAxis("P1_Horizontal") >= .5 && playerNum) || (Input.GetAxis("P2_Horizontal") >= .5 && !playerNum)) && vel.magnitude < maxSpeed && !anim.GetBool("Attack") && canBeDamaged && !isStunned)
        {
            if (isgrounded){
                rb2D.AddForce(Vector2.right * speed);
                sprite.flipX = false;
            }
        }
        //Set velocity after changes that were made
        rb2D.velocity = vel;
        //Resets if not blocking
        if ( (!Input.GetKey(block) && (!Input.GetButton("P1_Block") && playerNum)) || (!Input.GetKey(block) && (!Input.GetButton("P2_Block") && !playerNum)))
        {
            canBeDamaged = true;
            blockCube.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D theCollision)
    {
        if(theCollision.gameObject.tag == "Ground")
        {
            isgrounded = true;
        }
        if(theCollision.gameObject.tag == "platformGround"){
        	isgrounded = true;
        	transform.SetParent(theCollision.gameObject.transform);
        }
    }
    
    void OnCollisionExit2D(Collision2D theCollision)
    {
        if (theCollision.gameObject.tag == "Ground")
        {
            isgrounded = false;
        }
        if(theCollision.gameObject.tag == "platformGround"){
        	isgrounded = false;
        	transform.parent = null;
        }
    }

    //trigger attack animation
    void turnAttackOff()
    {
        anim.SetBool("Attack", false);
    }
    //stop dash (wind) animation
    void turnLeftDashOff(){
        leftDashAnim.SetBool("Dash", false);
    }
    //stop dash (wind) animation
    void turnRightDashOff(){
        rightDashAnim.SetBool("Dash", false);
    }

    void turnStunOff(){
        anim.SetBool("Stunned", false);
    }

    private IEnumerator DashLeft(){
        while(dashTime >= 0){
            dashTime -= Time.deltaTime;
            rb2D.velocity = Vector2.left * dashSpeed;
            yield return null; //wait for next frame
        }
        rb2D.velocity = Vector2.zero;
        dashTime = startDashTime;
        
    }

    private IEnumerator DashRight(){
        while(dashTime >= 0){
            dashTime -= Time.deltaTime;
            rb2D.velocity = Vector2.right * dashSpeed;
            yield return null; //wait for next frame
        }
        rb2D.velocity = Vector2.zero;
        dashTime = startDashTime;
         
    }
    //Fireball delay
    IEnumerator Fireballwaiter()
    {
        float count = 0;
        while (count < fireballWait)
        {
            count += Time.deltaTime;
            yield return null;
        }
        fireballAvailable = true;
    }

    //Cooldown timer
    IEnumerator waiter(){
        float count = 0;
        while(count < attackCooldownTime){
            count += Time.deltaTime;
            yield return null;
        }
        attackAvailable = true;
    }

    IEnumerator stun(){
        float count = 0;
        while(count < stunTime){
            count += Time.deltaTime;
            yield return null;
        }
        isStunned = false;
    }
    //Kill player 1 or 2
    public void KillPlayer(Rigidbody2D rb){
        if(rb.gameObject.tag == "Player1"){
            if(rb.gameObject.GetComponent<PlayerController>().numLifes != 0){
                rb.gameObject.GetComponent<PlayerController>().health = 100;
                Rigidbody2D newPlayer1 = (Rigidbody2D)Instantiate(rb, new Vector3(-6, 5, 0), gameObject.transform.rotation);
                updateUI.Player1 = newPlayer1.transform;
                updateUI.Player1_UIhealthNum.text = "100";
                updateUI.Player1_UIhealthSlider.value = 100;
            }
            else{ //if(rb.gameObject.GetComponent<PlayerController>().numLifes == 1){
                rb.gameObject.GetComponent<PlayerController>().health = 0;
                updateUI.Player1_UIhealthNum.text = "0";
                updateUI.Player1_UIhealthSlider.value = 0;
            }
            //rb.gameObject.transform.parent = null;
            bool waitForAnimation = true;
            while(waitForAnimation){
            	if(!leftDashAnim.GetCurrentAnimatorStateInfo(0).IsName("dash") && !rightDashAnim.GetCurrentAnimatorStateInfo(0).IsName("dash")){
            		Destroy(rb.gameObject);
            		waitForAnimation = false;
            	}
            }

        }
        else if(rb.gameObject.tag == "Player2"){
            if(rb.gameObject.GetComponent<PlayerController>().numLifes != 0){
                rb.gameObject.GetComponent<PlayerController>().health = 100;
                Rigidbody2D newPlayer2 = (Rigidbody2D)Instantiate(rb, new Vector3(6, 5, 0), gameObject.transform.rotation);
                updateUI.Player2 = newPlayer2.transform;
                updateUI.Player2_UIhealthNum.text = "100";
                updateUI.Player2_UIhealthSlider.value = 100;
            }
            else{
                rb.gameObject.GetComponent<PlayerController>().health = 0;
                updateUI.Player2_UIhealthNum.text = "0";
                updateUI.Player2_UIhealthSlider.value = 0;
            }
            //rb.gameObject.transform.parent = null;
            bool waitForAnimation = true;
            while(waitForAnimation){
            	if(!leftDashAnim.GetCurrentAnimatorStateInfo(0).IsName("dash") && !rightDashAnim.GetCurrentAnimatorStateInfo(0).IsName("dash")){
            		Destroy(rb.gameObject);
            		waitForAnimation = false;
            	}
            }
        }
        
    }

    public void callBlockReady()
    {
        Invoke("blockReady", blockRegenWaitTime);
    }

    private void blockReady()
    {
        canBlock = true;
    }
}
