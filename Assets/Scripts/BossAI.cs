using System;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Health")]
    private int maxHealth = 500;
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

    private float speed = 4;

    [Header("Timeline")] 
    public GameObject Waypoint1;
    public GameObject Waypoint2;
    public GameObject Waypoint3;
    private bool phase1 = true;
    private bool movingToPhase2;
    private bool phase2;
    private bool movingToPhase3;
    private bool phase3;
    private bool movingToPhase4;
    private bool phase4;
    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth < 400 && currentHealth > 300)
        {
            phase1 = false;
            movingToPhase2 = true;
        }

        if (movingToPhase2)
        {
            if (Vector2.Distance(transform.position, Waypoint1.transform.position) <= 0)
            {
                phase2 = true;
                movingToPhase2 = false;
            }
            transform.position =
                Vector2.MoveTowards(transform.position, Waypoint1.transform.position, speed * Time.deltaTime);
        }

        if (phase2 && !isGrounded)
        {
            phase2 = false;
            movingToPhase3 = true;
        }

        if (movingToPhase3 && isGrounded)
        {
            movingToPhase3 = false;
            phase3 = true;
        }

        if (phase3 && currentHealth < 200)
        {
            movingToPhase4 = true;
            phase3 = false;
        }

        if (movingToPhase4 && Vector2.Distance(transform.position, Waypoint3.transform.position) <= 0)
        {
            movingToPhase4 = false;
            phase4 = true;
        }
        
        
        isGrounded = IsGrounded();
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

    private void ThrowExplosionBlobs()
    {
        
    }
}
