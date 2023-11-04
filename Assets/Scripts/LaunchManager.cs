using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject launcherUI = null;
    public Text connectionStatus;
    [Space(5)]
    public GameObject gameInfo;
    public GameObject roomJoinUI;
    public InputField playerNameField;
    public InputField roomNameField;

    public GameObject buttonJoinRoom;
    public GameObject buttonLoadArena;
    public GameObject buttonLeaveRoom;

    public Text playerStatus;


    public GameObject playerlist;
    public Text[] players;

    private string gameVersion = "1";
    private string playerName = "";
    private string roomName = "";
    private bool loading = false;

    void Start()
    {
        playerlist.SetActive(false);
        gameInfo.SetActive(false);
        roomJoinUI.SetActive(false);
        buttonLoadArena.SetActive(false);
        buttonLeaveRoom.SetActive(false);
        PlayerPrefs.DeleteAll();
        Debug.Log("Connecting to Photon Network");
        ConnectToPhoton();
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        playerNameField.text = "Player" + Random.Range(0, 1000);
        roomNameField.text = "room";
        playerStatus.text = "";
    }

    void ConnectToPhoton()
    {
        connectionStatus.text = "Connecting...";
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            playerlist.SetActive(true);
            gameInfo.SetActive(false);
            playerName = playerNameField.text;
            roomName = roomNameField.text;
            PhotonNetwork.LocalPlayer.NickName = playerName;
            Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room "
            + roomName);
            RoomOptions roomOptions = new RoomOptions();
            TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
        }
    }

    public void LoadArena()
    {
        
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 5)
        {
            Loading();
            PhotonNetwork.LoadLevel("Race");
        }
        else
        {
            playerStatus.text = "5 players maximum";
        }
    }

    // Photon Methods
    public override void OnConnected()
    {
        base.OnConnected();
        connectionStatus.text = "";
        gameInfo.SetActive(true);
        roomJoinUI.SetActive(true);
        buttonLoadArena.SetActive(false);
        buttonLeaveRoom.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        launcherUI.SetActive(true);
        UpdatePlayerList();
        Debug.LogError("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //playerlist.SetActive(true);
            buttonLoadArena.SetActive(true);
            buttonJoinRoom.SetActive(false);
            buttonLeaveRoom.SetActive(true);
            playerStatus.text = "You are Lobby Leader";

        }
        else
        {
            buttonJoinRoom.SetActive(false);
            buttonLeaveRoom.SetActive(true);
            playerStatus.text = PhotonNetwork.MasterClient.NickName +" is Lobby Leader";
        }

        players[0].text = PhotonNetwork.LocalPlayer.NickName;
        players[0].color = Color.green;
        UpdatePlayerList();


    }
    public override void OnLeftRoom()
    {
        // seen when other disconnects
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Launcher");
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdatePlayerList();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdatePlayerList();
        }

    }

    public void UpdatePlayerList()
    {
        ResetList();
        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            players[i].text = player.NickName;
            players[i].color = Color.green;
            i++;
        }
    }

    public void ResetList()
    {
        for (int i = 0; i < 5; i++)
        {
            players[i].text = "Empty player";
            players[i].color = Color.red;
        }
    }

    void Loading()
    {
        connectionStatus.text = "Loading circuit...";
    }
    
}