using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The prefab to use for representing the TopPlayer")]
        public GameObject playerTopPrefab;
        [Tooltip("The prefab to use for representing the Botplayer")]
        public GameObject playerBotPrefab;
        
        
        #endregion

        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            // Load Menus
            SceneManager.LoadScene(1);
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            ResumeGame();
            PhotonNetwork.LeaveRoom();
        }
        
        void PauseGame ()
        {
            Time.timeScale = 0;
        }

        void ResumeGame ()
        {
            Time.timeScale = 1;
        }


        #endregion
        
        
        #region Private Methods


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Launcher");
            PhotonNetwork.LoadLevel("Launcher");
        }


        #endregion
        
        #region Photon Callbacks

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
            
            // So that the player leave alone and the player who did not left is not brutally kicked of the game
            PhotonNetwork.AutomaticallySyncScene = false;
            if (PhotonNetwork.IsMasterClient)
            {
                // if the master client leaves
                if (other.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(1);
                }
                // if the slave leaves
                else
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PauseGame();
                    GameObject gameoverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
                    gameoverPanel.SetActive(true);
                    gameoverPanel.transform.Find("gameover Reason").GetComponent<Text>().text = other.NickName + " left the room";
                }
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
            {
                // Back to automatically sync scene
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }
        
        
        #endregion

        public void Start()
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene());
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            if (SceneManager.GetActiveScene().name == "HLevel1")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(8f, 7f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(8f, 2f,0f), Quaternion.identity, 0);
                } 
            }
            else if (SceneManager.GetActiveScene().name == "VLevel2")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(-6f, -3f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(2f, -3f,0f), Quaternion.identity, 0);
                }
            }
            else if (SceneManager.GetActiveScene().name == "HLevel2")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(6f, 7f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(6f, 0f,0f), Quaternion.identity, 0);
                }
            }
            else if (SceneManager.GetActiveScene().name == "VLevel3")
            {
                
            }
        }
    }
}