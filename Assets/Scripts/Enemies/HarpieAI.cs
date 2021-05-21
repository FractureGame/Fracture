using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpieAI : MonoBehaviour
{

    private Vector2 pos;
    private Vector2 playerTopPos;
    private Vector2 playerBotPos;
    private Vector2 playerToFollow;
    private Vector2 oldposition;
    private SpriteRenderer spriteRenderer;
    public float distance;
    public float speedEnemy;
    public int enemyDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            playerToFollow = playerTopPos;
        }
        else if (pos.y > playerTopPos.y && playerBotPos.y > playerTopPos.y)
        {
            playerToFollow = playerBotPos;
        }
        else if (pos.y < playerTopPos.y && playerBotPos.y < playerTopPos.y)
        {
            playerToFollow = playerBotPos;
        }
        else if (pos.y < playerBotPos.y && playerTopPos.y < playerBotPos.y)
        {
            playerToFollow = playerTopPos;
        }
        
        if (Vector2.Distance(transform.position, playerToFollow) < distance)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerToFollow, speedEnemy * Time.deltaTime);
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
        if (oldposition.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (oldposition.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }
}
