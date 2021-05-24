using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HarpieAI : MonoBehaviour
{
    private Vector2 pos;
    private GameObject playerTop;
    private GameObject playerBot;
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    private GameObject playerToFollow;
    private Vector2 playerToFollowPos;
    private Vector2 posLastFrame;
    private Vector2 posThisFrame;
    private Vector2 direction;
    public float distance;
    public float speedEnemy;
    public Transform[] waypoints;
    private Transform target;
    private bool isPatrolling;
    private int destPoint;
    public int enemyDamage;
    public bool canPatrol;
    public bool facingRight;
    private Vector2 originDir;
    private bool followPlayerTop;
    private bool horizontal;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            horizontal = true;
            Debug.LogFormat("CAMERA Y : {0} vs {1}", GameObject.Find("Main Camera").transform.position.y, pos.y);
            if (pos.y > GameObject.Find("Main Camera").transform.position.y)
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
            horizontal = false;
            if (pos.x > GameObject.Find("Main Camera").transform.position.x)
            {
                followPlayerTop = true;
            }
            else
            {
                followPlayerTop = false;
            }
        }
        target = waypoints[1];
        if (canPatrol)
        {
            isPatrolling = true;
        }
        else
        {
            isPatrolling = false;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speedEnemy * Time.deltaTime);
        }

        if (facingRight)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = Vector2.left;
            transform.Rotate(0, 180, 0);
        }
        originDir = direction;
    }

    
    // Update is called once per frame
    void Update()
    {
        // Flipping to go to the right direction
        posLastFrame = posThisFrame;
        posThisFrame = transform.position;


        playerTopPos = GameObject.Find("PlayerTop(Clone)").GetComponent<Transform>().position;
        playerBotPos = GameObject.Find("PlayerBot(Clone)").GetComponent<Transform>().position;

        

        if (horizontal)
        {
            if (followPlayerTop && playerTopPos.y > playerBotPos.y)
            {
                Debug.Log("That's right");
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


        playerToFollowPos = playerToFollow.GetComponent<Transform>().position;
        
        
        if (isPatrolling)
        {
            Debug.Log("TARGET  ??");
            Debug.Log(target.position);
            transform.position = Vector2.MoveTowards(transform.position, target.position, speedEnemy * Time.deltaTime);
            if(Vector2.Distance(transform.position,target.position)<0.3f)
            {
                destPoint = (destPoint + 1) % waypoints.Length;
                target = waypoints[destPoint];
            }
        }
        
        if (Vector2.Distance(transform.position, playerToFollowPos) < distance && playerToFollow.GetComponent<PlayerMovement>().isDead == false)
        {
            isPatrolling = false;
            transform.position = Vector2.MoveTowards(transform.position, playerToFollowPos, speedEnemy * Time.deltaTime);
        }
        else
        {
            if (Vector2.Distance(transform.position, pos) < 0.3 && canPatrol)
            {
                isPatrolling = true;
                if (direction != originDir && facingRight)
                {
                    transform.Rotate(0, 180, 0);
                    direction = originDir;
                }
            }
            else if (Vector2.Distance(transform.position, pos) < 0.3)
            {
                posThisFrame = posLastFrame;
                if (direction != originDir)
                {
                    transform.Rotate(0, 180, 0);
                    direction = originDir;
                }
            }
            else if (isPatrolling == false)
            {
                transform.position = Vector2.MoveTowards(transform.position, pos, speedEnemy * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        if (posThisFrame == posLastFrame)
        {
            if (direction != originDir)
            {
                transform.Rotate(0, 180, 0);
                direction = originDir;
            }
        }
        else if (posThisFrame.x - posLastFrame.x >= 0.02)
        {
            if (direction != Vector2.right)
            {
                transform.Rotate(0, 180, 0);
                direction = Vector2.right;
            }
        }
        else if (posThisFrame.x - posLastFrame.x <= -0.02)
        {
            if (direction != Vector2.left)
            {
                transform.Rotate(0, 180, 0);
                direction = Vector2.left;
            }
        }
    }
}
