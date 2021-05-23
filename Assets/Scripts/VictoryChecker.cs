using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class VictoryChecker : MonoBehaviour
{

    public Activation victory1;
    public Activation victory2;

    public ParticleSystem confetti;
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
        // Ajouter des confettis/fireworks
        // Instantiate(confetti, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + Camera.main.orthographicSize, 0), Quaternion.identity);

        Instantiate(confetti, victory1.transform.position, Quaternion.identity);
        Instantiate(confetti, victory2.transform.position, Quaternion.identity);
        
        GameObject gameOverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
        gameOverPanel.transform.Find("gameover Label").GetComponent<Text>().text = "Congratulations !";
        gameOverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName + " and " + PhotonNetwork.PlayerList[1].NickName + " won";
        gameOverPanel.SetActive(true);
        Destroy(GameObject.Find("PlayerBot(Clone)"));
        Destroy(GameObject.Find("PlayerTop(Clone)"));
        

    }
}
