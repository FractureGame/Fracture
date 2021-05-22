﻿using UnityEngine;

public class HarpieAI : MonoBehaviour
{

    private Vector2 pos;
    private GameObject playerTop;
    private GameObject playerBot;
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    private GameObject playerToFollow;
    private Vector2 playerToFollowPos;
    private Vector2 oldposition;
    private Vector2 oldDir;
    private Vector2 direction;
    public float distance;
    public float speedEnemy;
    public int enemyDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        direction = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {

        // Flipping to go to the right direction
        oldposition = transform.position;
        
        playerTopPos = GameObject.Find("PlayerTop(Clone)").GetComponent<Transform>().position;
        playerBotPos = GameObject.Find("PlayerBot(Clone)").GetComponent<Transform>().position;

        if (pos.y > playerBotPos.y && playerTopPos.y > playerBotPos.y)
        {
            playerToFollow = GameObject.Find("PlayerTop(Clone)");
        }
        else if (pos.y > playerTopPos.y && playerBotPos.y > playerTopPos.y)
        {
            playerToFollow = GameObject.Find("PlayerBot(Clone)");
        }
        else if (pos.y < playerTopPos.y && playerBotPos.y < playerTopPos.y)
        {
            playerToFollow = GameObject.Find("PlayerBot(Clone)");
        }
        else if (pos.y < playerBotPos.y && playerTopPos.y < playerBotPos.y)
        {
            playerToFollow = GameObject.Find("PlayerTop(Clone)");
        }

        playerToFollowPos = playerToFollow.GetComponent<Transform>().position;
        if (Vector2.Distance(transform.position, playerToFollowPos) < distance && playerToFollow.GetComponent<PlayerMovement>().isDead == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerToFollowPos, speedEnemy * Time.deltaTime);
        }
        else
        {
            if (Vector2.Distance(transform.position, pos) <= 0)
            {
                // IDLE
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, pos, speedEnemy * Time.deltaTime);
            }
        }
        
        
        
    }

    private void FixedUpdate()
    {
        oldDir = direction;
        if (oldposition.x > transform.position.x)
        {
            direction = Vector2.left;
        }
        else if (oldposition.x < transform.position.x)
        {
            direction = Vector2.right;
        }
        
        if (oldDir != direction && oldDir != Vector2.zero)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}
