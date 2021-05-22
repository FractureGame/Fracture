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
        
        public GameObject playerTopPrefab;
        public GameObject playerBotPrefab;
        public GameObject Enemylifebar;
        
        
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
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
            foreach (var enemy in enemies)
            {
                GameObject e = Instantiate(Enemylifebar, GameObject.Find("Canvas").transform);
                e.name = enemy.name + "LifeBar";
                e.tag = "LifeBar";
                e.transform.position = new Vector3(enemy.GetComponentInChildren<BoxCollider2D>().transform.position.x, enemy.transform.position.y + 0.5f, 0);
            }
            
            
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene());
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            if (SceneManager.GetActiveScene().name == "HLevel1")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(8f, 9f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(8f, 2f,0f), Quaternion.identity, 0);
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
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(250f, 5f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(264f, 5f,0f), Quaternion.identity, 0);
                }
            }
        }

        private void Update()
        {
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemies"))
            {
                if (GameObject.Find(enemy.name + "LifeBar") == null)
                {
                    PhotonNetwork.Destroy(enemy);
                }
            }
            
            foreach (var lifebar in GameObject.FindGameObjectsWithTag("LifeBar"))
            {
                if (GameObject.Find(lifebar.name.Replace("LifeBar", "")) == null)
                {
                    PhotonNetwork.Destroy(lifebar);
                }
            }
            
            
        }
    }
}