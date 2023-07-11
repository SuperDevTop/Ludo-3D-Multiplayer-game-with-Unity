using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MenuPhoton : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Text playerNameCreate;

    [Header("---------- Player2Mode ----------")]    
    [SerializeField] private GameObject menu2Panel;
    [SerializeField] private GameObject room2Panel;
    [SerializeField] private GameObject play2Btn;
    [SerializeField] private GameObject[] roomList;
    [SerializeField] private GameObject waiting2Player;
    [SerializeField] private Text[] player2Name;

    [Header("---------- Player4Mode ----------")]
    [SerializeField] private GameObject menu4Panel;
    [SerializeField] private GameObject room4Panel;
    [SerializeField] private GameObject play4Btn;
    [SerializeField] private GameObject[] room4List;
    [SerializeField] private GameObject[] waiting4Player;
    [SerializeField] private Text[] player4Name;

    List<RoomInfo> rooms = new List<RoomInfo>();
    private string gameVersion = "1";

    public static int playMode = 0;
    public static List<int> playerIDs;    
    public string[] playerNamesStr;
    public static List<string> playerNames;

    private bool is2Connecting;
    private bool is4Connecting;
    private bool isAvailableRoom;

    #region MonobehaviourCallbacks
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.Disconnect();
        is2Connecting = false;
        is4Connecting = false;
        isAvailableRoom = false;

        playerIDs = new List<int>();
        playerNames = new List<string>();
    }

    #endregion

    #region MonoBehaviourPunCallbacks
    void ConnectToRegion()
    {
        //AppSettings regionSettings = new AppSettings();
        //regionSettings.UseNameServer = true;
        //regionSettings.FixedRegion = "eu";
        //regionSettings.AppIdRealtime = "8068fc05-4485-40fd-b050-3794aef552d1";
        //regionSettings.AppVersion = gameVersion;
        //PhotonNetwork.ConnectUsingSettings(regionSettings);

        // Test
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // -------------------- Connect two players mode ----------------------
    public void Connect2Players()
    {
        is2Connecting = true;
        is4Connecting = false;

        ConnectToRegion();
        loadingPanel.SetActive(true);
    }

    // -------------------- Connect four players mode ----------------------
    public void Connect4Players()
    {
        is4Connecting = true;
        is2Connecting = false;

        ConnectToRegion();
        loadingPanel.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomLists)
    {
        print("Room updated.");
        rooms = roomLists;      

        if(roomLists.Count >= 1)
        {
            isAvailableRoom = true;
            print("Available");
        }
        else
        {
            isAvailableRoom = false;
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master!");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected.");
    }

    public override void OnJoinedLobby()
    {
        print("Joined Robby.");
        PhotonNetwork.NickName = playerNameCreate.text;
        StartCoroutine(DelayForJoinning());        
    }

    public override void OnJoinedRoom()
    {
        print("Joined Room.");
        loadingPanel.SetActive(false);

        if (is2Connecting)
        {
            room2Panel.SetActive(true);
        }
        else if (is4Connecting){
            room4Panel.SetActive(true);
        }
    }

    public override void OnCreatedRoom()
    {
        print("Room created.");
        loadingPanel.SetActive(false);
        RefreshNames();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("New player entered room.");

        if (is2Connecting)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            play2Btn.SetActive(true);

            RefreshNames();
        }    

        // Player4Mode
        if (is4Connecting && PhotonNetwork.IsMasterClient)
        {
            RefreshNames();
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 4 && PhotonNetwork.IsMasterClient)
        {
            play4Btn.SetActive(true);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Player left the room.");

        if (is2Connecting)
        {
            playerNamesStr[1] = "Open";
            play2Btn.SetActive(false);
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;

            RefreshNames();
        }

        // Player4Mode
        if (is4Connecting && PhotonNetwork.IsMasterClient)
        {
            play4Btn.SetActive(false);
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;

            RefreshNames();
        }           
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (is2Connecting)
        {
            LeaveRoomBtnClick();
        }

        if (is4Connecting)
        {
            LeaveRoom4BtnClick();
        }
    }

    public void Create2Room()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)2;

        PhotonNetwork.CreateRoom(playerNameCreate.text + "_" + Random.Range(0.1f, 10.0f), roomOptions, TypedLobby.Default);
    }

    public void Create4Room()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        //roomOptions.EmptyRoomTtl = 0;
        roomOptions.MaxPlayers = (byte)4;

        PhotonNetwork.CreateRoom(playerNameCreate.text + "_" + Random.Range(0.1f, 10.0f), roomOptions, TypedLobby.Default);
    }

    public void RefreshNames()
    {
        for (int i = 0; i < 4; i++)
        {
            playerNamesStr[i] = "Open";
        }

        List<string> names = new List<string>();

        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            names.Add(playerInfo.Value.NickName);
        }

        for(int i = 0; i < names.Count; i++)
        {
            playerNamesStr[i] = names[i];
        }
        
        SyncPlayerNames(new object[] { playerNamesStr[0], playerNamesStr[1], playerNamesStr[2], playerNamesStr[3] });
    }

    #endregion

    #region UI Functions

    public void AIModePlay()
    {
        SceneManager.LoadScene("AIScene");
    }

    // Player2Mode ------------------------------------
    public void LeaveRoomBtnClick()
    {
        room2Panel.SetActive(false);
        loadingPanel.SetActive(false); 
        isAvailableRoom = false;
        play2Btn.SetActive(false);

        PhotonNetwork.Disconnect();
    }

    public void Play2Mode()
    {
        playMode = 0;

        for (int i = 0; i < 2; i++)
        {
            playerNames.Add(playerNamesStr[i]);
        }

        SceneManager.LoadScene("MultiScene");
    }

    // Player4Mode ------------------------------------
    public void LeaveRoom4BtnClick()
    {
        room4Panel.SetActive(false);
        loadingPanel.SetActive(false);
        isAvailableRoom = false;
        play2Btn.SetActive(false);

        PhotonNetwork.Disconnect();
    }

    public void Play4Mode()
    {
        playerIDs.Clear();
        
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            playerIDs.Add(playerInfo.Value.ActorNumber);                        
        }

        for(int i = 0; i < 4; i++)
        {
            playerNames.Add(playerNamesStr[i]);
        }

        playMode = 1;
        SceneManager.LoadScene("MultiScene");
    }

    IEnumerator DelayForJoinning()
    {
        yield return new WaitForSeconds(3f);

        if (is2Connecting)
        {
            if (isAvailableRoom)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].MaxPlayers == 2)
                    {
                        if (rooms[i].IsOpen && rooms[i].IsVisible)
                        {
                            PhotonNetwork.JoinRoom(rooms[i].Name);
                            print("Random joining...");
                            break;
                        }
                    }
                }

                if (PhotonNetwork.NetworkClientState != ClientState.Joining
                    && PhotonNetwork.NetworkClientState != ClientState.Joined)
                {
                    print("Random creating...");
                    Create2Room();
                }
            }
            else
            {
                Create2Room();
            }
        }

        if (is4Connecting)
        {
            if (isAvailableRoom)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].MaxPlayers == 4)
                    {
                        if (rooms[i].IsOpen && rooms[i].IsVisible)
                        {
                            PhotonNetwork.JoinRoom(rooms[i].Name);
                            print("Random joining...");
                            break;
                        }
                    }
                }

                if (PhotonNetwork.NetworkClientState != ClientState.Joining
                    && PhotonNetwork.NetworkClientState != ClientState.Joined)
                {
                    Create4Room();
                }
            }
            else
            {
                Create4Room();
            }
        }
    }

    #endregion

    #region PunCallbacks

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SyncPlayerNames(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(0, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if(eventCode == 0)
        {
            object[] infos = (object[])photonEvent.CustomData;

            if (is2Connecting)
            {
                waiting2Player.SetActive(true);  
                player2Name[0].text = infos[0].ToString();
                player2Name[1].text = infos[1].ToString();
                playerNamesStr[0] = infos[0].ToString();
                playerNamesStr[1] = infos[1].ToString();

                if (PhotonNetwork.CurrentRoom.Players.Count == 2)
                {
                    waiting2Player.SetActive(false);       
                }
            }

            if (is4Connecting)
            {
                for (int i = 0; i < 4; i++)
                {
                    player4Name[i].text = infos[i].ToString();
                    playerNamesStr[i] = infos[i].ToString();
                    waiting4Player[i].SetActive(true);
                }

                for(int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
                {
                    waiting4Player[i].SetActive(false);
                }
            }            
        }
    }

    #endregion
}
