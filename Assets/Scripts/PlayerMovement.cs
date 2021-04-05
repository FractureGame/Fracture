﻿using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


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
    
    [Header("Switch")] 
    public GameObject playerTopprefab;
    public GameObject playerBotprefab;
    private GameObject playerTop;
    private GameObject playerBot;
    public GameObject switcherPrefab;
    private GameObject switcher;
    public bool isSwitching = true;
    private Vector2 lastTopPos;

    [Header("WallJump")] 
    private bool onWall;
    private bool isWallJumping;
    [SerializeField] private LayerMask WallsLayerMask;
    
    
    private void Start()
    {
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

        // if (Input.GetKeyDown(KeyCode.S) && gameObject.transform.position.y > 4f)
        // {
        //     Debug.Log("Switching");
        //     PhotonView photonView = PhotonView.Get(this);
        //     photonView.RPC("Switch", RpcTarget.All);
        // }


        if (foo(playerTop.transform.position))
        {
            lastTopPos = playerTop.transform.position;
        }
        else if (foo(playerBot.transform.position))
        {
            lastTopPos = playerBot.transform.position;
        }
        
        if (Input.GetKeyDown(KeyCode.S) && foo(gameObject.transform.position))
        {
            Debug.Log("Switching");
            isSwitching = false;
            Switch();
            if (SceneManager.GetActiveScene().name[0] == 'H')
            {
                PhotonNetwork.Instantiate(switcherPrefab.name, new Vector3(10f, 3f, 0f), Quaternion.identity, 0);
            }
            else
            {
                PhotonNetwork.Instantiate(switcherPrefab.name, new Vector3(2f, 3f, 0f), Quaternion.identity, 0);
            }
        }
        
        onGround = IsGrounded();
        if (onGround)
        {
            nbJump = 0;
        }
            

        onWall = IsTouchingWalls();

        if (onWall && !onGround && Input.GetKeyDown(KeyCode.Space))
        {
            isWallJumping = true;
            jumpTimer = Time.time + jumpDelay;
        }

        // premier input
        // if (Input.GetKey(KeyCode.C) && !onGround && onWall && !isWallSliding)
        // {
        //     Debug.Log("WallSliding");
        //     isWallSliding = true;
        //     if (direction == Vector2.left)
        //     {
        //         direction = Vector2.right;
        //     }
        //     else if (direction == Vector2.right)
        //     {
        //         direction = Vector2.left;
        //     }
        // }
        //
        // if (Input.GetKey(KeyCode.C) && !onGround && onWall && isWallSliding)
        // {
        //     isWallSliding = true;
        // }
        // else
        // {
        //     isWallSliding = false;
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        // {
        //     isWallSliding = false;
        //     jumpTimer = Time.time + jumpDelay;
        // }
        //
        // if (!isWallSliding)
        // {
        //     direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (direction != Vector2.zero)
            orientation = direction;
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
        Move();
        modifyPhysics();
        
        
        // Handle Jump
        if(jumpTimer > Time.time && (nbJump < nbJumpsAllowed || isWallJumping))
        {
            Jump();
            isWallJumping = false;
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

        if (GameObject.Find("Switcher(Clone)") != null && isSwitching)
        {
            gameObject.transform.position = lastTopPos;
            isSwitching = false;
        }


    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        // Debug.Log(boxCastHit.collider);
        return boxCastHit.collider != null;
    }

    private bool IsTouchingWalls()
    {
        float extraHeightText = 0.2f;
        RaycastHit2D boxCastHitRight = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.right,
            extraHeightText, WallsLayerMask);
        
        RaycastHit2D boxCastHitLeft = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.left,
            extraHeightText, WallsLayerMask);
        return boxCastHitRight.collider != null || boxCastHitLeft.collider != null;
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

    // [PunRPC]
    private void Switch()
    {
        Vector3 playerTopPos = playerTop.transform.position;
        Vector3 playerBotPos = playerBot.transform.position;
        if (gameObject.transform.position == playerTopPos)
        {
            gameObject.transform.position = playerBotPos;
        }
        else if (gameObject.transform.position == playerBotPos)
        {
            gameObject.transform.position = playerTopPos;
        }
    }
    
    private void modifyPhysics()
    {
        // Drag can be used to slow down an object. The higher the drag the more the object slows down.
        rigidbody2d.drag = linearDrag;
        rigidbody2d.gravityScale = gravity * fallMultiplier;
        
    }

    // private void WallSlide()
    // {
    //     // Drag can be used to slow down an object. The higher the drag the more the object slows down.
    //     rigidbody2d.drag = linearDrag;
    //     rigidbody2d.gravityScale = gravity;
    // }

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

