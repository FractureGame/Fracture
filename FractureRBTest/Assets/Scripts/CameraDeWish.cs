using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraDeWish : MonoBehaviour
{
    
    private Transform playerTop;
    private Transform playerBot;
    private bool follow = false;
    private float beginning;
    private float distanceBetweenPlayers;
    private float cameraSpeed = 10f;
    // private float moveSpeed = .02f;

    // Start is called before the first frame update
    void Start()
    {
        beginning = transform.position.x;
    }

    private void Update()
    {
        if (playerTop == null)
            playerTop = GameObject.Find("PlayerTop(Clone)").GetComponent<Transform>();
        
        if (playerBot == null)
            playerBot = GameObject.Find("PlayerBot(Clone)").GetComponent<Transform>();

        Debug.Log(playerTop.transform.position.x);
        distanceBetweenPlayers = playerTop.transform.position.x - playerBot.transform.position.x;
        if (distanceBetweenPlayers < 0)
            distanceBetweenPlayers = -distanceBetweenPlayers;
        
        // if (playerTop.transform.position.x > transform.position.x && playerBot.transform.position.x > transform.position.x)
        //     follow = true;
        
        if (transform.position.x >= 142.86 && (playerTop.transform.position.x < transform.position.x && playerBot.transform.position.x < transform.position.x))
            follow = true;
        else if (transform.position.x >= 142.86)
            follow = false;
        
        else if (distanceBetweenPlayers >= 40)
            follow = false;
        
        else if (playerTop.transform.position.x < beginning || playerBot.transform.position.x < beginning && transform.position.x == beginning) 
            follow = false;
        
        else if (playerTop.transform.position.x > beginning && playerBot.transform.position.x > beginning && transform.position.x > beginning)
            follow = true;
        
        else if (playerTop.transform.position.x > beginning && playerBot.transform.position.x > beginning )
            follow = true;
        
        else if (transform.position.x > beginning)
            follow = true;
        
        else
            follow = false;
        
    }

    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 temp = transform.position;

        if (follow)
        {
            temp.x = (playerTop.transform.position.x + playerBot.transform.position.x) / 2;
            transform.position = Vector3.MoveTowards(transform.position, temp, cameraSpeed * Time.deltaTime); 
        }

        // temp.x += moveSpeed;
        // transform.position = temp;
    }
    
}
