﻿using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isGrounded = false;
    private GameObject lifebar;

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

        rigidbody2d = GetComponent<Rigidbody2D>();

        
        boxCollider2d = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        
        // so that the enemy does slide when moving
 
        rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        modifyPhysics();
        
        // lifebar = GameObject.Find("Canvas").transform.Find(name + "LifeBar").gameObject; 
        // lifebar.transform.position = new Vector3(transform.position.x - 1, transform.position.y + 1, 0);

        
        
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
    }

    public int TakeDamage(int damage)
    {
        currentHealth -= damage;
        lifebar.GetComponent<HPBar>().SetHealth(currentHealth);
        
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }

        return currentHealth;
    }

    void Die()
    {
        Debug.Log("Enemy Died " + gameObject.name);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
