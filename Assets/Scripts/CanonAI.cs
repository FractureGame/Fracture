using Photon.Pun;
using UnityEngine;

public class CanonAI : MonoBehaviour
{
    private bool horizontal;
    private bool followPlayerTop;
    public float fireRange;
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    private Vector2 playerToFollowPos;
    private GameObject playerToFollow;
    private float angularSpeed = 1;
    private float fireCD = 2;
    private float fireCDStatus;
    public GameObject fireBall;
    public float force = 400;
    public Transform shootPoint;
    private Vector2 direction;

    public bool freeCanon;
    
    // Start is called before the first frame update
    void Start()
    {
        fireCDStatus = fireCD;
        horizontal = Camera.main.GetComponent<CameraMovement>().horizontalLeft || Camera.main.GetComponent<CameraMovement>().horizontalRight || Camera.main.GetComponent<CameraMovement>().horizontalWithMaxDistance;
        if (horizontal)
        {
            if (transform.position.y > GameObject.Find("Main Camera").transform.position.y)
            {
                followPlayerTop = true;
            }
            else
            {
                followPlayerTop = false;
            }
        }
        else
        {
            if (transform.position.x > GameObject.Find("Main Camera").transform.position.x)
            {
                followPlayerTop = true;
            }
            else
            {
                followPlayerTop = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        playerTopPos = GameObject.Find("PlayerTop(Clone)").transform.position;
        playerBotPos = GameObject.Find("PlayerBot(Clone)").transform.position;

        if (freeCanon)
        {
            if (Vector2.Distance(transform.position, playerTopPos) < Vector2.Distance(transform.position, playerBotPos))
            {
                playerToFollow = GameObject.Find("PlayerTop(Clone)");
            }
            else
            {
                playerToFollow = GameObject.Find("PlayerBot(Clone)");
            }

        }
        else
        {
            if (horizontal)
            {
                if (followPlayerTop && playerTopPos.y > playerBotPos.y)
                {
                    playerToFollow = GameObject.Find("PlayerTop(Clone)");
                }
                else if (followPlayerTop && playerTopPos.y < playerBotPos.y)
                {
                    playerToFollow = GameObject.Find("PlayerBot(Clone)");
                }
                else if (!followPlayerTop && playerTopPos.y > playerBotPos.y)
                {
                    playerToFollow = GameObject.Find("PlayerBot(Clone)");
                }
                else if (!followPlayerTop && playerTopPos.y < playerBotPos.y)
                {
                    playerToFollow = GameObject.Find("PlayerTop(Clone)");
                }
            }
            else
            {
                if (followPlayerTop && playerTopPos.x > playerBotPos.x)
                {
                    playerToFollow = GameObject.Find("PlayerTop(Clone)");
                }
                else if (followPlayerTop && playerTopPos.x < playerBotPos.x)
                {
                    playerToFollow = GameObject.Find("PlayerBot(Clone)");
                }
                else if (!followPlayerTop && playerTopPos.x > playerBotPos.x)
                {
                    playerToFollow = GameObject.Find("PlayerBot(Clone)");
                }
                else if (!followPlayerTop && playerTopPos.x < playerBotPos.x)
                {
                    playerToFollow = GameObject.Find("PlayerTop(Clone)");
                }
            }
        

            
        }
        playerToFollowPos = playerToFollow.transform.position;
        
        




        if (Vector2.Distance(transform.position, playerToFollowPos) <= fireRange && playerToFollow.GetComponent<PlayerMovement>().isDead == false)
        {
            // Aim at the player (change rotation)
            var dir = playerToFollowPos - (Vector2)transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            direction = playerToFollowPos - (Vector2) transform.position;

            // Shoot him
            if (fireCDStatus <= 0)
            {
                // Shoot    
                Fire();
                fireCDStatus = fireCD;

            }
            else
            {
                fireCDStatus -= Time.deltaTime;
            }
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(fireBall, shootPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(direction.normalized * force);
    }
}
