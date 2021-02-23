using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    
    [Header("Components")]
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    public Vector2 lastInterestingDir;
    private bool facingRight = true;
    private int nbFramesToFullSpeed = 6;
    private int nbFramesToStop = 3;
    
    [Header("Vertical Movement")]
    float jumpVelocity = 10f;
    private float jumpTimer = 0f;
    public float nbJumpsAllowed = 1;
    public float jumpDelay = 0.25f;
    private float nbJump = 0;

    [Header("Collision")] 
    private bool onGround = false;

    [Header("Physics")]
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;
    
    

    private void Awake()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
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
        onGround = IsGrounded();
        if (onGround)
            nbJump = 0;
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
        if(jumpTimer > Time.time && nbJump < nbJumpsAllowed){
            Jump();
        }

        modifyPhysics();
    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        Debug.Log(boxCastHit.collider);
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
                // No keys pressed
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
                // No keys pressed
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

    void modifyPhysics()
    {
        rigidbody2d.gravityScale = gravity;
        rigidbody2d.drag = linearDrag * 0.15f;
        if (rigidbody2d.velocity.y < 0)
        {
            rigidbody2d.gravityScale = gravity * fallMultiplier;
        }
        else if (rigidbody2d.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rigidbody2d.gravityScale = gravity * (fallMultiplier / 2);
        }
    }
}
