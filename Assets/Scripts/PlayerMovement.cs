using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public GameObject thisBar;
    public GameObject otherBar;
    
    public int maxHealth;
    private int currentHealth;
    
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
    public float nbJumpsAllowed = 1;
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


    [Header("WallJump")]
    private Vector2 onWall;
    private bool isWallJumping;
    private bool isWallSliding;

    
    
    private void Start()
    {
        // thisBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        dashTime = startDashTime;
        dashCooldownStatus = 0f;
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

        if (Input.GetKeyDown(KeyCode.Space) && nbJump < nbJumpsAllowed)
        {
            jumpTimer = Time.time + jumpDelay;
            nbJump += 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownStatus <= 0f)
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

        if (Input.GetKeyDown(KeyCode.S) && (foo(playerTop.transform.position) != foo(playerBot.transform.position)))
        {
            Debug.Log("Switching");
            isSwitching = true;
            
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
        }
        else
        {
            isWallSliding = false;
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

        
        if (isWallSliding)
        {
            Vector2 temp = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            if (temp != Vector2.zero)
                direction = temp;
        }
        else
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        }
        
        
        if (direction != oldDirection && direction != Vector2.zero && oldDirection != Vector2.zero)
        {
            Flip();
        }
        
        if (direction != Vector2.zero)
            orientation = direction;

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Damage");
            int i = TakeDamage(20);
            photonView.RPC("SetHealthBar",RpcTarget.All,i, thisBar.name);
        }
    }

    public int TakeDamage(int dmg)
    {
        
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PhotonNetwork.Destroy(gameObject);
        }
        return currentHealth;
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
            Jump();
        }
        
        // Handle attack
        if (isAttacking && !isWallSliding)
        {
            Attack();
            isAttacking = false;
        }
        
        //Handle dash
        if (isDashing && !isWallSliding)
        {
            Dash();
            dashCooldownStatus = DASH_COOLDOWN;
        }

        // Handle switch
        if (isSwitching)
        {
            photonView.RPC("InstantiateSwitch", RpcTarget.All, gameObject.transform.position);
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

            isWallJumping = false;
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

