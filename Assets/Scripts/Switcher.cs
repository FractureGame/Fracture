using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Switcher : MonoBehaviour
{
    
    public GameObject playerTopprefab;
    public GameObject playerBotprefab;
    private GameObject playerTop;
    private GameObject playerBot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBot == null)
        {
            playerBot = GameObject.Find("PlayerBot(Clone)");
        }

        if (playerTop == null)
        {
            playerTop = GameObject.Find("PlayerTop(Clone)");
        }
        
        if (playerTop.transform.position.y > 4f || playerBot.transform.position.y > 4f)
        {

            Debug.Log("Destroying switcher");
            PhotonNetwork.Destroy(gameObject);
            if (playerBot.transform.position.y < 4f)
            {
                playerBot.GetComponent<PlayerMovement>().isSwitching = true;
            }
            else if (playerTop.transform.position.y < 4f)
            {
                playerTop.GetComponent<PlayerMovement>().isSwitching = true;
            }
            
        }
    }
}
