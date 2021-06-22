using System;
using System.IO;
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
        public GameObject BossLifeBar;
        private bool hasTransitionned;
        
        
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
            
            // So that the player leaves alone and the player who did not leave is not brutally kicked of the game
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
                Debug.Log(enemy.name);
                if (enemy.name == "RoiBlob")
                {
                    // GameObject e = Instantiate(BossLifeBar, GameObject.Find("Canvas").transform);
                    // e.name = enemy.name + "LifeBar";
                    // e.tag = "LifeBar";
                    // // e.transform.position = BossLifeBar.transform.position;
                    // e.transform.position = new Vector3(enemy.GetComponentInChildren<PolygonCollider2D>().transform.position.x, enemy.transform.position.y + 0.5f, 0);
                }
                else
                {
                    GameObject e = Instantiate(Enemylifebar, GameObject.Find("LifeBars").transform);
                    e.name = enemy.name + "LifeBar";
                    e.tag = "LifeBar";
                    e.transform.position = new Vector3(enemy.GetComponentInChildren<BoxCollider2D>().transform.position.x, enemy.transform.position.y + 0.5f, 0);
                }
                
            }
            
            
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene());
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            if (SceneManager.GetActiveScene().name == "Level1")
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
            else if (SceneManager.GetActiveScene().name == "Level2")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(6f, 11f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(6f, 1f,0f), Quaternion.identity, 0);
                }
            }
            else if (SceneManager.GetActiveScene().name == "Level3")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(6.5f, 2.5f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(22f, 2.5f,0f), Quaternion.identity, 0);
                }
            }
            else if (SceneManager.GetActiveScene().name == "VLevel8")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(6.5f, 2.5f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(22f, 2.5f,0f), Quaternion.identity, 0);
                }
            }
            else if (SceneManager.GetActiveScene().name == "BossRoom")
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(-35, 8.5f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(-35, -5f,0f), Quaternion.identity, 0);
                }
            }
            else if (SceneManager.GetActiveScene().name == "Transition" && PhotonNetwork.PlayerList.Length > 1)
            {

                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Instantiate(playerTopPrefab.name, new Vector3(33, 4f,0f), Quaternion.identity, 0);
                }
                else
                {
                    PhotonNetwork.Instantiate(playerBotPrefab.name, new Vector3(26f, 6f,0f), Quaternion.identity, 0);
                }
            }
        }


        public void DisplayLevels()
        {


            if (PhotonNetwork.IsMasterClient)
            {
                if (GameObject.Find("Change Level").GetComponentInChildren<Text>().text == "Levels")
                {
                    GameObject gameoverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
                    gameoverPanel.SetActive(true);
                    GameObject.Find("gameover Label").GetComponent<Text>().text = "";
                    GameObject.Find("gameover Reason").GetComponent<Text>().text = "";
                    GameObject.Find("Change Level").GetComponentInChildren<Text>().text = "Back";
                }
                else
                {
                    GameObject gameoverPanel = GameObject.Find("Canvas").transform.Find("GameOverPanel").gameObject;
                    gameoverPanel.SetActive(false);
                    GameObject.Find("Change Level").GetComponentInChildren<Text>().text = "Levels";
                }

            }
        }
        
        
        
        
        private void Update()
        {

            // foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemies"))
            // {
            //     if (enemy.GetComponentInChildren<Enemy>().currentHealth <= 0)
            //     {
            //         PhotonNetwork.Destroy(enemy);
            //     }
            //     if (GameObject.Find(enemy.name + "LifeBar") == null)
            //     {
            //         PhotonNetwork.Destroy(enemy);
            //     }
            // }
            
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