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
    private bool isDead;
    public GameObject thisBar;
    public GameObject otherBar;

    [Header("Abilities")]
    public bool canDash;
    public float nbJumpsAllowed = 1;
    
    [Header("Components")]
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    
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
    private bool onGround = false;

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
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;

    [Header("Orientation")]
    public Vector2 orientation = Vector2.right;
    
    [Header("Switch")]
    private GameObject playerTop;
    private GameObject playerBot;
    private bool isSwitching = false;
    private float SWITCH_COOLDOWN = 0.5f;
    public float switchCooldownStatus;

    [Header("WallJump")]
    private Vector2 onWall;
    private bool isWallJumping;
    private bool isWallSliding;

    [Header("Animation")] 
    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        dashTime = startDashTime;
        dashCooldownStatus = 0f;
        animator = GetComponentInChildren<Animator>();
        switchCooldownStatus = 0f;
        



    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
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
            // playerTopsign.transform.rotation = Camera.main.transform.rotation; // Causes the text faces camera.
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
            // playerBotsign.transform.rotation = Camera.main.transform.rotation; // Causes the text faces camera.
            TextMesh tm2 = playerBotsign.AddComponent<TextMesh>();
            tm2.text = PhotonNetwork.PlayerList[1].NickName;
            tm2.color = new Color(0.8f, 0.8f, 0.8f);
            tm2.fontStyle = FontStyle.Bold;
            tm2.alignment = TextAlignment.Center;
            tm2.anchor = TextAnchor.MiddleCenter;
            tm2.characterSize = 0.065f;
            tm2.fontSize = 40;
        }
        
        
        
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
        
        if (isDead)
            return;

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


        if (Input.GetKeyDown(KeyCode.A))
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
        int i = ApplyDamage(dmg);
        photonView.RPC("SetHealthBar",RpcTarget.All,i, thisBar.name);
    }
    
    public int ApplyDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0)
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

    [PunRPC]
    public void DisplayDeath(string deadPlayerName)
    {
        GameObject gameoverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
        gameoverPanel.SetActive(true);
        gameoverPanel.transform.Find("gameover Reason").gameObject.GetComponent<Text>().text = deadPlayerName + " died";
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().isDead = true;
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
        bar.GetComponent<HPBar>().SetHealth(value);
    }

    private void Flip()
    {
        Debug.Log("Flipped");
        facingRight = !facingRight;
        gameObject.transform.Rotate(0,180,0);
    }
    private bool foo(Vector2 pos)
    {
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            if (pos.y > 4f)
            {
                return true;
            }
            
        }
        if (SceneManager.GetActiveScene().name[0] == 'V')
        {
            if (pos.x < 0f)
            {
                return true;
            }
        }
        return false;
    }
    
    private void FixedUpdate()
    {
        if (isDead)
            return;
        
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
            WallSlide();
        }
        
        // Handle Jump
        if(jumpTimer > Time.time && (nbJump < nbJumpsAllowed || isWallJumping))
        {
            animator.SetTrigger("jump");
            animator.SetBool("isJumping", true);
            Jump();
        }
        
        // Handle attack
        if (isAttacking && !isWallSliding)
        {
            animator.SetTrigger("attack");
            Attack();
            isAttacking = false;
        }
        
        //Handle dash
        if (isDashing && !isWallSliding)
        {
            animator.SetTrigger("dash");
            Dash();
            dashCooldownStatus = DASH_COOLDOWN;
            animator.SetTrigger("enddash");
            
        }

        // Handle switch
        if (isSwitching)
        {
            photonView.RPC("InstantiateSwitch", RpcTarget.All, gameObject.transform.position);
        }
    }

    private void LateUpdate()
    {
        if (isDead)
            return;
        
        if (photonView.IsMine)
        {
            if (SceneManager.GetActiveScene().name[0] == 'V')
            {
                Camera.main.GetComponent<VerticalCamera>().FollowPlayer(gameObject);
            }
        }
    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        return boxCastHit.collider != null;
    }

    private Vector2 IsTouchingWalls()
    {
        float extraHeightText = 0.2f;
        RaycastHit2D boxCastHitRight = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.right,
            extraHeightText, platformLayerMask);
        if (boxCastHitRight.collider != null)
            return Vector2.right;
        RaycastHit2D boxCastHitLeft = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.left,
            extraHeightText, platformLayerMask);
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
                rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
            }
            else if (direction == Vector2.right)
            {
                rigidbody2d.velocity = new Vector2(moveSpeed, rigidbody2d.velocity.y);
            }
            else
            {
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
                Debug.Log(lastInterestingDir);
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
    
    private void Jump()
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
        
    }

    [PunRPC]
    private void InstantiateSwitch(Vector3 pos)
    {
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().switchCooldownStatus = SWITCH_COOLDOWN;
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().switchCooldownStatus = SWITCH_COOLDOWN;
        
        Vector3 playerTopPos = playerTop.transform.position;
        Vector3 playerBotPos = playerBot.transform.position;
        Instantiate(dashParticleRight, playerTopPos, Quaternion.identity);
        Instantiate(dashParticleRight, playerBotPos, Quaternion.identity);
        if (foo(playerTopPos) && foo(pos))
        {
            playerTop.transform.position = playerBot.transform.position;
            playerBot.transform.position = playerTopPos;
        }
        else if (foo(playerTopPos) && !foo(pos))
        {
            playerBot.transform.position = playerTop.transform.position;
            playerTop.transform.position = playerBotPos;
        }
        else if (foo(playerBotPos) && foo(pos))
        {
            playerBot.transform.position = playerTop.transform.position;
            playerTop.transform.position = playerBotPos;
        }
        else if (foo(playerBotPos) && !foo(pos))
        {
            playerTop.transform.position = playerBot.transform.position;
            playerBot.transform.position = playerTopPos;
        }
        
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
        Debug.Log(animator.name);
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

    private void Attack()
    {
        // Play an attack animation
        
        
        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        
        // Damage them
        foreach (var enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;
        // Display the attack range in unity
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("WeakSpot"))
        {
            Jump();
        }
    }
}

