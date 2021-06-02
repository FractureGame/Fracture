using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{

    public bool horizontalLeft = false;
    public bool horizontalRight;
    public bool verticalUp;
    public bool verticalDown;
    
    private Transform playerTop;
    private Transform playerBot;
    private bool follow = false;
    private Vector3 beginning;
    private float distanceBetweenPlayers;
    private float cameraSpeed = 10f;

    private float camPos;
    private float playerTopPos;
    private float playerBotPos;

    private bool horizontal;
    // private bool both;
    public float endTilePos;
    private float maxDistanceBetweenPlayers;
    public GameObject[] cameras;

    // Start is called before the first frame update
    void Start()
    {
        // if (SceneManager.GetActiveScene().name[0] == 'H')
        // {
        //     horizontal = true;
        //     maxDistanceBetweenPlayers = 22;
        // }
        // else if (SceneManager.GetActiveScene().name[0] == 'V')
        // {
        //     horizontal = false;
        // }
        // else if (SceneManager.GetActiveScene().name[0] == 'B')
        // {
        //     both = true;
        // }
        // maxDistanceBetweenPlayers = 22;
        beginning = transform.position;
        
        
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
        
            if (horizontalLeft || horizontalRight)
            { 
                camPos = transform.position.x;
                playerTopPos = playerTop.transform.position.x;
                playerBotPos = playerBot.transform.position.x;
        
                
                // Debug.Log(playerTop.transform.position.x);
                distanceBetweenPlayers = playerTopPos - playerBotPos;
                if (distanceBetweenPlayers < 0)
                    distanceBetweenPlayers = -distanceBetweenPlayers;
        
                if (horizontalRight)
                {
                    if (camPos >= endTilePos && (playerTopPos < camPos && playerBotPos < camPos))
                        follow = true;
                
                    else if (camPos >= endTilePos)
                        follow = false;
                
                    else if (distanceBetweenPlayers >= maxDistanceBetweenPlayers)
                        follow = false;
                
                            
                    else if (camPos > beginning.x)
                    {
                        follow = true;
                    }
                            
                    else if (playerTopPos > beginning.x && playerBotPos > beginning.x && camPos <= beginning.x)
                        follow = true;
                
                    // si les deux joueurs sont à gauche de l'écran au début on ne follow pas
                    else if (playerTopPos < beginning.x || playerBotPos < beginning.x && camPos <= beginning.x)
                        follow = false;
                }
                else if (horizontalLeft)
                {
                    if (camPos <= endTilePos && (playerTopPos > camPos && playerBotPos > camPos))
                        follow = true;
                
                    else if (camPos <= endTilePos)
                        follow = false;
                
                    else if (distanceBetweenPlayers >= maxDistanceBetweenPlayers)
                        follow = false;
                
                            
                    else if (camPos < beginning.x)
                    {
                        follow = true;
                    }
                            
                    else if (playerTopPos < beginning.x && playerBotPos < beginning.x && camPos >= beginning.x)
                        follow = true;
                
                    // si les deux joueurs sont à gauche de l'écran au début on ne follow pas
                    else if (playerTopPos > beginning.x || playerBotPos > beginning.x && camPos >= beginning.x)
                        follow = false;
                }
            }
        //
        //     // else if (verticalUp)
        //     // {
        //     //     playerTop.GetComponent<PlayerMovement>().SetCamToVerticalUp();
        //     //     playerBot.GetComponent<PlayerMovement>().SetCamToVerticalUp();
        //     // }
        //     // else if (verticalDown)
        //     // {
        //     //     playerTop.GetComponent<PlayerMovement>().SetCamToVerticalDown();
        //     //     playerBot.GetComponent<PlayerMovement>().SetCamToVerticalDown();
        //     // }
        //
        //     
        }
    }

    void LateUpdate()
    {
        if (horizontalLeft || horizontalRight)
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
    }
    
    public void FollowPlayerHorizontallyRight(GameObject player)
    {
        if (player.transform.position.x > beginning.x)
        {
            Vector3 temp = new Vector3(player.transform.position.x, transform.position.y, -10);
            if (player.transform.position.x > beginning.x)
            {
                transform.position = temp;
            }
            else
            {
                transform.position = new Vector3(beginning.x, beginning.y, -10);
            }
        }
    }
    
    public void FollowPlayerHorizontallyLeft(GameObject player)
    {
        if (player.transform.position.x < beginning.x)
        {
            Vector3 temp = new Vector3(player.transform.position.x, transform.position.y, -10);
            if (player.transform.position.x < beginning.x)
            {
                transform.position = temp;
            }
            else
            {
                transform.position = new Vector3(beginning.x, beginning.y, -10);
            }
        } 
    }
    
    
    public void FollowPlayerVerticallyUp(GameObject player)
    {
        if (player.transform.position.y < endTilePos)
        {
            Vector3 temp = new Vector3(transform.position.x, player.transform.position.y, -10);
            if (player.transform.position.y > beginning.y)
            {
                transform.position = temp;
            }
            else
            {
                transform.position = new Vector3(beginning.x, beginning.y, -10);
            }
        }
    }
    
    public void FollowPlayerVerticallyDown(GameObject player)
    {
        if (player.transform.position.y > endTilePos)
        {
            Vector3 temp = new Vector3(transform.position.x, player.transform.position.y, -10);
            if (player.transform.position.y < beginning.y)
            {
                transform.position = temp;
            }
            else
            {
                transform.position = new Vector3(beginning.x, beginning.y, -10);
            }
        }
    }

    public void FollowPlayer(GameObject player)
    {
        Vector3 temp = new Vector3(player.transform.position.x, player.transform.position.y + 8, -10);
        transform.position = temp;
            
    }
    
    
    
}
