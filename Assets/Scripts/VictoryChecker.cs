using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class VictoryChecker : MonoBehaviour
{

    public Activation victory1;
    public Activation victory2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (victory1.isActivated && victory2.isActivated)
        {
            Victory();
        }
    }
    
    private void Victory()
    {
        // Ajouter des confettis
        GameObject gameOverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
        gameOverPanel.transform.Find("gameover Label").GetComponent<Text>().text = "Congratulations !";
        gameOverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName + " and " + PhotonNetwork.PlayerList[1].NickName + " won";
        gameOverPanel.SetActive(true);
        GameObject.Find("PlayerBot(Clone)").GetComponent<PlayerMovement>().isDead = true;
        GameObject.Find("PlayerTop(Clone)").GetComponent<PlayerMovement>().isDead = true;

    }
}
