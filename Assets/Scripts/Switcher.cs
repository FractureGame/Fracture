using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Switcher : MonoBehaviour
{
    private GameObject playerTop;
    private GameObject playerBot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool foo(Vector2 pos)
    {
        if (SceneManager.GetActiveScene().name[0] == 'H')
        {
            if (pos.y > 4f)
            {
                return true;
            }
            
        }
        if (SceneManager.GetActiveScene().name[0] == 'V')
        {
            if (pos.x < 0f)
            {
                return true;
            }

        }

        return false;
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
        
        if (foo(playerTop.transform.position) || foo(playerBot.transform.position))
        {

            Debug.Log("Destroying switcher");
            PhotonNetwork.Destroy(gameObject);
            if (foo(playerBot.transform.position) == false)
            {
                playerBot.GetComponent<PlayerMovement>().isSwitching = true;
            }
            else if (foo(playerTop.transform.position) == false)
            {
                playerTop.GetComponent<PlayerMovement>().isSwitching = true;
            }
            
        }
    }
}
