using System.IO;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviourPunCallbacks
{
    public int buildIndex;

    public void LoadLevel()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (SceneManager.GetActiveScene().buildIndex == buildIndex)
        {

            if (!File.Exists("transition.txt"))
            {
                File.Create("transition.txt");
            }
            
            StreamWriter sw = new StreamWriter("transition.txt");
            sw.WriteLine(buildIndex.ToString());
            sw.Close();
            PhotonNetwork.LoadLevel("Transition");
        }
        else
        {
            PhotonNetwork.LoadLevel(buildIndex);
        }
        
        // SceneManager.LoadScene(buildIndex);
    }

}
