using System;
using System.Collections;


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


        // public override void OnPlayerEnteredRoom(Player other)
        // {
        //     Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        //
        //
        //     if (PhotonNetwork.IsMasterClient)
        //     {
        //         Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        //
        //
        //         LoadArena();
        //     }
        // }


        // public override void OnPlayerLeftRoom(Player other)
        // {
        //     Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        //     if (PhotonNetwork.IsMasterClient)
        //     {
        //         Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        //
        //
        //         LoadArena();
        //     }
        //     if (!other.IsMasterClient)
        //     {
        //         PhotonNetwork.LeaveRoom();
        //     }
        //
        //
        // }
        
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            PhotonNetwork.AutomaticallySyncScene = false;
            if (PhotonNetwork.IsMasterClient)
            {
                if (other.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(1);
                }
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
                // PhotonNetwork.LoadLevel(0);
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }
        
        
        #endregion

        public void Start()
        {
            gameoverPanel.SetActive(false);
            gameoverReasonLabel.SetActive(false);
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiat
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(this.playerTopPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
            }
            else
            {
                PhotonNetwork.Instantiate(this.playerBotPrefab.name, new Vector3(0f,-5f,0f), Quaternion.identity, 0);
            }
        }
    }
    
    
}