using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    
    // private Transform playerTop;
    // private Transform playerBot;
    // private bool follow = false;
    // private float beginning;
    // private float distanceBetweenPlayers;
    // private float cameraSpeed = 10f;
    // private float camPos;
    // private float playerTopPos;
    // private float playerBotPos;
    public float endTilePos;
    // private float maxDistanceBetweenPlayers;
    private bool horizontal;
    public GameObject playerToFollow;
    private float playerPos;
    private string playerName;

    // Start is called before the first frame update
    void Start()
    {
        playerName = playerToFollow.name;
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            horizontal = true;
        }
        else if (SceneManager.GetActiveScene().name[0] == 'V')
        {
            horizontal = false;
        }
    }

    private void Update()
    {
        if (horizontal)
        {
            
        }
        else
        {

        }
        
        
        // if (playerTop == null)
        // {
        //     try
        //     {
        //         playerTop = GameObject.Find("PlayerTop(Clone)").GetComponent<Transform>();
        //     }
        //     catch (NullReferenceException)
        //     {
        //         
        //     }
        // }
        //
        // if (playerBot == null)
        // {
        //     try
        //     {
        //         playerBot = GameObject.Find("PlayerBot(Clone)").GetComponent<Transform>();
        //     }
        //     catch (NullReferenceException)
        //     {
        //         
        //     }
        // }
        //
        // if (playerTop != null && playerBot != null)
        // {
        //     if (horizontal)
        //     {
        //         camPos = transform.position.x;
        //         playerTopPos = playerTop.transform.position.x;
        //         playerBotPos = playerBot.transform.position.x;
        //     }
        //     else
        //     {
        //         camPos = transform.position.y;
        //         playerTopPos = playerTop.transform.position.y;
        //         playerBotPos = playerBot.transform.position.y; 
        //     }
        //     
        //     distanceBetweenPlayers = playerTopPos - playerBotPos;
        //     if (distanceBetweenPlayers < 0)
        //         distanceBetweenPlayers = -distanceBetweenPlayers;
        //
        //     
        //     if (camPos >= endTilePos && (playerTopPos < camPos && playerBotPos < camPos))
        //         follow = true;
        //     
        //     else if (camPos >= endTilePos)
        //         follow = false;
        //     
        //     else if (distanceBetweenPlayers >= maxDistanceBetweenPlayers)
        //         follow = false;
        //     
        //                 
        //     else if (camPos > beginning)
        //     {
        //         follow = true;
        //     }
        //                 
        //     else if (playerTopPos > beginning && playerBotPos > beginning && camPos <= beginning)
        //         follow = true;
        //     
        //     // si les deux joueurs sont à gauche de l'écran au début on ne follow pas
        //     else if (playerTopPos < beginning || playerBotPos < beginning && camPos <= beginning)
        //         follow = false;
        //     
        // }
    }

    void FixedUpdate()
    {
        // if (playerTop != null && playerBot != null)
        // {
        //     Vector3 temp = transform.position;
        //
        //     if (follow)
        //     {
        //         if (horizontal)
        //         {
        //             temp.x = (playerTopPos + playerBotPos) / 2;
        //         }
        //         else
        //         {
        //             temp.y = (playerTopPos + playerBotPos) / 2;
        //         }
        //         
        //         transform.position = Vector3.MoveTowards(transform.position, temp, cameraSpeed * Time.deltaTime); 
        //     }
        // }
    }

    private void LateUpdate()
    {

    }
    
}
