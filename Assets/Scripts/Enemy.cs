using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isGrounded = false;
    
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Physics")] 
    public float linearDrag;
    public float gravity;
    public float fallMultiplier;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        
        // so that the enemy does slide when moving
        rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        modifyPhysics();
    }
    
    private bool IsGrounded()
    {
        // distance to the ground from which the player can jump again
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        return boxCastHit.collider != null;
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }
        
    }

    void Die()
    {
        Debug.Log("Enemy Died " + gameObject.name);
        
        // Play die animation
        
        // Disable the enemy

        // Really bad : player is thrown outside the map and keeps falling... but at least both players can't see him anymore
        GetComponent<Transform>().position = new Vector2(-5f, -5f);
    }
    
}
