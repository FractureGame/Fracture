using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = System.Random;

public class BossAI : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    private bool isGrounded = false;
    // private GameObject lifebar;
    private int currentHealth;
    public bool isInvincible = false; // triggered when enemy contact

    private Rigidbody2D rigidbody2d;
    private PolygonCollider2D polygonCollider2D;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Physics")] 
    public float linearDrag;
    public float gravity;
    public float fallMultiplier;

    private float speed = 13;

    
    [Header("Timeline")] 
    public GameObject[] waypoints;
    private bool phase1 = true;
    private bool movingToPhase2;
    private bool phase2;
    private bool movingToPhase3;
    private bool phase3;
    private bool isPhase1Playing;
    private Coroutine coroutine;
    public Tilemap castleTilemap;
    public Tilemap castleGround;
    public Tilemap lastTilemap;
    public LayerMask dangerLayerMask;
    public Grid grid;
    public bool isEscaping;
    public bool abdcef = false;

    [Header("Abilities")] 
    public GameObject throwBlobsParticle;
    private float throwBlobsCD = 5f;
    public GameObject BomberHarpiePrefab;
    public GameObject explosionParticle;
    public GameObject blobPrefab;
    private float spawnBlob = 0.5f;


    private Vector2 pos;
    public HarpieAI[] rocketHarpies;
    private bool hasCalledHarpies;
    public float escapeSpeed;
    private bool hasCalledIgnition;
    private bool hasDestroyedlast;
    private bool hasdestroyedCastleAndGround;
    
    [Header("Jump")]
    private float jumpCD = 5f;
    private float jumpCDStatus;
    private float jumpVelocity = 200f;
    private float jumpHeight = 8f;
    private Vector2 jumpDest;
    private bool falling;
    private bool isJumpPlaying;
    private int nbJumpBeforeDestruction = 2;
    private int nbJump;
    private bool isJumping;
    private bool playerNearby;
    
    
    [Header("JumpAttack")]
    public GameObject JumpGroundParticles;
    private float distanceFromPlayer = 25; 

    [Header("Players")]
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    
    [Header("Particles")]
    public ParticleSystem confetti;

    [Header("CastlePhase")] 
    private float actionCD = 2;

    private bool isCastlePhasePlaying;
    
    // Start is called before the first frame update
    void Start()
    {
        jumpCDStatus = 0;
        pos = transform.position;
        rigidbody2d = GetComponent<Rigidbody2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        transform.position = Vector2.MoveTowards(transform.position, transform.position, speed * Time.deltaTime);
        // lifebar = GameObject.Find("Canvas").transform.Find("BossLifeBar").gameObject;
        
        // TEST
        // phase1 = false;
        // movingToPhase2 = true;
        
    }
    
    
    public IEnumerator Phase1()
    {
        // throw blobs
        yield return new WaitForSeconds(throwBlobsCD);
        PhotonNetwork.Instantiate(throwBlobsParticle.name, transform.position, Quaternion.identity, 1);
        isPhase1Playing = false;
    }

    public IEnumerator MovingToPhase4()
    {
        yield return new WaitForSeconds(spawnBlob);
        Instantiate(blobPrefab, new Vector2(transform.position.x -4, transform.position.y), Quaternion.identity);
    }

    [PunRPC]
    private void DestroyTiles()
    {
        Random rand = new Random();
        int prob;
        BoundsInt bounds = castleTilemap.cellBounds;
        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                Vector3Int coordinate = castleTilemap.WorldToCell(new Vector3(x, y-15, 0));
                prob = rand.Next(5);
                if (prob == 0)
                {
                    castleTilemap.SetTile(coordinate, null);
                }
            }
        }
    }


    [PunRPC]
    private void DestroyCastleAndGround()
    {
        try
        {
            Destroy(GameObject.Find("Grid").transform.Find(castleTilemap.name).gameObject);
            Destroy(GameObject.Find("Grid").transform.Find(castleGround.name).gameObject);
        }
        catch (Exception)
        {
        }
    }

    [PunRPC]

    private void DestroyLastTilemap()
    {
        Destroy(GameObject.Find("Grid").transform.Find(lastTilemap.name).gameObject);
    }


    private void ThrowBlobs()
    {
        PhotonNetwork.Instantiate(throwBlobsParticle.name, transform.position, Quaternion.identity, 1);
    }

    private void Jump()
    {
        rigidbody2d.AddForce(jumpVelocity * Vector2.up, ForceMode2D.Impulse);
        falling = true;
        isGrounded = false;
    }
    
    public IEnumerator CastlePhase()
    {
        yield return new WaitForSeconds(actionCD);
        Random rand = new Random();
        int actionCode = rand.Next(2);
        if (actionCode == 0)
        {
            Jump();
        }
        else if (actionCode == 1)
        {
            ThrowBlobs();
        }

        isCastlePhasePlaying = false;

    }
    
    
    private void Update()
    {

        playerTopPos = GameObject.Find("PlayerTop(Clone)").transform.position;
        playerBotPos = GameObject.Find("PlayerBot(Clone)").transform.position;


        playerNearby = Math.Abs(transform.position.x - playerTopPos.x) <= distanceFromPlayer || Math.Abs(transform.position.x - playerBotPos.x) <= distanceFromPlayer;
        

        currentHealth = GetComponent<Enemy>().currentHealth;
        isGrounded = IsGrounded();

        if (playerNearby)
        {
            phase2 = true;
        }

        if (phase2 && nbJump < nbJumpBeforeDestruction)
        {
            if (!isCastlePhasePlaying)
            {
                isCastlePhasePlaying = true;
                coroutine = StartCoroutine(CastlePhase());
            }

            if (falling && isGrounded)
            {
                nbJump += 1;
                photonView.RPC("DestroyTiles", RpcTarget.All);
                PhotonNetwork.Instantiate(JumpGroundParticles.name, new Vector2(0, -5), Quaternion.identity, 1);
                falling = false;
            }
        }
        else if (phase2 && nbJump >= nbJumpBeforeDestruction && isGrounded && !hasdestroyedCastleAndGround)
        {
            photonView.RPC("DestroyCastleAndGround", RpcTarget.All);
            phase2 = false;
            isCastlePhasePlaying = true;
            movingToPhase3 = true;
            isGrounded = false;
            hasdestroyedCastleAndGround = true;
        }
        
        
        if (movingToPhase3 && IsGrounded())
        {
            rigidbody2d.isKinematic = true;
            rigidbody2d.simulated = false;
            if (Math.Abs(transform.position.x - waypoints[1].transform.position.x) <= 0.1)
            {
                // CHECK IF there is a player nearby
                if (playerNearby)
                {
                    movingToPhase3 = false;
                    phase3 = true;
                }
            }
            transform.position =
                Vector2.MoveTowards(transform.position, new Vector2(waypoints[1].transform.position.x, transform.position.y), speed * Time.deltaTime);
        }

        if (phase3)
        {
            
            // Harpies deploy cables
            if (!hasCalledHarpies)
            {
                CallHarpies();
                hasCalledHarpies = true;
            }
            else
            {
                if (!hasCalledIgnition)
                {
                    foreach (var rocketHarpy in rocketHarpies)
                    {
                        rocketHarpy.ignition = true;
                    }
                    hasCalledIgnition = true;
                }
            }
            


            // Everybody moves UP at same speed
            Vector2 dest = new Vector2(transform.position.x, waypoints[3].transform.position.y);


            
            // if all harpies are dead you win, the bitch falls in lava
            if (CheckWinCondition() && !hasDestroyedlast)
            {
                abdcef = true;
                photonView.RPC("callPlayers", RpcTarget.All);

                rigidbody2d.isKinematic = false;
                rigidbody2d.simulated = true;
                if (isGrounded && !hasDestroyedlast)
                {
                    photonView.RPC("DestroyLastTilemap", RpcTarget.All);
                    hasDestroyedlast = true;
                }
            }
            else
            {
                if (hasDestroyedlast)
                {
                    if (isTouchingDanger())
                    {
                        GetComponent<Enemy>().TakeDamage(100);
                    }
                }
                else
                {
                    // MOVE UP WITH THEM
                    // CHeck if one of them is unreachable
                    if (HarpiesEscaped())
                    {
                        isEscaping = true;
                        photonView.RPC("callPlayers2", RpcTarget.All);
                    }

                    transform.position = Vector2.MoveTowards(transform.position, dest, escapeSpeed * Time.deltaTime);
                    
                }
            }
        }
    }

    [PunRPC]
    private void callPlayers()
    {
        GameObject.Find("RoiBlob").GetComponentInChildren<BossAI>().abdcef = true;
    }

    [PunRPC]
    private void callPlayers2()
    {
        GameObject.Find("RoiBlob").GetComponentInChildren<BossAI>().isEscaping = true;
    }

    private bool HarpiesEscaped()
    {
        foreach (var harpy in rocketHarpies)
        {
            try
            {            
                if (harpy.transform.position.y > waypoints[2].transform.position.y)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                
            }

        }

        return false;
    }

    private bool CheckWinCondition()
    {
        foreach (var harpie in rocketHarpies)
        {
            try
            {
                if (harpie.name == "Graphics")
                {
                    return false;
                }
            }
            catch (MissingReferenceException)
            {

            }
        }
        return true;
    }

    private void CallHarpies()
    {
        foreach (var rocketHarpy in rocketHarpies)
        {
            rocketHarpy.transform.Find("Cable").gameObject.GetComponent<LineRenderer>().startWidth = 0.2f;
            rocketHarpy.transform.Find("Cable").gameObject.GetComponent<LineRenderer>().endWidth = 0.2f;
            rocketHarpy.escortBlobKing = true;
        }
    }

    private bool IsGrounded()
    {
        // distance to the ground from which the player can jump again
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(polygonCollider2D.bounds.center, polygonCollider2D.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        return boxCastHit.collider != null;
    }

    private bool isTouchingDanger()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(polygonCollider2D.bounds.center, polygonCollider2D.bounds.size,0f,Vector2.down,
            extraHeightText, dangerLayerMask);
        return boxCastHit.collider != null;
        
    }
}
