using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private bool movingToCatsle;
    private bool movingToPhase2;
    private bool phase2;
    private bool movingToPhase3;
    private bool phase3;
    private bool movingToPhase4;
    private bool phase4;
    private bool isMovingToPhase4Playing;
    private bool isPhase4Playing;
    private Coroutine coroutine;
    private Coroutine jumpCoroutine;
    public Tilemap castleTilemap;

    [Header("Abilities")] 
    public GameObject throwBlobsParticle;
    private float throwBlobsCD = 2.5f;
    public GameObject BomberHarpiePrefab;
    public GameObject blobPrefab;
    private float spawnBlob = 0.5f;
    private float jumpCD = 5f;
    private float jumpVelocity = 20f;
    private bool isJumpPlaying;
    private int nbJumpBeforeDestruction = 3;
    private int nbJump;
    private Vector2 pos;
    
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        rigidbody2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        coroutine = StartCoroutine(Phase1());
        // jumpCoroutine = StartCoroutine(Jump());
    }
    
    public IEnumerator Phase1()
    {
        // throw blobs
        yield return new WaitForSeconds(throwBlobsCD);
        PhotonNetwork.Instantiate(throwBlobsParticle.name, transform.position, Quaternion.identity, 1);
    }

    public IEnumerator Jump()
    {
        yield return new WaitForSeconds(jumpCD);
        while (Vector2.Distance(transform.position, new Vector2(transform.position.x, pos.y + 10)) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, pos.y + 5), speed
                * Time.deltaTime);
        }
        
    }
    
    public IEnumerator MovingToPhase4()
    {
        yield return new WaitForSeconds(spawnBlob);
        PhotonNetwork.Instantiate(blobPrefab.name, new Vector2(transform.position.x -4, transform.position.y), Quaternion.identity, 1);
        
    }

    private void Update()
    {
        currentHealth = GetComponent<Enemy>().currentHealth;
        isGrounded = IsGrounded();
        // Debug.LogFormat("TOUCHING LAYERS : {0}", rigidbody2d.IsTouchingLayers(platformLayerMask));
        Debug.LogFormat("isGrounded : {0}", isGrounded);

        if (phase1)
        {
            if (!isJumpPlaying)
            {
                nbJump = 0;
                isJumpPlaying = true;
                jumpCoroutine = StartCoroutine(Jump());
            }
        }
        
        if (currentHealth < 400 && currentHealth > 300 && phase1)
        {
            phase1 = false;
            StopCoroutine(coroutine);
            StopCoroutine(jumpCoroutine);
            movingToPhase2 = true;
            // BomberHarpie
            PhotonNetwork.Instantiate(BomberHarpiePrefab.name, new Vector2(-1, 0), Quaternion.identity, 1);
        }

        if (movingToPhase2)
        {
            if (Vector2.Distance(transform.position, new Vector2(Waypoint1.transform.position.x, transform.position.y)) <= 0)
            {
                movingToCatsle = true;
            }
            else
            {
                transform.position =
                    Vector2.MoveTowards(transform.position, new Vector2(Waypoint1.transform.position.x, transform.position.y), speed * Time.deltaTime);
            }
        }
        
        if (phase2 && nbJump < nbJumpBeforeDestruction)
        {
            if (!isJumpPlaying)
            {
                nbJump = 0;
                isJumpPlaying = true;
                jumpCoroutine = StartCoroutine(Jump());
            }
        }

        if (phase2 && nbJump >= nbJumpBeforeDestruction)
        {
            StopCoroutine(jumpCoroutine);
            // On détruit le château
            Destroy(GameObject.Find("Grid").transform.Find(castleTilemap.name).gameObject);
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
            if (!isMovingToPhase4Playing)
            {
                isMovingToPhase4Playing = true;
                // laisser des blobs derrière, blobs vivants
                coroutine = StartCoroutine(MovingToPhase4());
                
            }
            
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
            StopCoroutine(coroutine);
            isMovingToPhase4Playing = false;
            // WAIT LITTLE BIT
            // CALL THE HARPIES
            // MOVE UP WITH THEM
        }
        
        // rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        // modifyPhysics();
        
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
