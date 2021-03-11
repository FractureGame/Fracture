using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;


public class PlayerMovement : MonoBehaviourPunCallbacks
{
    
    [Header("Components")]
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    private Vector2 direction;
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
    
    [Header("Attack")]
    private bool isAttacking = false;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;

    [Header("Orientation")]
    public Vector2 orientation = Vector2.right;
    
    
    private void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        dashTime = startDashTime;
        dashCooldownStatus = 0f;
    }
    
    private void Update()
    {
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



        onGround = IsGrounded();
        if (onGround)
            nbJump = 0;
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (direction != Vector2.zero)
            orientation = direction;
    }

    private void FixedUpdate()
    {
        // Check the view
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }
        
        // Handle Movement
        Move();
        
        // Handle Jump
        if(jumpTimer > Time.time && nbJump < nbJumpsAllowed)
        {
            Jump();
        }
        
        // Handle attack
        if (isAttacking)
        {
            Attack();
            isAttacking = false;
        }
        
        //Handle dash
        if (isDashing)
        {
            Dash();
            dashCooldownStatus = DASH_COOLDOWN;
        }

        modifyPhysics();
    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        // Debug.Log(boxCastHit.collider);
        return boxCastHit.collider != null;
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
        rigidbody2d.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        jumpTimer = 0;
        lastInterestingDir = direction;
    }

    private void modifyPhysics()
    {
        rigidbody2d.gravityScale = gravity;
        
        // Drag can be used to slow down an object. The higher the drag the more the object slows down.
        rigidbody2d.drag = linearDrag;
        rigidbody2d.gravityScale = gravity * fallMultiplier;
        
        // rigidbody2d.drag = linearDrag * 0.15f;
        // // si le joueur descends, la force de gravité le fait descendre plus vite
        // if (rigidbody2d.velocity.y < 0)
        // {
        //     rigidbody2d.gravityScale = gravity * fallMultiplier;
        // }
        // // si le joueur monte et que l'on maintient Jump, il flotte
        // else if (rigidbody2d.velocity.y > 0 && !Input.GetButton("Jump"))
        // {
        //     rigidbody2d.gravityScale = gravity * (fallMultiplier / 2);
        // }
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

