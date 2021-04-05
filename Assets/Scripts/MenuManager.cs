using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
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
        private string sceneName = "HLevel1";

        #endregion


        #region Public Fields
        

        [Tooltip("Room Code Label")]
        [SerializeField]
        private GameObject inviteCodeLabel;
        [Tooltip("input Room Name To Join")]
        [SerializeField]
        private GameObject inputRoomNameToJoin;

        [Tooltip("LaunchGame Button")]
        [SerializeField]
        private GameObject launchGameButton;

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
        

        #endregion


        #region Public Methods

        public void PlayOnline()
        {
            Debug.Log("PLAYONLINE");
            publicGame = true;
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
            code = GenerateRandomCode();
            PhotonNetwork.CreateRoom(code, new RoomOptions {MaxPlayers = maxPlayersPerRoom, IsVisible = false}, null,
                new string[]{});
        }
        
        
        public void JoinPrivateRoom()
        {
            creator = false;
            publicGame = false;
            value = inputRoomNameToJoin.GetComponent<Text>().text;
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Room Name is null or empty");
                return;
            }
            PhotonNetwork.JoinRoom(value);
        }

        public void LaunchGame()
        {
            Debug.Log("We load the 'MapGame");
            PhotonNetwork.LoadLevel("MapGame");
        }

        #endregion
    
        #region MonoBehaviourPunCallbacks Callbacks



        public void leaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
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
                Debug.Log("TESTR");
                inviteCodeLabel.SetActive(true);
                inviteCodeLabel.GetComponent<Text>().text = "CODE : " + PhotonNetwork.CurrentRoom.Name;
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRoomFailed");
            if (!publicGame)
            {
            }
        }

        // There was no random room or they were all full -> CreateRoom
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            if (publicGame)
            {
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
                // now the player joins the room
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("OnPlayerEnteredRoom() called by PUN. Now this client is in a room.");
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2 && publicGame)
            {
                Debug.Log("We load the game scene");
                PhotonNetwork.LoadLevel(sceneName);
            }
            else if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                launchGameButton.SetActive(true);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !creator)
            {
                creator = true;
            }
        }

        #endregion
    }
}