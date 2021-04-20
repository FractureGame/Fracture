using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    public GameObject quitText;
    // Start is called before the first frame update
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GameObject.Find("LevelButtons").SetActive(false);
        quitText.SetActive(true);
        PhotonNetwork.LeaveRoom();
            
    }
}
