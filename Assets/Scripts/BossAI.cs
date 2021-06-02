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

    private Rigidbody2D rigidbody2d;
    private PolygonCollider2D polygonCollider2D;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Physics")] 
    public float linearDrag;
    public float gravity;
    public float fallMultiplier;

    private float speed = 10;

    
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
    
    [Header("Jump")]
    private float jumpCD = 5f;
    private float jumpCDStatus;
    private float jumpVelocity = 20f;
    private float jumpHeight = 8f;
    private Vector2 jumpDest;
    private bool falling;
    private bool isJumpPlaying;
    private int nbJumpBeforeDestruction = 3;
    private int nbJump;
    private bool isJumping;
    private bool playerNearby;
    
    
    [Header("JumpAttack")]
    public GameObject JumpGroundParticles;
    private float distanceFromPlayer = 15; 

    [Header("Players")]
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    
    // Start is called before the first frame update
    void Start()
    {
        jumpCDStatus = 0;
        pos = transform.position;
        rigidbody2d = GetComponent<Rigidbody2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        transform.position = Vector2.MoveTowards(transform.position, transform.position, speed * Time.deltaTime);
        // lifebar = GameObject.Find("Canvas").transform.Find("BossLifeBar").gameObject;
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
                    // Vector3 pos = castleTilemap.CellToWorld(coordinate);
                    // prob = rand.Next(15);
                    // if (prob == 0)
                    // {
                    //     PhotonNetwork.Instantiate(explosionParticle.name, pos, Quaternion.identity, 1);
                    // }
                }
            }
        }
    }


    [PunRPC]
    private void DestroyCastleAndGround()
    {
        Destroy(GameObject.Find("Grid").transform.Find(castleTilemap.name).gameObject);
        Destroy(GameObject.Find("Grid").transform.Find(castleGround.name).gameObject);
    }

    [PunRPC]

    private void DestroyLastTilemap()
    {
        Destroy(GameObject.Find("Grid").transform.Find(lastTilemap.name).gameObject);
    }
    
    private void Update()
    {
        playerTopPos = GameObject.Find("PlayerTop(Clone)").transform.position;
        playerBotPos = GameObject.Find("PlayerBot(Clone)").transform.position;


        playerNearby = Math.Abs(transform.position.x - playerTopPos.x) <= distanceFromPlayer || Math.Abs(transform.position.x - playerBotPos.x) <= distanceFromPlayer;

        
        

        if (movingToPhase2)
        {
            rigidbody2d.isKinematic = true;
        }

        currentHealth = GetComponent<Enemy>().currentHealth;
        isGrounded = IsGrounded();
        Debug.LogFormat("isGrounded : {0}", isGrounded);

        if (phase1)
        {
            if (!isPhase1Playing)
            {
                isPhase1Playing = true;

                coroutine = StartCoroutine(Phase1());
                
            }

            if (jumpCDStatus <= 0 && isGrounded && playerNearby)
            {
                // Random rand = new Random();
                // int n = rand.Next(2);
                // int x = rand.Next(5);
                // if (n == 0)
                // {
                //     x = -x;
                // }
                jumpDest = new Vector2(transform.position.x , transform.position.y + jumpHeight);
                isJumping = true;
            }
            else if (jumpCDStatus > 0 && isGrounded)
            {
                jumpCDStatus -= Time.deltaTime;
            }

            if (isJumping)
            {
                if (Vector2.Distance(transform.position, jumpDest) <= 1)
                {
                    jumpCDStatus = jumpCD;
                    rigidbody2d.isKinematic = false;
                    falling = true;
                    isJumping = false;
                }
                else
                {
                    rigidbody2d.isKinematic = true;
                    transform.position = Vector2.MoveTowards(transform.position, jumpDest, jumpVelocity * Time.deltaTime);
                }
            }

            if (falling && isGrounded)
            {
                // Show the attack range with particules
                PhotonNetwork.Instantiate(JumpGroundParticles.name, new Vector2(0, -5),
                    Quaternion.identity, 1);

                falling = false;
            }
            

        }
        
        if (currentHealth < 400 && currentHealth > 300 && phase1 && isGrounded)
        {
            phase1 = false;
            StopCoroutine(coroutine);
            isPhase1Playing = false;
            movingToPhase2 = true;
            // BomberHarpie
            PhotonNetwork.Instantiate(BomberHarpiePrefab.name, new Vector2(-27, 0), Quaternion.identity, 1);
        }

        if (movingToPhase2)
        {
            if (Vector2.Distance(transform.position, new Vector2(waypoints[0].transform.position.x, transform.position.y)) <= 0)
            {
                movingToPhase2 = false;
                nbJump = 0;
                phase2 = true;
            }
            else
            {
                transform.position =
                    Vector2.MoveTowards(transform.position, new Vector2(waypoints[0].transform.position.x, transform.position.y), speed * Time.deltaTime);
            }
        }
        
        if (phase2 && nbJump < nbJumpBeforeDestruction)
        {
            if (jumpCDStatus <= 0 && isGrounded && playerNearby)
            {
                jumpDest = new Vector2(transform.position.x, transform.position.y + jumpHeight);
                isJumping = true;
            }
            else if (jumpCDStatus > 0 && isGrounded)
            {
                jumpCDStatus -= Time.deltaTime;
            }

            if (isJumping)
            {
                if (Vector2.Distance(transform.position, jumpDest) <= 1)
                {
                    falling = true;
                    jumpCDStatus = jumpCD;
                    rigidbody2d.isKinematic = false;
                    nbJump += 1;
                    isJumping = false;
                }
                else
                {
                    rigidbody2d.isKinematic = true;
                    transform.position = Vector2.MoveTowards(transform.position, jumpDest, jumpVelocity * Time.deltaTime);
                }
            }
            if (falling && isGrounded)
            {
                photonView.RPC("DestroyTiles", RpcTarget.All);
                PhotonNetwork.Instantiate(JumpGroundParticles.name, new Vector2(transform.position.x, transform.position.y - 3),
                    Quaternion.identity, 1);

                falling = false;
            }
            
            
        }
        else if (phase2 && nbJump >= nbJumpBeforeDestruction && rigidbody2d.IsTouchingLayers(platformLayerMask))
        {
            rigidbody2d.isKinematic = false;
            photonView.RPC("DestroyCastleAndGround", RpcTarget.All);
            phase2 = false;
            movingToPhase3 = true;
            
            
        }
        
        
        

        if (movingToPhase3 && isGrounded)
        {
            rigidbody2d.isKinematic = true;
            if (Vector2.Distance(transform.position, new Vector2(waypoints[1].transform.position.x, transform.position.y)) <= 0)
            {
                movingToPhase3 = false;
                phase3 = true;
            }
            transform.position =
                Vector2.MoveTowards(transform.position, new Vector2(waypoints[1].transform.position.x, transform.position.y), speed * Time.deltaTime);
        }

        if (phase3)
        {
            rigidbody2d.isKinematic = true;
            // StopCoroutine(coroutine);
            // CALL THE HARPIES
            if (!hasCalledHarpies)
            {
                CallHarpies();
                hasCalledHarpies = true;
            }

            if (IsReady())
            {
                Vector2 dest = new Vector2(transform.position.x, waypoints[2].transform.position.y);
                
                
                // if all harpies are dead you win, the bitch falls in lava
                if (CheckWinCondition())
                {
                    phase3 = false;
                    rigidbody2d.isKinematic = false;
                    // False into the lava
                    if (isGrounded && !hasDestroyedlast)
                    {
                        photonView.RPC("DestroyLastTilemap", RpcTarget.All);
                        hasDestroyedlast = true;
                    }

                    if (isTouchingDanger())
                    {
                        GameObject gameOverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
                        gameOverPanel.transform.Find("gameover Label").GetComponent<Text>().text = "Congratulations !";
                        gameOverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName + " and " + PhotonNetwork.PlayerList[1].NickName + " won";
                        gameOverPanel.SetActive(true);
                        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().NowDead();
        
                        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().NowDead();
                    }
                    
                    
                }
                
                
                
                // MOVE UP WITH THEM
                if (Vector2.Distance(transform.position, dest) <= 0)
                {
                    // YOU LOSE
                    GameObject gameOverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
                    gameOverPanel.transform.Find("gameover Label").GetComponent<Text>().text = "Game over !";
                    gameOverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = "The Blob king escaped !";
                    gameOverPanel.SetActive(true);
                    GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().NowDead();
        
                    GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().NowDead();
                    
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, dest, escapeSpeed * Time.deltaTime);
                }

                if (!hasCalledIgnition)
                {
                    foreach (var rocketHarpy in rocketHarpies)
                    {
                        rocketHarpy.ignition = true;
                    }
                    hasCalledHarpies = true;
                }
            }
        }
        
        // rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        // modifyPhysics();
        
    }


    private bool CheckWinCondition()
    {
        foreach (var harpie in rocketHarpies)
        {
            if (GameObject.Find(harpie.name) != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsReady()
    {
        foreach (var rocketHarpy in rocketHarpies)
        {
            if (rocketHarpy.readyToCarryKingBlob == false)
            {
                return false;
            }
        }
        return true;
    }
    private void CallHarpies()
    {
        foreach (var rocketHarpy in rocketHarpies)
        {
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
