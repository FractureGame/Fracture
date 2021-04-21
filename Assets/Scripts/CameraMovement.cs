using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private bool horizontal;
    public float endTilePos;
    private float maxDistanceBetweenPlayers;

    // Start is called before the first frame update
    void Start()
    {
        
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            horizontal = true;
            maxDistanceBetweenPlayers = 22;
            beginning = transform.position.x;
        }
        else if (SceneManager.GetActiveScene().name[0] == 'V')
        {
            horizontal = false;
            maxDistanceBetweenPlayers = 22;
            beginning = transform.position.y;
        }
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
            if (horizontal)
            {
                camPos = transform.position.x;
                playerTopPos = playerTop.transform.position.x;
                playerBotPos = playerBot.transform.position.x;
            }
            else
            {
                camPos = transform.position.y;
                playerTopPos = playerTop.transform.position.y;
                playerBotPos = playerBot.transform.position.y;
            }
            
            
            
            // Debug.Log(playerTop.transform.position.x);
            distanceBetweenPlayers = playerTopPos - playerBotPos;
            if (distanceBetweenPlayers < 0)
                distanceBetweenPlayers = -distanceBetweenPlayers;

            

            // si la caméra dépasse 150 et que les deux joueurs sont dans la partie gauche on continue à follow
            if (camPos >= endTilePos && (playerTopPos < camPos && playerBotPos < camPos))
                follow = true;
            
            else if (camPos >= endTilePos)
                follow = false;
            
            else if (distanceBetweenPlayers >= maxDistanceBetweenPlayers)
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
                if (horizontal)
                {
                    temp.x = (playerTop.transform.position.x + playerBot.transform.position.x) / 2;
                }
                else
                {
                    temp.y = (playerTop.transform.position.y + playerBot.transform.position.y) / 2;
                }
                
                transform.position = Vector3.MoveTowards(transform.position, temp, cameraSpeed * Time.deltaTime); 
            }
        }
    }

    private void LateUpdate()
    {

    }
    
}
