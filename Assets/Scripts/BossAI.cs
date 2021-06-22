using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = System.Random;

public class BossAI : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    private bool isGrounded;
    // private GameObject lifebar;
    private int currentHealth;
    public bool isInvincible; // triggered when enemy contact

    private Rigidbody2D rigidbody2d;
    private PolygonCollider2D polygonCollider2D;
    [SerializeField] private LayerMask platformLayerMask;
    
    [Header("Physics")] 
    private float linearDrag = 1;
    private float gravity = 1;
    private float fallMultiplier = 9;
    private float speed = 13;

    
    [Header("Timeline")] 
    public GameObject[] waypoints;
    private bool movingToPhase2;
    private bool phase2;
    private bool movingToPhase3;
    private bool phase3;
    private Coroutine coroutine;
    public Tilemap castleTilemap;
    public Tilemap castleGround;
    public Tilemap lastTilemap;
    public LayerMask dangerLayerMask;
    public bool isEscaping;
    public bool abdcef = false;

    public HarpieAI[] rocketHarpies;
    private bool hasCalledHarpies;
    public float escapeSpeed;
    private bool hasCalledIgnition;
    private bool hasDestroyedlast;
    private bool hasdestroyedCastleAndGround;
    
    [Header("Jump")]
    private float jumpVelocity = 40;
    private bool falling;
    private bool isJumpPlaying;
    private int nbJumpBeforeDestruction = 2;
    private int nbJump;
    private bool isJumping;
    private bool playerNearby;
    
    
    [Header("JumpAttack")]
    public GameObject JumpGroundParticles;
    private float distanceFromPlayer = 25;
    private Shake shake;

    [Header("Players")] private GameObject playerTop;
    private GameObject playerBot;
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    
    [Header("Particles")]
    public ParticleSystem confetti;

    [Header("CastlePhase")] 
    private float actionCD = 0;
    private bool isCastlePhasePlaying;
    private bool started;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        transform.position = Vector2.MoveTowards(transform.position, transform.position, speed * Time.deltaTime);
        shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<Shake>();
    }
    
    private void modifyPhysics()
    {
        // Drag can be used to slow down an object. The higher the drag the more the object slows down.
        rigidbody2d.drag = linearDrag;
        rigidbody2d.gravityScale = gravity * fallMultiplier;
        
    }
    
    // [PunRPC]
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
            GameObject.Find("PlayerTop(Clone)").GetComponentInChildren<Animator>().SetBool("isJumping", true);
            GameObject.Find("PlayerTop(Clone)").GetComponentInChildren<Animator>().SetTrigger("jump");
            GameObject.Find("PlayerBot(Clone)").GetComponentInChildren<Animator>().SetBool("isJumping", true);
            GameObject.Find("PlayerBot(Clone)").GetComponentInChildren<Animator>().SetTrigger("jump");
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
    private void Jump()
    {
        rigidbody2d.AddForce(jumpVelocity * Vector2.up, ForceMode2D.Impulse);
    }
    
    public IEnumerator CastlePhase()
    {

        yield return new WaitForSeconds(actionCD);
        if (actionCD == 0)
        {
            actionCD += 2;
        }


        if (transform.position.y > -5)
        {
            Jump();
            isJumping = true;
        }

        
        isCastlePhasePlaying = false;

    }


    [PunRPC]
    private void InstanciateGroundParticles(string groundParticlesName, float x, float y)
    {
        PhotonNetwork.Instantiate(groundParticlesName, new Vector2(x, y), Quaternion.identity, 1);
    }

    [PunRPC]
    private void ShakeCamera1()
    {
        GameObject cam1 = GameObject.Find("Camera1");
        GameObject shakeManager = GameObject.Find("ShakeManager");
        shakeManager.GetComponent<Shake>().camAnim = cam1.GetComponent<Animator>();
        shakeManager.GetComponent<Shake>().CamShake();
    }

    
    
    private void Update()
    {

        playerTop = GameObject.Find("PlayerTop(Clone)");
        playerBot = GameObject.Find("PlayerBot(Clone)");
        playerTopPos = playerTop.transform.position;
        playerBotPos = playerBot.transform.position;


        if (transform.position.y > -10)
        {
            playerNearby = Math.Abs(transform.position.x - playerTopPos.x) <= 20 || Math.Abs(transform.position.x - playerBotPos.x) <= 20;
        }
        else
        {
            playerNearby = Math.Abs(transform.position.x - playerTopPos.x) <= 25 || Math.Abs(transform.position.x - playerBotPos.x) <= 25;
        }
        
        

        currentHealth = GetComponent<Enemy>().currentHealth;
        isGrounded = IsGrounded();

        if (playerNearby && !started)
        {
            movingToPhase2 = true;
            started = true;
            GameObject objective = GameObject.Find("Canvas").transform.Find("ObjImage").transform.Find("Objective")
                .gameObject;
            objective.GetComponent<TextMeshProUGUI>().text = "Objective : Chase Blob King";

        }

        if (movingToPhase2)
        {
            if (Math.Abs(transform.position.x - waypoints[0].transform.position.x) > 0)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(waypoints[0].transform.position.x, transform.position.y), speed * Time.deltaTime);
            }
            else
            {
                movingToPhase2 = false;
                phase2 = true;
            }
        }

        if (phase2 && nbJump < nbJumpBeforeDestruction)
        {
            if (!isCastlePhasePlaying)
            {
                isCastlePhasePlaying = true;
                coroutine = StartCoroutine(CastlePhase());
            }

            if (transform.position.y > 0)
            {
                falling = true;
            }
            
            if (isGrounded && falling)
            {
                nbJump += 1;
                DestroyTiles();
                // photonView.RPC("DestroyTiles", RpcTarget.All);
                photonView.RPC("InstanciateGroundParticles", RpcTarget.All, JumpGroundParticles.name, transform.position.x, transform.position.y);
                // PhotonNetwork.Instantiate(JumpGroundParticles.name, transform.position, Quaternion.identity, 1);
                falling = false;
                
                // Shake Camera
                // photonView.RPC("ShakeCamera1", RpcTarget.All);
                shake.camAnim = GameObject.Find("Camera1").GetComponent<Animator>();
                shake.CamShake();
                


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
                    GameObject objective = GameObject.Find("Canvas").transform.Find("ObjImage").transform.Find("Objective")
                        .gameObject;
                    objective.GetComponent<TextMeshProUGUI>().text = "Objective : \nKill the harpies to stop the king from escaping";
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
                    // Shake Camera
                    shake.camAnim = GameObject.Find("CameraOnBlob").GetComponent<Animator>();
                    shake.CamShake();
                    
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
                        GetComponent<Enemy>().TakeDamage(167);
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
        modifyPhysics();
        
    }

    [PunRPC]
    private void callPlayers()
    {
        GameObject.Find("RoiBlob").GetComponentInChildren<BossAI>().abdcef = true;
        GameObject.Find("RoiBlob").GetComponentInChildren<Rigidbody2D>().isKinematic = false;
        GameObject.Find("RoiBlob").GetComponentInChildren<Rigidbody2D>().simulated = true;
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
