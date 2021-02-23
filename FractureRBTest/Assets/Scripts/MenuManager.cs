using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;


namespace Com.MyCompany.MyGame
{
    public class MenuManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        #endregion


        #region Private Fields
        
        private bool publicGame;
        private string value;
        private bool creator;
        private const int CODE_LENGTH = 4;
        private string code;

        #endregion


        #region Public Fields
        
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        
        [Tooltip("The Play Menu")]
        [SerializeField]
        private GameObject menuPlay;

        [Tooltip("Invite Code Label")]
        [SerializeField]
        private GameObject inviteCodeLabel;

        [Tooltip("input Room Name To Join")]
        [SerializeField]
        private GameObject inputRoomNameToJoin;
        
        [Tooltip("Erro Join Label")]
        [SerializeField]
        private GameObject errorJoinLabel;
        
        [Tooltip("Join a Private Room menu")]
        [SerializeField]
        private GameObject joinPrivateGameMenu;
        
        [Tooltip("LaunchGame Button")]
        [SerializeField]
        private GameObject launchGameButton;
        
        [Tooltip("Player2 Label")]
        [SerializeField]
        private GameObject player2Label;
        
        [Tooltip("Master Label")]
        [SerializeField]
        private GameObject masterLabel;
        
        [Tooltip("Back Button")]
        [SerializeField]
        private GameObject backButton;
        
        #endregion
        
        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            progressLabel.SetActive(false);
            menuPlay.SetActive(true);
            backButton.SetActive(true);
            masterLabel.SetActive(false);
            player2Label.SetActive(false);
            launchGameButton.SetActive(false);
            joinPrivateGameMenu.SetActive(false);
            inviteCodeLabel.SetActive(false);
        }

        #endregion


        #region Public Methods

        public void PlayOnline()
        {
            Debug.Log("PLAYONLINE");
            publicGame = true;
            menuPlay.SetActive(false);
            progressLabel.SetActive(true);
            backButton.GetComponentInChildren<Text>().text = "Quit Queue";
            PhotonNetwork.JoinRandomRoom();
        }

        public string GenerateRandomCode()
        {
            code = "";
            Random rand = new Random();
            for (int i = 0; i < CODE_LENGTH; i++)
            {
                code += (char) rand.Next(65, 91);
            }

            return code;
        }

        public void CreatePrivateRoom()
        {
            publicGame = false;
            creator = true;
            backButton.GetComponentInChildren<Text>().text = "Quit Room";
            menuPlay.SetActive(false);
            masterLabel.SetActive(true);
            player2Label.SetActive(true);
            masterLabel.GetComponent<Text>().text = PhotonNetwork.NickName;
            code = GenerateRandomCode();
            PhotonNetwork.CreateRoom(code, new RoomOptions {MaxPlayers = maxPlayersPerRoom, IsVisible = false}, null,
                new string[]{});
        }

        
        public void DisplayJoinPrivateRoom()
        {
            menuPlay.SetActive(false);
            joinPrivateGameMenu.SetActive(true);
        }
        
        public void JoinPrivateRoom()
        {
            errorJoinLabel.GetComponent<Text>().text = "";
            creator = false;
            publicGame = false;
            backButton.GetComponentInChildren<Text>().text = "Quit Room";
            value = inputRoomNameToJoin.GetComponent<Text>().text;
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Room Name is null or empty");
                errorJoinLabel.GetComponent<Text>().text = "Room Name is null or empty";
                
                return;
            }
            PhotonNetwork.JoinRoom(value);
        }

        public void LaunchGame()
        {
            Debug.Log("We load the 'MapGame");
            PhotonNetwork.LoadLevel("MapGame");
        }


        public void Back()
        {
            if (menuPlay.activeSelf)
            {
                SceneManager.LoadScene("Launcher");
            }
            else if(joinPrivateGameMenu.activeSelf)
            {
                joinPrivateGameMenu.SetActive(false);
                menuPlay.SetActive(true);
                errorJoinLabel.GetComponent<Text>().text = "";
            }
            else if (inviteCodeLabel.activeSelf)
            {
                PhotonNetwork.LeaveRoom();
                backButton.GetComponentInChildren<Text>().text = "Back";
                inviteCodeLabel.SetActive(false);
                masterLabel.SetActive(false);
                player2Label.SetActive(false);
                launchGameButton.SetActive(false);
                menuPlay.SetActive(true);
            }
            else if (player2Label.activeSelf && !creator)
            {
                PhotonNetwork.LeaveRoom();
                backButton.GetComponentInChildren<Text>().text = "Back";
                masterLabel.SetActive(false);
                player2Label.SetActive(false);
                menuPlay.SetActive(true);
            }
            else if (publicGame)
            {
                PhotonNetwork.LeaveRoom();
                backButton.GetComponentInChildren<Text>().text = "Back";
                progressLabel.SetActive(false);
                menuPlay.SetActive(true);
            }
        }
        
        #endregion
    
        #region MonoBehaviourPunCallbacks Callbacks
        
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            code = GenerateRandomCode();
            PhotonNetwork.CreateRoom(code, new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null,
                new string[]{});
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log("ONCREATEDROOM");
            if (PhotonNetwork.IsMasterClient && !publicGame)
            {
                inviteCodeLabel.SetActive(true);
                inviteCodeLabel.GetComponent<Text>().text = "CODE : " + PhotonNetwork.CurrentRoom.Name;
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRoomFailed");
            if (!publicGame)
            {
                errorJoinLabel.GetComponent<Text>().text = "Error: Room does not exist";
                DisplayJoinPrivateRoom();
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            if (publicGame)
            {
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            }
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
            if (PhotonNetwork.IsMasterClient && publicGame)
            {
                progressLabel.GetComponent<Text>().text = "Connected. Waiting for my special someone...";
            }
            else if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                menuPlay.SetActive(false);
                masterLabel.SetActive(true);
                player2Label.SetActive(true);
                launchGameButton.SetActive(false);
                masterLabel.GetComponent<Text>().text = PhotonNetwork.NickName;
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !publicGame)
            {
                menuPlay.SetActive(false);
                joinPrivateGameMenu.SetActive(false);
                masterLabel.SetActive(true);
                player2Label.SetActive(true);
                masterLabel.GetComponent<Text>().text = PhotonNetwork.MasterClient.NickName;
                player2Label.GetComponent<Text>().text = PhotonNetwork.NickName;
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("OnPlayerEnteredRoom() called by PUN. Now this client is in a room.");
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2 && publicGame)
            {
                Debug.Log("We load the 'MapGame");
                PhotonNetwork.LoadLevel("MapGame");
            }
            else if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                player2Label.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList[1].NickName;
                launchGameButton.SetActive(true);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !creator)
            {
                creator = true;
                masterLabel.GetComponent<Text>().text = PhotonNetwork.MasterClient.NickName;
                player2Label.GetComponent<Text>().text = "Waiting for my special someone...";
                inviteCodeLabel.GetComponent<Text>().text = "CODE : " + PhotonNetwork.CurrentRoom.Name;
                inviteCodeLabel.SetActive(true);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                player2Label.GetComponent<Text>().text = "Waiting for my special someone...";
            }
        }

        #endregion
    }
}