using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


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

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField] private GameObject controlPanel;

        #endregion


        #region MonoBehaviour CallBacks
        void Start()
        {
            // Connect(); Called when pressing the Play Button
            Screen.SetResolution(1000, 425, FullScreenMode.Windowed);
            controlPanel.SetActive(true);
            // backButton.SetActive(false);
        }


        #endregion


        #region Public Methods


        public void ConnectToMaster()
        {
            // isConnecting = PhotonNetwork.ConnectUsingSettings();
            if (PhotonNetwork.IsConnected)
            {
                OnConnectedToMaster();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }

        }

        public void LoadMenus()
        {
            PhotonNetwork.LoadLevel(1);
            // SceneManager.LoadScene("Menus");
        }
        
        #endregion
    
        #region MonoBehaviourPunCallbacks Callbacks
        
        public override void OnConnectedToMaster()
        {
            LoadMenus();
        }

        #endregion
    }
}