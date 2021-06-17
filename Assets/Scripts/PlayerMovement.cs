using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    public int maxHealth;
    private int currentHealth;
    public bool isDead;
    public GameObject thisBar;
    public GameObject otherBar;

    [Header("Abilities")]
    public bool canDash;
    public float nbJumpsAllowed = 1;
    
    [Header("Components")]
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask dangerLayerMask;
    [SerializeField] private LayerMask laddersLayerMask;
    
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    private Vector2 direction;
    private Vector2 oldDirection = Vector2.right;
    private bool facingRight = true;
    private Vector2 lastInterestingDir;

    [Header("Vertical Movement")]
    public float jumpVelocity = 10f;
    private float jumpTimer = 0f;
    private float jumpDelay = 0.25f;
    private float nbJump = 0;

    [Header("Collision")] 
    public bool onGround = false;

    [Header("Physics")] 
    public float linearDrag;
    public float gravity;
    public float fallMultiplier;
    
    [Header("Dash")]
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private bool isDashing = false;
    private Vector2 dashDirection = Vector2.right;
    public float DASH_COOLDOWN = 1f;
    private float dashCooldownStatus;
    public GameObject dashParticleRight;
    public GameObject dashParticleLeft;
    
    [Header("Attack")]
    private bool isAttacking = false;
    public Transform attackPoint;
    public float attackRange = 1.4f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;
    private float ATTACK_COOLDOWN = 0.3f;
    private float attackCooldownStatus;
    private bool hasAttacked;

    [Header("Orientation")]
    public Vector2 orientation = Vector2.right;
    
    [Header("Switch")]
    private GameObject playerTop;
    private GameObject playerBot;
    private bool isSwitching = false;
    private float SWITCH_COOLDOWN = 1f;
    public float switchCooldownStatus;

    [Header("WallJump")]
    private Vector2 onWall;
    private bool isWallJumping;
    private bool isWallSliding;

    [Header("Animation")] 
    public Animator animator;
    
    [Header("Scene")]
    private bool horizontal;
    private bool both;

    [Header("Particles Systems")] 
    public ParticleSystem bloodEffect;
    
    
    [Header(("Damage"))]
    private bool isInvincible = false; // triggered when enemy contact
    private int dangerousTilesDmg = 30;

    [Header("Cameras")] 
    private GameObject[] cameras;
    private int cameraIndex;
    public bool focusOnKingBlob;


    public AudioManager am;
    
    
    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
        currentHealth = maxHealth;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        dashTime = startDashTime;
        dashCooldownStatus = 0f;
        attackCooldownStatus = 0f;
        animator = GetComponentInChildren<Animator>();
        switchCooldownStatus = 0f;
        // if (SceneManager.GetActiveScene().name[0] == 'H')
        // {
        //     horizontal = true;
        // }
        // else if (SceneManager.GetActiveScene().name[0] == 'V')
        // {
        //     horizontal = false;
        // }
        // else
        // {
        //     both = true;
        // }

        cameras = Camera.main.GetComponent<CameraMovement>().cameras;
    }

    private void OnDestroy()
    {
        am.StopSound("Walk");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        
        
        if (other.gameObject.CompareTag("Player")) {
            Physics2D.IgnoreCollision(other.collider, boxCollider2d);
        }

        if (other.gameObject.CompareTag("Pieds"))
        {
            Debug.LogFormat("COLLISION : {0}", other.gameObject.transform.parent.parent.name);

            Physics2D.IgnoreCollision(other.collider, boxCollider2d);
            
        
        }
    }

    private void Update()
    {        
        
        if (playerBot == null)
        {
            playerBot = GameObject.Find("PlayerBot(Clone)");
        }

        if (playerTop == null)
        {
            playerTop = GameObject.Find("PlayerTop(Clone)");
        }
        

        
        // Check the view
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }

        GameObject playerTopsign = GameObject.Find("playertop_label");
        if (playerTopsign == null)
        {
            playerTopsign = new GameObject("playertop_label");
            TextMesh tm1 = playerTopsign.AddComponent<TextMesh>();
            tm1.text = PhotonNetwork.MasterClient.NickName;
            tm1.color = new Color(0.8f, 0.8f, 0.8f);
            tm1.fontStyle = FontStyle.Bold;
            tm1.alignment = TextAlignment.Center;
            tm1.anchor = TextAnchor.MiddleCenter;
            tm1.characterSize = 0.065f;
            tm1.fontSize = 40;
        }
        
        GameObject playerBotsign = GameObject.Find("playerbot_label");
        if (playerBotsign == null)
        {
            playerBotsign = new GameObject("playerbot_label");
            TextMesh tm2 = playerBotsign.AddComponent<TextMesh>();
            tm2.text = PhotonNetwork.PlayerList[1].NickName;
            tm2.color = new Color(0.8f, 0.8f, 0.8f);
            tm2.fontStyle = FontStyle.Bold;
            tm2.alignment = TextAlignment.Center;
            tm2.anchor = TextAnchor.MiddleCenter;
            tm2.characterSize = 0.065f;
            tm2.fontSize = 40;
        }

        try
        {
            if (gameObject == playerTop)
            {
                playerTopsign.transform.position = gameObject.transform.position + Vector3.up * 1.5f;
                playerBotsign.transform.position = playerBot.transform.position + Vector3.up * 1.5f;
            }
            else
            {
                playerBotsign.transform.position = gameObject.transform.position + Vector3.up * 1.5f;
                playerTopsign.transform.position = playerTop.transform.position + Vector3.up * 1.5f;
            }
        }
        catch (Exception)
        {
            
        }
        
        if (isDead)
        {
            // PhotonNetwork.Destroy(gameObject);
            return;
        }
            

        if (Input.GetKeyDown(KeyCode.Space) && nbJump < nbJumpsAllowed)
        {
            jumpTimer = Time.time + jumpDelay;
            nbJump += 1;
        }

        if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownStatus <= 0f)
        {
            isDashing = true;
            dashTime = startDashTime;
            dashDirection = orientation;
            Debug.Log("Dashing");
        }

        if (dashCooldownStatus > 0)
        {
            dashCooldownStatus -= Time.deltaTime;
        }

        if (attackCooldownStatus > 0)
        {
            attackCooldownStatus -= Time.deltaTime;
        }
        else
        {
            hasAttacked = false;
            animator.SetBool("isAttacking", false);
        }


        if (Input.GetKeyDown(KeyCode.A) && attackCooldownStatus <= 0f)
        {
            Debug.Log("Attacking");
            isAttacking = true;
            
        }


        if (switchCooldownStatus > 0)
        {
            switchCooldownStatus -= Time.deltaTime;
        }
        
        if (Input.GetKeyDown(KeyCode.S) && switchCooldownStatus <= 0f)
        {
            Debug.Log("Switching");
            isSwitching = true;
        }

        if (switchCooldownStatus <= 0.3f)
        {
            try
            {
                playerTop.GetComponent<TrailRenderer>().time = 0.6f;
                playerBot.GetComponent<TrailRenderer>().time = 0.6f;

            }
            catch (Exception)
            {
                
            }
            
        }
        
        if (switchCooldownStatus > 0)
        {
            switchCooldownStatus -= Time.deltaTime;
        }
        
        onGround = IsGrounded();
        if (onGround)
        {
            nbJump = 0;
            
        }
            

        onWall = IsTouchingWalls();
        if (Input.GetKey(KeyCode.C) && !onGround && onWall != Vector2.zero && rigidbody2d.velocity.y < 0)
        {
            Debug.Log("WallSliding");
            isWallSliding = true;
            // animator.SetTrigger("wallSlide");
            animator.SetBool("isWallSliding", true);
            animator.SetBool("isTurningHeadWallSlide", onWall != oldDirection);
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("isWallSliding", false);
        }


        
        if (isWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            isWallJumping = true;
            isWallSliding = false;
            jumpTimer = Time.time + jumpDelay;
        }


        if (direction != Vector2.zero)
        {
            oldDirection = direction;
        }
        
        if (direction != Vector2.zero)
            animator.SetBool("isWalking", true);
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (onGround)
            animator.SetBool("isJumping", false);

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0);;

        

        if (direction != oldDirection && direction != Vector2.zero && oldDirection != Vector2.zero)
        {
            Flip();
        }
        if (direction != Vector2.zero)
        {
            oldDirection = direction;
        }
        
        
        
        if (direction != Vector2.zero)
            orientation = direction;

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Damage");
            TakeDamage(20);
        }
    }

    
    public void TakeDamage(int dmg)
    {
        if(!isInvincible)
        {
            ApplyDamage(dmg);
            photonView.RPC("SetHealthBar", RpcTarget.All, currentHealth, thisBar.name);
            isInvincible = true;
            StartCoroutine(InvincibilityFlash(gameObject.name));
            StartCoroutine(HandleInvincibilityDelay());
        }
    }
    
    
    [PunRPC]
    private void ChangeBodyVisibility(string playerName, float r, float g, float b, float a)
    {
        GameObject playerSprite = GameObject.Find(playerName).transform.GetChild(1).gameObject;
        for (int i = 0; i < 6; i++)
        {
            playerSprite.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
        }
    }
    
    public IEnumerator InvincibilityFlash(string playerName)
    {
        while(isInvincible)
        {
            photonView.RPC("ChangeBodyVisibility", RpcTarget.All, playerName, 1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(0.15f);
            photonView.RPC("ChangeBodyVisibility", RpcTarget.All, playerName, 1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public IEnumerator HandleInvincibilityDelay()
    {
        yield return new WaitForSeconds(2f);
        isInvincible = false;
    }
    
    
    // private void Victory()
    // {
    //     // Ajouter des confettis
    //     GameObject gameOverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
    //     gameOverPanel.transform.Find("gameover Label").GetComponent<Text>().text = "Congratulations !";
    //     gameOverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName + " and " + PhotonNetwork.PlayerList[1].NickName + " won";
    //     gameOverPanel.SetActive(true);
    // }

    /*public override void OnTriggerEnter2D(Collider2D enemy)
    {
        if (enemy.gameObject.tag == "Enemies")
        {
            TakeDamage(20); // take 20 dmg
        }
        if (enemy.gameObject.tag == "Water")
        {
            TakeDamage(10);
        }
    }*/

    public int ApplyDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        return currentHealth;
    }

    public void Die()
    {
        animator.SetTrigger("death");
        photonView.RPC("DisplayDeath", RpcTarget.All, PhotonNetwork.NickName);
    }


    public void NowDead()
    {
        direction = Vector2.zero;
        rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        animator.SetBool("isWalking", false);
        am.StopSound("Walk");
        isDead = true;
    }

    [PunRPC]
    public void DisplayDeath(string deadPlayerName)
    {
        GameObject gameoverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
        gameoverPanel.SetActive(true);
        gameoverPanel.transform.Find("gameover Label").gameObject.GetComponent<Text>().text = "GAME OVER";
        gameoverPanel.transform.Find("gameover Reason").gameObject.GetComponent<Text>().text = deadPlayerName + " died";
        
        
        
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().direction = Vector2.zero;
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().animator.SetBool("isWalking", false);
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().am.StopSound("Walk");
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().isDead = true;
        
        
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().direction = Vector2.zero;
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().animator.SetBool("isWalking", false);
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().am.StopSound("Walk");
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().isDead = true;
        
    }

    // [PunRPC]
    // public void SetHealthBar(HPBar bar,uint value)
    // {
    //     bar.SetHealth(value);
    // }
    
    [PunRPC]
    public void SetHealthBar(int value, string barName)
    {
        GameObject bar = GameObject.Find(barName);
        if (value < bar.GetComponent<HPBar>().slider.value)
        {
            bar.GetComponent<HPBar>().SetHealth(value);
        }
        
    }

    private void Flip()
    {
        Debug.Log("Flipped");
        facingRight = !facingRight;
        gameObject.transform.Rotate(0,180,0);
    }
    
    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        // Check the view
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }
        
        // Handle Movement
        if (!isWallSliding)
        {
            Move();
            modifyPhysics();
        }
        else
        {
            am.StopSound("Walk");
            if (nbJump < 2)
            {
                nbJump = 0;
            }
            WallSlide();
        }
        
        // Handle Jump
        if(jumpTimer > Time.time && (nbJump < nbJumpsAllowed || isWallJumping))
        {
            animator.SetBool("isJumping", true);
            animator.SetTrigger("jump");
            
            Jump();
        }
        
        // Handle attack
        if (isAttacking && !isWallSliding)
        {
            animator.SetTrigger("attack");

            // animator.ResetTrigger("attack");
            animator.SetBool("isAttacking", true);
            Attack();
            attackCooldownStatus = ATTACK_COOLDOWN;
            isAttacking = false;
        }

        //Handle dash
        if (isDashing && !isWallSliding)
        {
            animator.SetTrigger("dash");
            Dash();
            dashCooldownStatus = DASH_COOLDOWN;
            am.PlaySound("Dash");
            animator.SetTrigger("enddash");
            
        }

        // Handle switch
        if (isSwitching)
        {
            photonView.RPC("InstantiateSwitch", RpcTarget.All, gameObject.transform.position);
        }
        
        if (Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down,
            0.2f, dangerLayerMask).collider != null)
        {
            TakeDamage(dangerousTilesDmg);
        }

        Collider2D onTouchEnemy = IsTouchingEnemy();
        if (onTouchEnemy != null && onTouchEnemy.gameObject.name != "RoiBlob" && onTouchEnemy.gameObject.transform.parent.name != "RoiBlob")
        {
            TakeDamage(onTouchEnemy.transform.GetComponentInChildren<Enemy>().enemyDamage);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        TakeDamage(10);
    }


    // [PunRPC]
    // private void FocusOnBlob()
    // {
    //     GameObject[] cameras = GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().cameras;
    //     if (cameras[5].activeSelf == false)
    //     {
    //         cameras[4].SetActive(false);
    //         cameras[2].SetActive(false);
    //         cameras[1].SetActive(false);
    //         cameras[0].SetActive(false);
    //         cameras[3].SetActive(false);
    //         cameras[5].SetActive(true);
    //         GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[5].GetComponent<Camera>();
    //         GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[5].GetComponent<Camera>();
    //     }
    // }

    private void LateUpdate()
    {
        if (isDead)
        {
            return;
        }
        
        if (photonView.IsMine)
        {
            if (SceneManager.GetActiveScene().name == "BossRoom")
            {
                if (focusOnKingBlob)
                {
                    cameras[4].SetActive(false);
                    cameras[2].SetActive(false);
                    cameras[1].SetActive(false);
                    cameras[0].SetActive(false);
                    cameras[3].SetActive(false);
                    cameras[5].SetActive(true);
                    GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[5].GetComponent<Camera>();
                    GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[5].GetComponent<Camera>();
                }
                else if (transform.position.x > 184)
                {
                    if (cameras[3].activeSelf == false)
                    {
                        cameras[4].SetActive(false);
                        cameras[2].SetActive(false);
                        cameras[1].SetActive(false);
                        cameras[0].SetActive(false);
                        cameras[3].SetActive(true);
                        cameras[5].SetActive(false);
                        GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[3].GetComponent<Camera>();
                        GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[3].GetComponent<Camera>();
                    }
                }
                
                else if (transform.position.y < -62)
                {
                    if (cameras[2].activeSelf == false)
                    {
                        cameras[4].SetActive(false);
                        cameras[3].SetActive(false);
                        cameras[1].SetActive(false);
                        cameras[0].SetActive(false);
                        cameras[2].SetActive(true);
                        cameras[5].SetActive(false);
                        GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[2].GetComponent<Camera>();
                        GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[2].GetComponent<Camera>();
                    }
                }
                

                else if (transform.position.y < -11)
                {
                    if (cameras[1].activeSelf == false)
                    {
                        cameras[4].SetActive(false);
                        cameras[3].SetActive(false);
                        cameras[2].SetActive(false);
                        cameras[0].SetActive(false);
                        cameras[1].SetActive(true);
                        cameras[5].SetActive(false);
                        GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[1].GetComponent<Camera>();
                        GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[1].GetComponent<Camera>();
                    }
                }
                
                else if (transform.position.x > 82)
                {
                    if (cameras[0].activeSelf == false)
                    {
                        cameras[4].SetActive(false);
                        cameras[3].SetActive(false);
                        cameras[2].SetActive(false);
                        cameras[1].SetActive(false);
                        cameras[0].SetActive(true);
                        cameras[5].SetActive(false);

                        GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[0].GetComponent<Camera>();
                        GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[0].GetComponent<Camera>();
                    }
                }

                else
                {
                    if (cameras[4].activeSelf == false)
                    {
                        
                        cameras[3].SetActive(false);
                        cameras[2].SetActive(false);
                        cameras[1].SetActive(false);
                        cameras[0].SetActive(false);
                        cameras[4].SetActive(true);
                        cameras[5].SetActive(false);

                        GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = cameras[4].GetComponent<Camera>();
                        GameObject.Find("LifeBars").GetComponent<Canvas>().worldCamera = cameras[4].GetComponent<Camera>();
                    }
                }
            }
            if (Camera.current.GetComponent<CameraMovement>().horizontalLeft)
            {
                Camera.current.GetComponent<CameraMovement>().FollowPlayerHorizontallyLeft(gameObject);
            }
            else if (Camera.current.GetComponent<CameraMovement>().horizontalRight)
            {
                Camera.current.GetComponent<CameraMovement>().FollowPlayerHorizontallyRight(gameObject);
            }
            else if (Camera.current.GetComponent<CameraMovement>().verticalUp)
            {
                Camera.current.GetComponent<CameraMovement>().FollowPlayerVerticallyUp(gameObject);
            }
            else if (Camera.current.GetComponent<CameraMovement>().verticalDown)
            {
                Camera.current.GetComponent<CameraMovement>().FollowPlayerVerticallyDown(gameObject);
            }
        }
    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        if (boxCastHit.collider == null)
        { 
            boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
                extraHeightText, dangerLayerMask);
        }
        if (boxCastHit.collider == null)
        { 
            boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
                extraHeightText, laddersLayerMask);
        }
        
        return boxCastHit.collider != null;
    }

    private Vector2 IsTouchingWalls()
    {
        float extraHeightText = 0.2f;
        RaycastHit2D boxCastHitRight = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.right,
            extraHeightText, laddersLayerMask);
        if (boxCastHitRight.collider != null)
            return Vector2.right;
        RaycastHit2D boxCastHitLeft = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.left,
            extraHeightText, laddersLayerMask);
        if (boxCastHitLeft.collider != null)
            return Vector2.left;

        return Vector2.zero;
    }

    private void Move()
    {
        if (onGround)
        {
            
            if (direction == Vector2.left)
            {
                try
                {
                    am.PlaySound("Walk");
                }
                catch
                {
                    
                }
                rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
            }
            else if (direction == Vector2.right)
            {
                try
                {
                    am.PlaySound("Walk");
                }
                catch
                {
                    
                }
                rigidbody2d.velocity = new Vector2(moveSpeed, rigidbody2d.velocity.y);
            }
            else
            {
                try
                {
                    am.StopSound("Walk");
                }
                catch
                {
                    
                }
                // No keys pressed, the player does not slide
                rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
            }
        }
        else
        {
            if (direction == Vector2.left)
            {
                lastInterestingDir = direction;
                rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
            }
            else if (direction == Vector2.right)
            {
                lastInterestingDir = direction;
                rigidbody2d.velocity = new Vector2(moveSpeed, rigidbody2d.velocity.y);
            }
            else
            {
                if (jumpTimer == 0 && isWallJumping)
                {
                    lastInterestingDir = orientation;
                    isWallJumping = false;
                }
                // No keys pressed, the player keeps going in the same direction while falling but slowly
                if (lastInterestingDir == Vector2.left)
                    rigidbody2d.velocity = new Vector2(-moveSpeed/2, rigidbody2d.velocity.y);
                else if(lastInterestingDir == Vector2.right)
                    rigidbody2d.velocity = new Vector2(moveSpeed/2, rigidbody2d.velocity.y);
                else
                    rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y); 
            }
        }
    }

    // public void SetCamToVerticalUp()
    // {
    //     Camera.main.GetComponent<CameraMovement>().FollowPlayerVerticallyUp(gameObject);
    // }
    // public void SetCamToVerticalDown()
    // {
    //     Camera.main.GetComponent<CameraMovement>().FollowPlayerVerticallyDown(gameObject);
    // }
    
    public void Jump()
    {
        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
        if (isWallJumping)
        {
            if (direction == onWall || direction == Vector2.zero)
            {
                Debug.Log("LittlePush");
                rigidbody2d.AddForce(new Vector2(jumpVelocity * -onWall.x * 3f, jumpVelocity), ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("NoPush");
                rigidbody2d.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            }

            // isWallJumping = false;
        }
        else
        {
            Debug.Log("Normal JUmp");
            rigidbody2d.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }
        
        jumpTimer = 0;
        lastInterestingDir = direction;
        try
        {
            am.PlaySound("Jump");

        }
        catch (Exception)
        {
        }
    }

    [PunRPC]
    private void InstantiateSwitch(Vector3 pos)
    {
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().switchCooldownStatus = SWITCH_COOLDOWN;
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().switchCooldownStatus = SWITCH_COOLDOWN;
        GameObject hpBarTop = GameObject.Find("HPBar Top");
        GameObject hpBarBot = GameObject.Find("HPBar Bot");
        Vector3 tmp = hpBarTop.transform.position;
        hpBarTop.transform.position = hpBarBot.transform.position;
        hpBarBot.transform.position = tmp;
        playerTop.GetComponent<TrailRenderer>().time = -1f;
        playerBot.GetComponent<TrailRenderer>().time = -1f;
        
        Vector3 playerTopPos = playerTop.transform.position;
        Vector3 playerBotPos = playerBot.transform.position;
        Instantiate(dashParticleRight, playerTopPos, Quaternion.identity);
        Instantiate(dashParticleRight, playerBotPos, Quaternion.identity);
        
        playerTop.transform.position = playerBot.transform.position;
        playerBot.transform.position = playerTopPos;

        isSwitching = false;
    }
    
    
    private void modifyPhysics()
    {
        // Drag can be used to slow down an object. The higher the drag the more the object slows down.
        rigidbody2d.drag = linearDrag;
        rigidbody2d.gravityScale = gravity * fallMultiplier;
        
    }

    private void WallSlide()
    {
        rigidbody2d.drag = linearDrag;
        rigidbody2d.gravityScale = gravity / 2;
    }

    [PunRPC]
    private void InstantiateParticle(Vector2 dashDir, Vector3 pos)
    {
        if (dashDir == Vector2.right)
            Instantiate(dashParticleRight, pos, Quaternion.identity);
        else if (dashDir == Vector2.left)
            Instantiate(dashParticleLeft, pos, Quaternion.identity);
    }

    private void Dash()
    {
        if (dashTime <= 0)
        {
            isDashing = false;
            rigidbody2d.velocity = Vector2.zero;
        }
        else
        {
            photonView.RPC("InstantiateParticle", RpcTarget.All, dashDirection, gameObject.transform.position);
            dashTime -= Time.deltaTime;
            rigidbody2d.velocity = dashDirection * dashSpeed;
        }
    }

    [PunRPC]
    private void InstantiateAttackParticle(Vector3 pos)
    {
        Instantiate(bloodEffect, pos, Quaternion.identity);
    }

    

    private void Attack()
    {
        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        
        
        // Debug.Log(hitEnemies.Length);
        foreach (var enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.transform.parent.name);
            if (!hasAttacked)
            {
                int enemyHealth =  enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                
                photonView.RPC("InstantiateAttackParticle", RpcTarget.All, enemy.transform.position);
                photonView.RPC("DmgEnemy", RpcTarget.All, enemy.transform.parent.gameObject.name, enemyHealth);
            }
        }
        hasAttacked = true;
    }

    [PunRPC]
    private void DmgEnemy(string enemyName, int enemyHealth)
    {
        if (enemyHealth > 0 )
        {
            GameObject.Find(enemyName).GetComponentInChildren<Enemy>().currentHealth = enemyHealth;
            GameObject bar = GameObject.Find(enemyName + "LifeBar");
            bar.GetComponent<HPBar>().SetHealth(enemyHealth);
        }
        else
        {
            GameObject bar = GameObject.Find("LifeBars").transform.Find(enemyName + "LifeBar").gameObject;
            bar.GetComponent<HPBar>().SetHealth(0);
            Destroy(bar);
        }
        
    }

    [PunRPC]
    private void KillEnemy(string enemyName)
    {
        GameObject bar = GameObject.Find("LifeBars").transform.Find(enemyName + "LifeBar").gameObject;
        bar.GetComponent<HPBar>().SetHealth(0);
        Destroy(bar);
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;
        // Display the attack range in unity
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if(collision.CompareTag("WeakSpot"))
    //     {
    //         Debug.Log(collision.transform.parent.parent.name);
    //         if (transform.position.y > collision.transform.parent.position.y )
    //         {
    //             photonView.RPC("KillEnemy", RpcTarget.All, collision.transform.parent.parent.name);
    //             animator.SetBool("isJumping", true);
    //             animator.SetTrigger("jump");
    //             
    //             Jump();
    //         }
    //     }
    // }
    
    private Collider2D IsTouchingEnemy()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, enemyLayers);
        if (boxCastHit.collider == null)
        {
            boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.up,
                extraHeightText, enemyLayers);
        }
        if (boxCastHit.collider == null)
        {
            boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.right,
                extraHeightText, enemyLayers);
        }
        if (boxCastHit.collider == null)
        {
            boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.left,
                extraHeightText, enemyLayers);
        }
        
        return boxCastHit.collider;
    }
}

