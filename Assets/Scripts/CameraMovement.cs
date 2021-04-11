using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    private Transform playerTop;
    private Transform playerBot;
    private bool follow = false;
    private float beginning;
    private float distanceBetweenPlayers;
    private float cameraSpeed = 10f;

    private float camPos;
    private float playerTopPos;
    private float playerBotPos;

    // Start is called before the first frame update
    void Start()
    {
        beginning = transform.position.x;
    }

    private void Update()
    {
        if (playerTop == null)
        {
            try
            {
                playerTop = GameObject.Find("PlayerTop(Clone)").GetComponent<Transform>();
            }
            catch (NullReferenceException)
            {
                
            }
        }

        if (playerBot == null)
        {
            try
            {
                playerBot = GameObject.Find("PlayerBot(Clone)").GetComponent<Transform>();
            }
            catch (NullReferenceException)
            {
                
            }
        }

        if (playerTop != null && playerBot != null)
        {
            
            camPos = transform.position.x;
            playerTopPos = playerTop.transform.position.x;
            playerBotPos = playerBot.transform.position.x;
            
            
            // Debug.Log(playerTop.transform.position.x);
            distanceBetweenPlayers = playerTopPos - playerBotPos;
            if (distanceBetweenPlayers < 0)
                distanceBetweenPlayers = -distanceBetweenPlayers;

            

            // si la caméra dépasse 150 et que les deux joueurs sont dans la partie gauche on continue à follow
            if (camPos >= 158.17 && (playerTopPos < camPos && playerBotPos < camPos))
                follow = true;
            
            else if (camPos >= 158.17)
                follow = false;
            
            else if (distanceBetweenPlayers >= 22)
                follow = false;
            
                        
            else if (camPos > beginning)
            {
                follow = true;
            }
                        
            else if (playerTopPos > beginning && playerBotPos > beginning && camPos <= beginning)
                follow = true;
            
            // si les deux joueurs sont à gauche de l'écran au début on ne follow pas
            else if (playerTopPos < beginning || playerBotPos < beginning && camPos <= beginning)
                follow = false;


            
            // else
            //     follow = false;
        }
    }

    void FixedUpdate()
    {
        if (playerTop != null && playerBot != null)
        {
            Vector3 temp = transform.position;

            if (follow)
            {
                temp.x = (playerTop.transform.position.x + playerBot.transform.position.x) / 2;
                transform.position = Vector3.MoveTowards(transform.position, temp, cameraSpeed * Time.deltaTime); 
            }
        }
    }

    private void LateUpdate()
    {

    }
    
}
