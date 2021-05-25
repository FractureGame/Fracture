using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    
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
    public float endTilePos;
    private float maxDistanceBetweenPlayers;
    

    // Start is called before the first frame update
    void Start()
    {
        
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            horizontal = true;
            maxDistanceBetweenPlayers = 22;
        }
        else if (SceneManager.GetActiveScene().name[0] == 'V')
        {
            horizontal = false;
        }
        beginning = transform.position;

    }

    private void Update()
    {
        if (horizontal)
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
        }
    }

    void LateUpdate()
    {
        if (horizontal)
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
    
    public void FollowPlayer(GameObject player)
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
    
    
}
