using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;
    bool isConnecting;
    string gameVersion = "1";
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    public GameObject playerPrefab;

    private void Start()
    {
        Screen.SetResolution(700, 400, false);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }
    
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            progressLabel.GetComponent<UnityEngine.UI.Text>().text = "Waiting for a special someone... :(";
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(1);
            progressLabel.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity);
                transform.position = new Vector3(0f, 5f, 0f);
            }
            else
            {
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, -5f, 0f), Quaternion.identity);
                transform.position = new Vector3(0f, -5f, 0f);
            }
        }
    }
}
