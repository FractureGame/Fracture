using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HarpieAI : MonoBehaviourPunCallbacks
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
    public bool canPatrol;
    public bool canChase = true;
    public bool facingRight;
    private Vector2 originDir;
    private bool followPlayerTop;
    private bool horizontal;

    public bool isStunned = false;
    private Vector2 stunPos;
    private Vector2 endStunPos;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            horizontal = true;
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
        Debug.LogFormat("IsStunned {0}", isStunned);
        if (isStunned == false)
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

            if (playerToFollow == GameObject.Find("PlayerTop(Clone)"))
            {
                try
                {
                    if (photonView.Owner.Equals(PhotonNetwork.PlayerList[0]) == false)
                    {
                        photonView.TransferOwnership(PhotonNetwork.PlayerList[0]);
                    }
                }
                catch (NullReferenceException)
                {
                    photonView.TransferOwnership(PhotonNetwork.PlayerList[0]);
                }

            }
            else
            {
                try
                {
                    if (photonView.Owner.Equals(PhotonNetwork.PlayerList[1]) == false)
                    {
                        photonView.TransferOwnership(PhotonNetwork.PlayerList[1]);
                    }
                }
                catch (NullReferenceException)
                {
                    photonView.TransferOwnership(PhotonNetwork.PlayerList[1]);
                }
            }
            
            playerToFollowPos = playerToFollow.GetComponent<Transform>().position;
            
            
            if (isPatrolling)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speedEnemy * Time.deltaTime);
                if(Vector2.Distance(transform.position,target.position)<0.3f)
                {
                    destPoint = (destPoint + 1) % waypoints.Length;
                    target = waypoints[destPoint];
                }
            }

            if (canChase)
            {
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
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, endStunPos, speedEnemy * 3 * Time.deltaTime);
            if (Vector2.Distance(transform.position, endStunPos) <= 0)
            {
                isStunned = false;
            }
            
        }

    }

    public void PushBack()
    {
        isStunned = true;
        stunPos = transform.position;
        if (direction == Vector2.left)
        {
            endStunPos = new Vector2(stunPos.x + 2, stunPos.y + 1);
        }
        else if (direction == Vector2.right)
        {
            endStunPos = new Vector2(stunPos.x - 2, stunPos.y + 1);
        }
        else
        {
            endStunPos = Vector2.zero;
        }
    }
    
    private void FixedUpdate()
    {

        if (isStunned == false)
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
}
