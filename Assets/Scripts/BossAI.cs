using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Health")]
    private bool isGrounded = false;
    private GameObject lifebar;
    private int currentHealth;

    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Physics")] 
    public float linearDrag;
    public float gravity;
    public float fallMultiplier;

    private float speed = 10;

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


    [Header("Abilities")] 
    public GameObject throwBlobsParticle;
    private float throwBlobsCD = 2.5f;

    private bool isPhase1playing;
    private float throwBlobs;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
    }
    
    public IEnumerator Phase1()
    {
        while(phase1)
        {
            // throw blobs
            PhotonNetwork.Instantiate(throwBlobsParticle.name, transform.position, Quaternion.identity, 1);
            yield return new WaitForSeconds(throwBlobsCD);
        }
    }

    private void Update()
    {
        currentHealth = GetComponent<Enemy>().currentHealth;
        isGrounded = IsGrounded();
        if (rigidbody2d.IsTouchingLayers(platformLayerMask))
        {
            rigidbody2d.isKinematic = true;
        }
        else if (isGrounded == false)
        {
            rigidbody2d.isKinematic = false;
            modifyPhysics();
        }

        if (phase1)
        {
            // chier des blobs toutes les 5 secondes
            if (!isPhase1playing)
            {
                isPhase1playing = true;
                StartCoroutine(Phase1());
            }
        }

        if (phase2)
        {
            
        }
        
        if (currentHealth < 400 && currentHealth > 300 && phase1)
        {
            phase1 = false;
            movingToPhase2 = true;
        }

        if (movingToPhase2)
        {
            // SPAWN blob behind you
            if (Vector2.Distance(transform.position, new Vector2(Waypoint1.transform.position.x, transform.position.y)) <= 0)
            {
                phase2 = true;
                movingToPhase2 = false;
            }
            transform.position =
                Vector2.MoveTowards(transform.position, new Vector2(Waypoint1.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }

        if (phase2 && !isGrounded)
        {
            phase2 = false;
            movingToPhase3 = true;
        }
        

        if (movingToPhase3 && isGrounded)
        {
            if (Vector2.Distance(transform.position, new Vector2(Waypoint2.transform.position.x, transform.position.y)) <= 0)
            {
                movingToPhase3 = false;
                phase3 = true;
            }
            transform.position =
                Vector2.MoveTowards(transform.position, new Vector2(Waypoint2.transform.position.x, transform.position.y), speed * Time.deltaTime);

        }

        if (phase3 && currentHealth < 200)
        {
            movingToPhase4 = true;
            phase3 = false;
        }

        if (movingToPhase4)
        {
            if (Vector2.Distance(transform.position, new Vector2(Waypoint3.transform.position.x, transform.position.y)) <= 0)
            {
                movingToPhase4 = false;
                phase4 = true;
            }
            transform.position =
                Vector2.MoveTowards(transform.position, new Vector2(Waypoint3.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }

        if (phase4)
        {
            // WAIT LITTLE BIT
            // CALL THE HARPIES
            // MOVE UP WITH THEM
        }
        
        rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        
        
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
    
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Platform"))
    //     {
    //         GetComponent<Rigidbody>().isKinematic = true;
    //         GetComponent<Rigidbody>().useGravity = false;
    //         GetComponent<Rigidbody>().velocity = Vector3.zero;
    //         GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    //     }
    //          
    //      
    //
    // }
    //
    //
    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Platform"))
    //     {
    //         // switch to 'non-kinematic'
    //         GetComponent<Rigidbody>().isKinematic = false;
    //         GetComponent<Rigidbody>().useGravity = true;
    //         GetComponent<Rigidbody>().velocity = Vector3.zero; // or another initial value
    //         modifyPhysics();
    //     }
    //
    // }
    
    
}
