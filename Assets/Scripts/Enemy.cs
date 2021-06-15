using System;
using Photon.Pun;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isGrounded = false;
    private GameObject lifebar;

    [Header("Damage")]
    public int enemyDamage;
    
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
        try
        {
            rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        }
        catch (Exception)
        {
        }
        
        boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        // lifebar = GameObject.Find("Canvas").transform.Find(transform.parent.name + "LifeBar").gameObject;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        
        // so that the enemy does slide when moving
        try
        {
            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
            modifyPhysics();
        }
        catch (Exception)
        {
        }
        
        


        try
        {
            lifebar = GameObject.Find("LifeBars").transform.Find(transform.parent.name + "LifeBar").gameObject;
            if (transform.parent.name != "RoiBlob")
            {
                lifebar.transform.position = new Vector3(transform.position.x - 1, transform.position.y + 1, 0);
            }
            
        }
        catch (Exception)
        {
            
        }
        
        
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
        if (transform.parent.name.StartsWith("Harpie"))
        {
            GetComponent<HarpieAI>().PushBack();
        }
        
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
        // Destroy(gameObject);
        // PhotonNetwork.Destroy(gameObject);
    }
    
}
