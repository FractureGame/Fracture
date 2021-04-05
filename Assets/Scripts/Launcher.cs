using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
    {
        #region Private Serializable Fields

        #endregion


        #region Private Fields
        
        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


        #endregion


        #region Public Fields
        

        #endregion


        #region MonoBehaviour CallBacks
        void Start()
        {
            //Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }


        #endregion


        #region Public Methods


        public void ConnectToMaster()
        {
            if (PhotonNetwork.IsConnected)
            {
                LoadMenus();
            }
            else
            {
                // Connect to Photon servers in Amsterdam (best region)
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
                LoadMenus();
            }

        }

        public void LoadMenus()
        {
            // Load Menus scene
            SceneManager.LoadScene("LobbiesMenu");
        }
        
        #endregion
    
        #region MonoBehaviourPunCallbacks Callbacks
        
        public override void OnConnectedToMaster()
        {
            // LoadMenus();
        }

        #endregion
    }
}