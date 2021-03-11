using System;
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
        
        [Tooltip("The prefab to use for representing Enemy1")]
        public GameObject enemy1Prefab;
        [Tooltip("The prefab to use for representing the Enemy2")]
        public GameObject enemy2Prefab;
        
        
        [Tooltip("The prefab to use for representing the Blob")]
        public GameObject blobPrefab;
        
        [Tooltip("The gameover Panel")]
        [SerializeField]
        private GameObject gameoverPanel;
        [Tooltip("The gameover reason Label")]
        [SerializeField]
        private GameObject gameoverReasonLabel;


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
                    gameoverPanel.SetActive(true);
                    gameoverReasonLabel.GetComponent<Text>().text = other.NickName + " left the room";
                    gameoverReasonLabel.SetActive(true);
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
            gameoverPanel.SetActive(false);
            gameoverReasonLabel.SetActive(false);
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene());
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            if (SceneManager.GetActiveScene().name == "MapGame")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(8f, 7f,0f), Quaternion.identity, 0);
                
                    // Instanciate the enemies of the TOP
                    PhotonNetwork.Instantiate(enemy2Prefab.name, new Vector3(enemy2Prefab.transform.position.x, enemy2Prefab.transform.position.y, 0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(8f, 2f,0f), Quaternion.identity, 0);
                
                    // Instanciate the enemies of the BOTTOM
                    PhotonNetwork.Instantiate(enemy1Prefab.name, new Vector3(enemy1Prefab.transform.position.x, enemy1Prefab.transform.position.y, 0f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(blobPrefab.name, new Vector3(blobPrefab.transform.position.x, blobPrefab.transform.position.y, 0f), Quaternion.identity, 0);
                } 
            }
            else if (SceneManager.GetActiveScene().name == "VerticalMapGame")
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
        }
        
    }
    
    
}