using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiEngine : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region GameInfos
    enum TeamIndex
    {
        Player1 = 0,
        Player2 = 1,
        Player3 = 2,
        Player4 = 3
    }

    enum CellType
    {
        Normal = 0,
        Origin = 1,
        End = 2,
        Safe = 3
    }

    enum TurnIndex
    {
        Player1 = 0,
        Player2 = 1,
        Player3 = 2,
        Player4 = 3
    }

    enum GameState
    {
        Start = 0,
        End = 1
    }

    enum TurnState
    {
        TurnStart = 0,
        TurnPlay = 1
    }

    enum GameType
    {
        Two = 0,
        Four = 1
    }

    #endregion

    [Header("-------------------- UI --------------------")]
    [SerializeField] private GameObject multiUI;
    [SerializeField] private GameObject winning2Dialog;
    [SerializeField] private GameObject winning4Dialog;
    [SerializeField] private GameObject playerLeftDialog;
    [SerializeField] private Text playerLeftName;
    [SerializeField] private Text whoseTurnText;
    [SerializeField] private Text[] player2Names;
    [SerializeField] private Text[] player4Names;
    [SerializeField] private List<string> playernames;
    [SerializeField] private List<string> rankNames;

    [Header("-------------------- Game --------------------")]
    [SerializeField] private GameObject[] player1Pawns;
    [SerializeField] private GameObject[] player2Pawns;
    [SerializeField] private GameObject[] player3Pawns;
    [SerializeField] private GameObject[] player4Pawns;
    [SerializeField] private GameObject[] player1Bases;
    [SerializeField] private GameObject[] player2Bases;
    [SerializeField] private GameObject[] player3Bases;
    [SerializeField] private GameObject[] player4Bases;
    [SerializeField] private GameObject[] player1Path;
    [SerializeField] private GameObject[] player2Path;
    [SerializeField] private GameObject[] player3Path;
    [SerializeField] private GameObject[] player4Path;
    [SerializeField] private GameObject[] originObjects;
    [SerializeField] private GameObject[] playerAvatars;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject selectedPawn;
    [SerializeField] private GameObject env;
    [SerializeField] private GameObject[] allPawns;    

    [SerializeField] private AnimationClip[] diceAnimations;

    [SerializeField] private int[] playerIDs;
    [SerializeField] private int currentTurnIndex;
    [SerializeField] private int gameTypeIndex;
    [SerializeField] private int gameStateIndex;
    [SerializeField] private int turnStateIndex;
    [SerializeField] private int moveIndex;
    [SerializeField] private int increasePathIndex;
    [SerializeField] private int playerIndex;
    [SerializeField] private int arrivedPawnIndex;        
    [SerializeField] private int[] arrivedCount;
    [SerializeField] private float moveSpeed;

    [SerializeField] private bool isMove;
    [SerializeField] private bool isAllDisabled;
    [SerializeField] private bool isKill;
    [SerializeField] private bool isRollingDice;
    [SerializeField] private bool[] isArrived;

    float offset;

    //[Header("--------------- Test ----------------")]
    //[SerializeField] private Text logText;
    //[SerializeField] private Text log1Text;

    #region MonobehaviourCallbacks
    void Start()
    {
        offset = 0.2f;
        rankNames = new List<string>();

        if (PhotonNetwork.IsMasterClient)
        {
            print("SyncTurn: " + "!!!");
            print("Master-GameTypeIndex: " + MenuPhoton.playMode);
            SetTurnIndex();
        }        
    }

    void Update()
    {
        multiUI.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);

        if (gameStateIndex == (int)GameState.Start)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Creates a Ray from the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (gameTypeIndex == 0)
                    {
                        if ((currentTurnIndex == 0 && PhotonNetwork.IsMasterClient) || (currentTurnIndex == 1 && !PhotonNetwork.IsMasterClient) && !isMove)
                        {
                            if (hit.transform.gameObject.tag == "Pawn" && turnStateIndex == (int)TurnState.TurnPlay && hit.transform.gameObject.GetComponent<PlayerInfo>().teamIndex == currentTurnIndex)
                            {
                                if (hit.transform.gameObject.GetComponent<PlayerInfo>().isActive)
                                {
                                    for (int i = 0; i < player1Pawns.Length; i++)
                                    {
                                        if (hit.transform.gameObject == player1Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                        else if (hit.transform.gameObject == player2Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                        else if (hit.transform.gameObject == player3Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                        else if (hit.transform.gameObject == player4Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                    }
                                }
                                else
                                {
                                    if (moveIndex == 5)
                                    {
                                        for (int i = 0; i < player1Pawns.Length; i++)
                                        {
                                            if (hit.transform.gameObject == player1Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                            else if (hit.transform.gameObject == player2Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                            else if (hit.transform.gameObject == player3Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                            else if (hit.transform.gameObject == player4Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                        }
                                    }
                                }
                            }
                            else if (hit.transform.gameObject.tag == "Dice" && turnStateIndex == (int)TurnState.TurnStart && !isRollingDice)
                            {
                                isRollingDice = true;
                                PlayDiceAnimation();
                            }
                        }
                    }
                    else
                    {
                        if (currentTurnIndex == playerIndex)
                        {
                            if (hit.transform.gameObject.tag == "Pawn" && turnStateIndex == (int)TurnState.TurnPlay && hit.transform.gameObject.GetComponent<PlayerInfo>().teamIndex == currentTurnIndex && !isMove)
                            {
                                if (hit.transform.gameObject.GetComponent<PlayerInfo>().isActive)
                                {
                                    for (int i = 0; i < player1Pawns.Length; i++)
                                    {
                                        if (hit.transform.gameObject == player1Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                        else if (hit.transform.gameObject == player2Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                        else if (hit.transform.gameObject == player3Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                        else if (hit.transform.gameObject == player4Pawns[i])
                                        {
                                            SyncMove(new object[] { i });
                                        }
                                    }
                                }
                                else
                                {
                                    if (moveIndex == 5)
                                    {
                                        for (int i = 0; i < player1Pawns.Length; i++)
                                        {
                                            if (hit.transform.gameObject == player1Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                            else if (hit.transform.gameObject == player2Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                            else if (hit.transform.gameObject == player3Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                            else if (hit.transform.gameObject == player4Pawns[i])
                                            {
                                                SyncSetOrigin(new object[] { i });
                                            }
                                        }
                                    }
                                }
                            }
                            else if (hit.transform.gameObject.tag == "Dice" && turnStateIndex == (int)TurnState.TurnStart)
                            {
                                isRollingDice = true;
                                PlayDiceAnimation();
                            }
                        }
                    }
                }
            }

            if (isMove && turnStateIndex == (int)TurnState.TurnPlay)
            {
                switch (currentTurnIndex)
                {
                    case 0:
                        {
                            MoveAnimation(0);
                            break;
                        }
                    case 1:
                        {
                            MoveAnimation(1);
                            break;
                        }
                    case 2:
                        {
                            MoveAnimation(2);
                            break;
                        }
                    case 3:
                        {
                            MoveAnimation(3);
                            break;
                        }
                }
            }
        }
    }    

    public void LeaveRoomBtnClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SyncLeave(new object[] { 0 });
        }
        else
        {
            SyncLeave(new object[] { 1 });
        }        
    }
    #endregion

    #region PhotonCallbacks
    public void SyncTurn(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(0, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncMove(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncSetOrigin(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncDiceAnimation(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(3, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncPlayerIDs(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncCamView(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(5, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncLeave(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(6, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncPlayerNames(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(7, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SyncRankNames(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(8, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == 0)
        {
            // ------------------------------- Sync Turn -------------------------------

            object[] infos = (object[])photonEvent.CustomData;

            currentTurnIndex = (int)infos[0];
            print("CurrentTurn: " + currentTurnIndex);
            print("GameTypeMode: " + gameTypeIndex);            

            GameFormat();
        }
        else if (eventCode == 1)
        {
            // ------------------------------- Sync Move -------------------------------

            object[] infos = (object[])photonEvent.CustomData;

            switch (currentTurnIndex)
            {
                case 0:
                    {
                        selectedPawn = player1Pawns[(int)infos[0]];
                        increasePathIndex = 0;
                        break;
                    }
                case 1:
                    {
                        selectedPawn = player2Pawns[(int)infos[0]];
                        increasePathIndex = 0;
                        break;
                    }
                case 2:
                    {
                        selectedPawn = player3Pawns[(int)infos[0]];
                        increasePathIndex = 0;
                        break;
                    }
                case 3:
                    {
                        selectedPawn = player4Pawns[(int)infos[0]];
                        increasePathIndex = 0;
                        break;
                    }
            }

            if(selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + moveIndex >= 56)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
        }
        else if (eventCode == 2)
        {
            // ------------------------------- Sync Set Origin -------------------------------

            object[] infos = (object[])photonEvent.CustomData;

            switch (currentTurnIndex)
            {
                case 0:
                    {
                        selectedPawn = player1Pawns[(int)infos[0]];
                        SetOrigin();                        
                        break;
                    }
                case 1:
                    {
                        selectedPawn = player2Pawns[(int)infos[0]];
                        SetOrigin();                        
                        break;
                    }
                case 2:
                    {
                        selectedPawn = player3Pawns[(int)infos[0]];
                        SetOrigin();                        
                        break;
                    }
                case 3:
                    {
                        selectedPawn = player4Pawns[(int)infos[0]];
                        SetOrigin();                        
                        break;
                    }
            }
        }
        else if (eventCode == 3)
        {
            // ------------------------------- Sync Dice Animation -------------------------------

            object[] infos = (object[])photonEvent.CustomData;
            moveIndex = (int)infos[0];

            if (IsAllDisabled() && moveIndex != 5 || IsAllUnavailable())
            {
                DiceAnimation(moveIndex);
                StartCoroutine(DelayDiceAnimation());
                print("Animation1");
            }
            else if ((bool)((object[])IsOneEnabled())[0] && moveIndex != 5)
            {
                DiceAnimation(moveIndex);

                // ---------------- Auto moving Pawn -----------------
                int activeIndex = (int)((object[])IsOneEnabled())[1];
                StartCoroutine(DelayToSyncMove(moveIndex, activeIndex));

                print("Animation2");
            } 
            else
            {
                turnStateIndex = (int)TurnState.TurnPlay;              
                DiceAnimation(moveIndex);

                print("MoveIndex: " + moveIndex);
            }            
        }
        else if (eventCode == 4)
        {
            // ------------------------------- Sync Player IDs -------------------------------

            object[] infos = (object[])photonEvent.CustomData;

            for (int i = 0; i < infos.Length; i++)
            {
                playerIDs[i] = (int)infos[i];
            }

            for (int i = 0; i < playerIDs.Length; i++)
            {
                if (playerIDs[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    print("PLAYERINDEX: " + i);
                    playerIndex = i;
                }
            }

            switch (PhotonNetwork.LocalPlayer.ActorNumber)
            {
                case 1:
                    {
                        env.transform.rotation = Quaternion.Euler(-52.564f, 0, 0);
                        break;
                    }
                case 2:
                    {
                        env.transform.rotation = Quaternion.Euler(-52.564f, 0, 180);
                        break;
                    }
                case 3:
                    {
                        env.transform.rotation = Quaternion.Euler(-52.564f, 0, 270);
                        break;
                    }
                case 4:
                    {
                        env.transform.rotation = Quaternion.Euler(-52.564f, 0, 90);
                        break;
                    }
            }

            gameTypeIndex = 1;
            currentTurnIndex = UnityEngine.Random.RandomRange(0, 4);
            SyncTurn(new object[] { currentTurnIndex });
        }
        else if (eventCode == 5)
        {
            // ------------------------------- Sync Camera View -------------------------------

            object[] infos = (object[])photonEvent.CustomData;            

            if (PhotonNetwork.IsMasterClient)
            {
                env.transform.rotation = Quaternion.Euler(-52.564f, 0, 0);
            }
            else
            {
                env.transform.rotation = Quaternion.Euler(-52.564f, 0, 180);
            }
        }
        else if (eventCode == 6)
        {
            // ------------------------------- Sync Leave Room -------------------------------

            object[] infos = (object[])photonEvent.CustomData;
            StartCoroutine(DelayLeave((int)infos[0]));            
        }
        else if (eventCode == 7)
        {
            // ------------------------------- Sync Player Names -------------------------------

            object[] infos = (object[])photonEvent.CustomData;
            playernames = new List<string>();
            playernames.Add(infos[0].ToString());
            playernames.Add(infos[1].ToString());
            playernames.Add(infos[2].ToString());
            playernames.Add(infos[3].ToString());

            switch (playerIndex)
            {
                case 0:
                    {
                        playerAvatars[0].GetComponentsInChildren<Text>()[0].text = playernames[0];
                        playerAvatars[1].GetComponentsInChildren<Text>()[0].text = playernames[1];
                        playerAvatars[2].GetComponentsInChildren<Text>()[0].text = playernames[2];
                        playerAvatars[3].GetComponentsInChildren<Text>()[0].text = playernames[3];
                        break;
                    }
                case 1:
                    {
                        playerAvatars[0].GetComponentsInChildren<Text>()[0].text = playernames[1];
                        playerAvatars[1].GetComponentsInChildren<Text>()[0].text = playernames[0];
                        playerAvatars[2].GetComponentsInChildren<Text>()[0].text = playernames[3];
                        playerAvatars[3].GetComponentsInChildren<Text>()[0].text = playernames[2];
                        break;
                    }
                case 2:
                    {
                        playerAvatars[0].GetComponentsInChildren<Text>()[0].text = playernames[2];
                        playerAvatars[1].GetComponentsInChildren<Text>()[0].text = playernames[3];
                        playerAvatars[2].GetComponentsInChildren<Text>()[0].text = playernames[1];
                        playerAvatars[3].GetComponentsInChildren<Text>()[0].text = playernames[0];
                        break;
                    }
                case 3:
                    {
                        playerAvatars[0].GetComponentsInChildren<Text>()[0].text = playernames[3];
                        playerAvatars[1].GetComponentsInChildren<Text>()[0].text = playernames[2];
                        playerAvatars[2].GetComponentsInChildren<Text>()[0].text = playernames[0];
                        playerAvatars[3].GetComponentsInChildren<Text>()[0].text = playernames[1];
                        break;
                    }
            }

            whoseTurnText.text = playernames[currentTurnIndex] + "'s Turn"; 
        }
        else if (eventCode == 8)
        {
            // ------------------------------- Sync Rank Names -------------------------------

            object[] infos = (object[])photonEvent.CustomData;

            rankNames.Add(infos[0].ToString());

            if (rankNames.Count == 1 && gameTypeIndex == 0)
            {
                player2Names[0].text = rankNames[0];

                for(int i = 0; i < 2; i++)
                {
                    if(arrivedCount[i] != 4)
                    {
                        player2Names[1].text = playernames[i];
                        break;
                    }
                }

                StartCoroutine(DelayTo2Finish());
            }

            if (rankNames.Count == 3)
            {
                for(int i = 0; i < 3; i++)
                {
                    player4Names[i].text = rankNames[i];
                }

                for(int i = 0; i < 4; i++)
                {
                    if(arrivedCount[i] != 4)
                    {
                        player4Names[3].text = playernames[i];
                        break;
                    }
                }

                StartCoroutine(DelayTo4Finish());
            }            
        }
    }    

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected.");
        SceneManager.LoadScene("MenuScene");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SceneManager.LoadScene("MenuScene");
    }
    #endregion

    #region GameEngine

    public void SetTurnIndex()
    {
        gameTypeIndex = MenuPhoton.playMode;

        if (gameTypeIndex == 0)
        {
            currentTurnIndex = UnityEngine.Random.RandomRange(0, 2);
            SyncCamView(new object[] { 0});
            SyncTurn(new object[] { currentTurnIndex });
        }
        else
        {            
            SyncPlayerIDs(new object[] { MenuPhoton.playerIDs[0], MenuPhoton.playerIDs[1] , MenuPhoton.playerIDs[2] , MenuPhoton.playerIDs[3] });            
        }
    }

    // ------------------------------ Check if All are Disabled or not --------------------------------
    public bool IsAllDisabled()
    {
        isAllDisabled = true;

        switch (currentTurnIndex)
        {
            case 0:
                {
                    for (int i = 0; i < player1Pawns.Length; i++)
                    {
                        if (player1Pawns[i].GetComponent<PlayerInfo>().isActive && !player1Pawns[i].GetComponent<PlayerInfo>().isArrived)
                        {
                            isAllDisabled = false;
                        }
                    }
                    break;
                }
            case 1:
                {
                    for (int i = 0; i < player2Pawns.Length; i++)
                    {
                        if (player2Pawns[i].GetComponent<PlayerInfo>().isActive && !player2Pawns[i].GetComponent<PlayerInfo>().isArrived)
                        {
                            isAllDisabled = false;
                        }
                    }
                    break;
                }
            case 2:
                {
                    for (int i = 0; i < player3Pawns.Length; i++)
                    {
                        if (player3Pawns[i].GetComponent<PlayerInfo>().isActive && !player3Pawns[i].GetComponent<PlayerInfo>().isArrived)
                        {
                            isAllDisabled = false;
                        }
                    }
                    break;
                }
            case 3:
                {
                    for (int i = 0; i < player4Pawns.Length; i++)
                    {
                        if (player4Pawns[i].GetComponent<PlayerInfo>().isActive && !player4Pawns[i].GetComponent<PlayerInfo>().isArrived)
                        {
                            isAllDisabled = false;
                        }
                    }
                    break;
                }
        }

        return isAllDisabled;
    }

    // ------------------------------ Look for only one enabled -----------------------------------
    public object IsOneEnabled()
    {
        bool isOneEnabled = false;
        int activeCount = 0;
        int activePawnIndex = 0;
        object[] result = new object[] { isOneEnabled, 0 };

        switch (currentTurnIndex)
        {
            case 0:
                {                    
                    for (int i = 0; i < player1Pawns.Length; i++)
                    {
                        if (player1Pawns[i].GetComponent<PlayerInfo>().isActive && !player1Pawns[i].GetComponent<PlayerInfo>().isArrived
                            && player1Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            activeCount++;
                            activePawnIndex = i;
                        }
                    }                    

                    break;
                }

            case 1:
                {
                    for (int i = 0; i < player2Pawns.Length; i++)
                    {
                        if (player2Pawns[i].GetComponent<PlayerInfo>().isActive && !player2Pawns[i].GetComponent<PlayerInfo>().isArrived
                            && player2Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            activeCount++;
                            activePawnIndex = i;
                        }
                    }

                    break;
                }

            case 2:
                {
                    for (int i = 0; i < player3Pawns.Length; i++)
                    {
                        if (player3Pawns[i].GetComponent<PlayerInfo>().isActive && !player3Pawns[i].GetComponent<PlayerInfo>().isArrived
                            && player3Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            activeCount++;
                            activePawnIndex = i;
                        }
                    }

                    break;
                }

            case 3:
                {
                    for (int i = 0; i < player4Pawns.Length; i++)
                    {
                        if (player4Pawns[i].GetComponent<PlayerInfo>().isActive && !player4Pawns[i].GetComponent<PlayerInfo>().isArrived
                            && player4Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            activeCount++;
                            activePawnIndex = i;
                        }
                    }

                    break;
                }                
        }

        if (activeCount == 1)
        {
            isOneEnabled = true;
            result = new object[] { isOneEnabled, activePawnIndex };
        }

        return result;
    }  
    
    // --------------------------- Check that all pawns active or not ----------------------------

    public bool IsAllUnavailable()
    {
        bool isAllUnavailable = false;
        bool isActiveExist = false;
        int unAvailableCount = 0;

        // Existing locked pawns & no 6, arrived, exceed range
        switch (currentTurnIndex)
        {
            case 0:
                {
                    for (int i = 0; i < player1Pawns.Length; i++)
                    {
                        if(!player1Pawns[i].GetComponent<PlayerInfo>().isActive && moveIndex != 5 || player1Pawns[i].GetComponent<PlayerInfo>().isArrived
                            || player1Pawns[i].GetComponent<PlayerInfo>().isActive && player1Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex >= 56)
                        {
                            unAvailableCount++;
                        }   

                        if (player1Pawns[i].GetComponent<PlayerInfo>().isActive)
                        {
                            isActiveExist = true;                
                        }
                    }

                    break;
                }

            case 1:
                {
                    for (int i = 0; i < player2Pawns.Length; i++)
                    {
                        if (!player2Pawns[i].GetComponent<PlayerInfo>().isActive && moveIndex != 5 || player2Pawns[i].GetComponent<PlayerInfo>().isArrived
                            || player2Pawns[i].GetComponent<PlayerInfo>().isActive && player2Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex >= 56)
                        {
                            unAvailableCount++;
                        }

                        if (player2Pawns[i].GetComponent<PlayerInfo>().isActive)
                        {
                            isActiveExist = true;               
                        }
                    }

                    break;
                }

            case 2:
                {
                    for (int i = 0; i < player3Pawns.Length; i++)
                    {
                        if (!player3Pawns[i].GetComponent<PlayerInfo>().isActive && moveIndex != 5 || player3Pawns[i].GetComponent<PlayerInfo>().isArrived
                            || player3Pawns[i].GetComponent<PlayerInfo>().isActive && player3Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex >= 56)
                        {
                            unAvailableCount++;
                        }

                        if (player3Pawns[i].GetComponent<PlayerInfo>().isActive)
                        {
                            isActiveExist = true;                 
                        }
                    }

                    break;
                }

            case 3:
                {
                    for (int i = 0; i < player4Pawns.Length; i++)
                    {
                        if (!player4Pawns[i].GetComponent<PlayerInfo>().isActive && moveIndex != 5 || player4Pawns[i].GetComponent<PlayerInfo>().isArrived
                            || player4Pawns[i].GetComponent<PlayerInfo>().isActive && player4Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex >= 56)
                        {
                            unAvailableCount++;
                        }

                        if (player4Pawns[i].GetComponent<PlayerInfo>().isActive)
                        {
                            isActiveExist = true;          
                        }
                    }

                    break;
                }
        }

        if(unAvailableCount == 4 && isActiveExist)
        {
            isAllUnavailable = true;            
        }                            

        return isAllUnavailable;
    }

    // --------------------------- Check that all pawns exceed Range ----------------------------
    public bool IsAllExceedRange()
    {
        bool isAllExceed = true;

        // Existing locked pawns & no 6, arrived, exceed range
        switch (currentTurnIndex)
        {
            case 0:
                {
                    for (int i = 0; i < player1Pawns.Length; i++)
                    {
                        if (player1Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            isAllExceed = false;
                            break;
                        }
                    }

                    break;
                }

            case 1:
                {
                    for (int i = 0; i < player2Pawns.Length; i++)
                    {
                        if (player2Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            isAllExceed = false;
                            break;
                        }
                    }

                    break;
                }

            case 2:
                {
                    for (int i = 0; i < player3Pawns.Length; i++)
                    {
                        if (player3Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            isAllExceed = false;
                            break;
                        }
                    }

                    break;
                }

            case 3:
                {
                    for (int i = 0; i < player4Pawns.Length; i++)
                    {
                        if (player4Pawns[i].GetComponent<PlayerInfo>().currentPosIndex + moveIndex < 56)
                        {
                            isAllExceed = false;
                            break;
                        }
                    }

                    break;
                }
        }

        return isAllExceed;
    }

    // --------------------------- Initialize parameters ---------------------------------------
    public void GameFormat()
    {
        gameStateIndex = (int)GameState.Start;
        moveSpeed = 4f;

        if (gameTypeIndex == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                playerIndex = 0;
            }
            else
            {
                playerIndex = 1;
            }
            
            playerAvatars[2].SetActive(false);
            playerAvatars[3].SetActive(false);
        }

        //List<string> names = new List<string>();
        
        //foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        //{
        //    names.Add(playerInfo.Value.NickName);                        
        //}

        //for (int i = 0; i < names.Count; i++)
        //{
        //    if (gameTypeIndex == 0)
        //    {                
        //        playerAvatars[i].GetComponentsInChildren<Text>()[0].text = names[i];
        //    }                       
        //}

        if(gameTypeIndex == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SyncPlayerNames(new object[] { MenuPhoton.playerNames[0], MenuPhoton.playerNames[1], "Open", "Open" });
            }                
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SyncPlayerNames(new object[] { MenuPhoton.playerNames[0], MenuPhoton.playerNames[1], MenuPhoton.playerNames[2], MenuPhoton.playerNames[3] });
            }
        }

        for (int i = 0; i < player1Pawns.Length; i++)
        {
            player1Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
            player2Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
            player3Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
            player4Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
            player1Pawns[i].GetComponent<Animator>().enabled = false;
            player2Pawns[i].GetComponent<Animator>().enabled = false;
            player3Pawns[i].GetComponent<Animator>().enabled = false;
            player4Pawns[i].GetComponent<Animator>().enabled = false;

            if (gameTypeIndex == 0)
            {
                player3Pawns[i].SetActive(false);
                player4Pawns[i].SetActive(false);
            }
        }

        TurnFormat();
    }

    public void TurnFormat()
    {        
        if (isArrived[playerIndex] && playerIndex == currentTurnIndex)
        {
            ChangeTurn();
        }
        else
        {
            print("Turn Format!!!");
            increasePathIndex = 0;
            moveIndex = 0;
            isMove = false;
            turnStateIndex = (int)TurnState.TurnStart;
            isRollingDice = false;
            dice.GetComponent<Animator>().enabled = false;

            List<string> names = new List<string>();

            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                names.Add(playerInfo.Value.NickName);
            }

            if (gameTypeIndex == 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    whoseTurnText.text = names[currentTurnIndex] + "'s Turn";
                }
                else
                {
                    whoseTurnText.text = names[(currentTurnIndex + 1) % 2] + "'s Turn";
                }
            }
            else
            {
                if (playernames.Count == 4)
                {
                    whoseTurnText.text = playernames[currentTurnIndex] + "'s Turn";
                }
            }

            for (int i = 0; i < player1Pawns.Length; i++)
            {
                player1Pawns[i].GetComponent<Animator>().enabled = false;
                player2Pawns[i].GetComponent<Animator>().enabled = false;
                player3Pawns[i].GetComponent<Animator>().enabled = false;
                player4Pawns[i].GetComponent<Animator>().enabled = false;
            }

            switch (currentTurnIndex)
            {
                case 0:
                    {
                        for (int i = 0; i < player1Pawns.Length; i++)
                        {
                            player1Pawns[i].GetComponent<Animator>().enabled = true;
                            player2Pawns[i].GetComponent<Animator>().enabled = false;
                            player3Pawns[i].GetComponent<Animator>().enabled = false;
                            player4Pawns[i].GetComponent<Animator>().enabled = false;
                        }

                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i < player1Pawns.Length; i++)
                        {
                            player1Pawns[i].GetComponent<Animator>().enabled = false;
                            player2Pawns[i].GetComponent<Animator>().enabled = true;
                            player3Pawns[i].GetComponent<Animator>().enabled = false;
                            player4Pawns[i].GetComponent<Animator>().enabled = false;
                        }

                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < player1Pawns.Length; i++)
                        {
                            player1Pawns[i].GetComponent<Animator>().enabled = false;
                            player2Pawns[i].GetComponent<Animator>().enabled = false;
                            player3Pawns[i].GetComponent<Animator>().enabled = true;
                            player4Pawns[i].GetComponent<Animator>().enabled = false;
                        }

                        break;
                    }
                case 3:
                    {
                        for (int i = 0; i < player1Pawns.Length; i++)
                        {
                            player1Pawns[i].GetComponent<Animator>().enabled = false;
                            player2Pawns[i].GetComponent<Animator>().enabled = false;
                            player3Pawns[i].GetComponent<Animator>().enabled = false;
                            player4Pawns[i].GetComponent<Animator>().enabled = true;
                        }

                        break;
                    }
            }
        }                
    }

    public void PlayDiceAnimation()
    {
        print("Dice Animation Start!!!");
        moveIndex = UnityEngine.Random.RandomRange(0, 6);
        SyncDiceAnimation(new object[] { moveIndex });
    }

    public void MoveAnimation(int index)
    {
        switch (index)
        {
            case 0:
                {
                    selectedPawn.transform.position = Vector3.MoveTowards(selectedPawn.transform.position,
                                        player1Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position,
                                        moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(selectedPawn.transform.position, player1Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position) < 0.01f)
                    {
                        if (increasePathIndex == moveIndex)
                        {
                            isMove = false;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosIndex += 1 + increasePathIndex;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosTile = player1Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex];

                            if (selectedPawn.GetComponent<PlayerInfo>().currentPosIndex == 56)
                            {
                                selectedPawn.GetComponent<PlayerInfo>().isArrived = true;
                                arrivedCount[0]++;
                            }

                            CheckToKillPawn();
                            CheckOverlappedPawns(0);
                            CheckArrivedPawns(gameTypeIndex);

                            if (moveIndex == 5 || isKill)
                            {           
                                TurnFormat();
                            }
                            else
                            {                        
                                ChangeTurn();
                            }                            
                        }
                        else
                        {
                            increasePathIndex++;
                        }
                    }
                    break;
                }
            case 1:
                {
                    selectedPawn.transform.position = Vector3.MoveTowards(selectedPawn.transform.position,
                                       player2Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position,
                                       moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(selectedPawn.transform.position, player2Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position) < 0.01f)
                    {
                        if (increasePathIndex == moveIndex)
                        {
                            isMove = false;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosIndex += 1 + increasePathIndex;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosTile = player2Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex];

                            if (selectedPawn.GetComponent<PlayerInfo>().currentPosIndex == 56)
                            {
                                selectedPawn.GetComponent<PlayerInfo>().isArrived = true;
                                arrivedCount[1]++;
                            }

                            CheckToKillPawn();
                            CheckOverlappedPawns(1);
                            CheckArrivedPawns(gameTypeIndex);

                            if (moveIndex == 5 || isKill)
                            {                
                                TurnFormat();
                            }
                            else
                            {                   
                                ChangeTurn();
                            }
                        }
                        else
                        {
                            increasePathIndex++;
                        }
                    }
                    break;
                }
            case 2:
                {
                    selectedPawn.transform.position = Vector3.MoveTowards(selectedPawn.transform.position,
                                      player3Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position,
                                      moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(selectedPawn.transform.position, player3Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position) < 0.01f)
                    {
                        if (increasePathIndex == moveIndex)
                        {
                            isMove = false;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosIndex += 1 + increasePathIndex;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosTile = player3Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex];

                            if (selectedPawn.GetComponent<PlayerInfo>().currentPosIndex == 56)
                            {
                                selectedPawn.GetComponent<PlayerInfo>().isArrived = true;
                                arrivedCount[2]++;
                            }

                            CheckToKillPawn();
                            CheckOverlappedPawns(2);
                            CheckArrivedPawns(gameTypeIndex);

                            if (moveIndex == 5 || isKill)
                            {            
                                TurnFormat();
                            }
                            else
                            {                        
                                ChangeTurn();
                            }
                        }
                        else
                        {
                            increasePathIndex++;
                        }
                    }
                    break;
                }
            case 3:
                {
                    selectedPawn.transform.position = Vector3.MoveTowards(selectedPawn.transform.position,
                                        player4Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position,
                                        moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(selectedPawn.transform.position, player4Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex + 1 + increasePathIndex].transform.position) < 0.01f)
                    {
                        if (increasePathIndex == moveIndex)
                        {
                            isMove = false;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosIndex += 1 + increasePathIndex;
                            selectedPawn.GetComponent<PlayerInfo>().currentPosTile = player3Path[selectedPawn.GetComponent<PlayerInfo>().currentPosIndex];

                            if (selectedPawn.GetComponent<PlayerInfo>().currentPosIndex == 56)
                            {
                                selectedPawn.GetComponent<PlayerInfo>().isArrived = true;
                                arrivedCount[3]++;
                            }

                            CheckToKillPawn();
                            CheckOverlappedPawns(3);
                            CheckArrivedPawns(gameTypeIndex);

                            if (moveIndex == 5 || isKill)
                            {   
                                TurnFormat();
                            }
                            else
                            {
                                ChangeTurn();
                            }
                        }
                        else
                        {
                            increasePathIndex++;
                        }
                    }
                    break;
                }
        }
    }

    public void ChangeTurn()
    {
        print("Change Turn.");
        
        if (gameTypeIndex == 0)
        {
            currentTurnIndex = (currentTurnIndex + 1) % 2;
        }
        else
        {
            currentTurnIndex = (currentTurnIndex + 1) % 4;
        }

        TurnFormat();
    }

    public void SetOrigin()
    {
        switch (currentTurnIndex)
        {
            case 0:
                {
                    selectedPawn.transform.position = originObjects[0].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[0];
                    selectedPawn.GetComponent<PlayerInfo>().currentPosIndex = 0;
                    break;
                }
            case 1:
                {
                    selectedPawn.transform.position = originObjects[1].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[1];
                    selectedPawn.GetComponent<PlayerInfo>().currentPosIndex = 0;
                    break;
                }
            case 2:
                {
                    selectedPawn.transform.position = originObjects[2].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[2];
                    selectedPawn.GetComponent<PlayerInfo>().currentPosIndex = 0;
                    break;
                }
            case 3:
                {
                    selectedPawn.transform.position = originObjects[3].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[3];
                    selectedPawn.GetComponent<PlayerInfo>().currentPosIndex = 0;
                    break;
                }
        }

        selectedPawn.GetComponent<PlayerInfo>().isActive = true;
        CheckOverlappedPawns(currentTurnIndex);

        if (moveIndex == 0)
        {
            ChangeTurn();
        }
        else
        {
            turnStateIndex = (int)TurnState.TurnStart;
            isRollingDice = false;
        }
    }

    public void CheckToKillPawn()
    {
        isKill = false;

        for (int i = 0; i < player1Pawns.Length; i++)
        {
            if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile == player1Pawns[i].GetComponent<PlayerInfo>().currentPosTile)
            {
                if (currentTurnIndex != player1Pawns[i].GetComponent<PlayerInfo>().teamIndex)
                {
                    if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 0 ||
                        (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 1 && selectedPawn.GetComponent<PlayerInfo>().currentPosTile != originObjects[0])
                            )
                    {                        
                        player1Pawns[i].transform.position = player1Bases[player1Pawns[i].GetComponent<PlayerInfo>().playerIndex].transform.position;
                        player1Pawns[i].GetComponent<PlayerInfo>().isActive = false;
                        isKill = true;
                        break;
                    }
                }
            }

            if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile == player2Pawns[i].GetComponent<PlayerInfo>().currentPosTile)
            {
                if (currentTurnIndex != player2Pawns[i].GetComponent<PlayerInfo>().teamIndex)
                {
                    if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 0 ||
                        (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 1 && selectedPawn.GetComponent<PlayerInfo>().currentPosTile != originObjects[1])
                            )
                    {
                        player2Pawns[i].transform.position = player2Bases[player2Pawns[i].GetComponent<PlayerInfo>().playerIndex].transform.position;
                        player2Pawns[i].GetComponent<PlayerInfo>().isActive = false;
                        isKill = true;
                        break;
                    }
                }
            }

            if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile == player3Pawns[i].GetComponent<PlayerInfo>().currentPosTile)
            {
                if (currentTurnIndex != player3Pawns[i].GetComponent<PlayerInfo>().teamIndex)
                {
                    if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 0 ||
                        (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 1 && selectedPawn.GetComponent<PlayerInfo>().currentPosTile != originObjects[2])
                            )
                    {
                        player3Pawns[i].transform.position = player3Bases[player3Pawns[i].GetComponent<PlayerInfo>().playerIndex].transform.position;
                        player3Pawns[i].GetComponent<PlayerInfo>().isActive = false;
                        isKill = true;
                        break;
                    }
                }
            }

            if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile == player4Pawns[i].GetComponent<PlayerInfo>().currentPosTile)
            {
                if (currentTurnIndex != player4Pawns[i].GetComponent<PlayerInfo>().teamIndex)
                {
                    if (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 0 ||
                        (selectedPawn.GetComponent<PlayerInfo>().currentPosTile.GetComponent<CellInfo>().cellType == 1 && selectedPawn.GetComponent<PlayerInfo>().currentPosTile != originObjects[3])
                            )
                    {
                        player4Pawns[i].transform.position = player4Bases[player4Pawns[i].GetComponent<PlayerInfo>().playerIndex].transform.position;
                        player4Pawns[i].GetComponent<PlayerInfo>().isActive = false;
                        isKill = true;
                        break;
                    }
                }
            }
        }
    }


    // ---------------------------- Check overlapped Pawns.-----------------------------------------
    public void CheckOverlappedPawns(int index)
    {
        switch (index)
        {
            case 0:
                {
                    for(int i = 0; i < player1Path.Length; i++)
                    {
                        List<GameObject> overlappedPawns = new List<GameObject>();                                                                        
                        
                        for(int j = 0; j < allPawns.Length; j++)
                        {
                            if (Vector3.Distance(player1Path[i].transform.position, allPawns[j].transform.position) < 0.3f)
                            {
                                overlappedPawns.Add(allPawns[j]);
                            }
                        }

                        switch (overlappedPawns.Count)
                        {
                            case 1:
                                {
                                    overlappedPawns[0].transform.position = player1Path[i].transform.position;
                                    break;
                                }
                            case 2:
                                {
                                    overlappedPawns[0].transform.position = player1Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player1Path[i].transform.position - new Vector3(offset, 0, 0);
                                    break;
                                }
                            case 3:
                                {
                                    overlappedPawns[0].transform.position = player1Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player1Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player1Path[i].transform.position;
                                    break;
                                }
                            case 4:
                                {
                                    overlappedPawns[0].transform.position = player1Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player1Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player1Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player1Path[i].transform.position - new Vector3(0, 0, offset);
                                    break;
                                }
                            case 5:
                                {
                                    overlappedPawns[0].transform.position = player1Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player1Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player1Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player1Path[i].transform.position - new Vector3(0, 0, offset);
                                    overlappedPawns[4].transform.position = player1Path[i].transform.position;
                                    break;
                                }
                        }
                    }
                    
                    break;
                }
            case 1:
                {
                    for (int i = 0; i < player2Path.Length; i++)
                    {
                        List<GameObject> overlappedPawns = new List<GameObject>();

                        for (int j = 0; j < allPawns.Length; j++)
                        {
                            if (Vector3.Distance(player2Path[i].transform.position, allPawns[j].transform.position) < 0.3f)
                            {
                                overlappedPawns.Add(allPawns[j]);
                            }
                        }

                        switch (overlappedPawns.Count)
                        {
                            case 1:
                                {
                                    overlappedPawns[0].transform.position = player2Path[i].transform.position;
                                    break;
                                }
                            case 2:
                                {
                                    overlappedPawns[0].transform.position = player2Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player2Path[i].transform.position - new Vector3(offset, 0, 0);
                                    break;
                                }
                            case 3:
                                {
                                    overlappedPawns[0].transform.position = player2Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player2Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player2Path[i].transform.position;
                                    break;
                                }
                            case 4:
                                {
                                    overlappedPawns[0].transform.position = player2Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player2Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player2Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player2Path[i].transform.position - new Vector3(0, 0, offset);
                                    break;
                                }
                            case 5:
                                {
                                    overlappedPawns[0].transform.position = player2Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player2Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player2Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player2Path[i].transform.position - new Vector3(0, 0, offset);
                                    overlappedPawns[4].transform.position = player2Path[i].transform.position;
                                    break;
                                }
                        }
                    }

                    break;
                }
            case 2:
                {
                    for (int i = 0; i < player3Path.Length; i++)
                    {
                        List<GameObject> overlappedPawns = new List<GameObject>();

                        for (int j = 0; j < allPawns.Length; j++)
                        {
                            if (Vector3.Distance(player3Path[i].transform.position, allPawns[j].transform.position) < 0.3f)
                            {
                                overlappedPawns.Add(allPawns[j]);
                            }
                        }

                        switch (overlappedPawns.Count)
                        {
                            case 1:
                                {
                                    overlappedPawns[0].transform.position = player3Path[i].transform.position;
                                    break;
                                }
                            case 2:
                                {
                                    overlappedPawns[0].transform.position = player3Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player3Path[i].transform.position - new Vector3(offset, 0, 0);
                                    break;
                                }
                            case 3:
                                {
                                    overlappedPawns[0].transform.position = player3Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player3Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player3Path[i].transform.position;
                                    break;
                                }
                            case 4:
                                {
                                    overlappedPawns[0].transform.position = player3Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player3Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player3Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player3Path[i].transform.position - new Vector3(0, 0, offset);
                                    break;
                                }
                            case 5:
                                {
                                    overlappedPawns[0].transform.position = player3Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player3Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player3Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player3Path[i].transform.position - new Vector3(0, 0, offset);
                                    overlappedPawns[4].transform.position = player3Path[i].transform.position;
                                    break;
                                }
                        }
                    }

                    break;
                }
            case 3:
                {
                    for (int i = 0; i < player4Path.Length; i++)
                    {
                        List<GameObject> overlappedPawns = new List<GameObject>();

                        for (int j = 0; j < allPawns.Length; j++)
                        {
                            if (Vector3.Distance(player4Path[i].transform.position, allPawns[j].transform.position) < 0.3f)
                            {
                                overlappedPawns.Add(allPawns[j]);
                            }
                        }

                        switch (overlappedPawns.Count)
                        {
                            case 1:
                                {
                                    overlappedPawns[0].transform.position = player4Path[i].transform.position;
                                    break;
                                }
                            case 2:
                                {
                                    overlappedPawns[0].transform.position = player4Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player4Path[i].transform.position - new Vector3(offset, 0, 0);
                                    break;
                                }
                            case 3:
                                {
                                    overlappedPawns[0].transform.position = player4Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player4Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player4Path[i].transform.position;
                                    break;
                                }
                            case 4:
                                {
                                    overlappedPawns[0].transform.position = player4Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player4Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player4Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player4Path[i].transform.position - new Vector3(0, 0, offset);
                                    break;
                                }
                            case 5:
                                {
                                    overlappedPawns[0].transform.position = player4Path[i].transform.position + new Vector3(offset, 0, 0);
                                    overlappedPawns[1].transform.position = player4Path[i].transform.position - new Vector3(offset, 0, 0);
                                    overlappedPawns[2].transform.position = player4Path[i].transform.position + new Vector3(0, 0, offset);
                                    overlappedPawns[3].transform.position = player4Path[i].transform.position - new Vector3(0, 0, offset);
                                    overlappedPawns[4].transform.position = player4Path[i].transform.position;
                                    break;
                                }
                        }
                    }

                    break;
                }
        }
    }

    // ---------------------------- Check arrived pawns. -------------------------------------------
    public void CheckArrivedPawns(int index)
    {
        print("Player1: " + arrivedCount[0]);
        print("Player2: " + arrivedCount[1]);

        if (gameTypeIndex == 0)
        {
            if(arrivedCount[playerIndex] == 4 && PhotonNetwork.IsMasterClient)
            {
                SyncRankNames(new object[] { playernames[playerIndex] });
            }         
        }
        else
        {
            if(arrivedCount[playerIndex] == 4 && !isArrived[playerIndex])
            {
                isArrived[playerIndex] = true;
                SyncRankNames(new object[] { playernames[playerIndex] });
            }
        }
    }

    public void DiceAnimation(int index)
    {
        switch (index)
        {
            case 0:
                {
                    dice.SetActive(false);
                    dice.SetActive(true);
                    dice.GetComponent<Animator>().enabled = true;
                    dice.GetComponent<Animator>().Play("dice1");
                    break;
                }
            case 1:
                {
                    dice.SetActive(false);
                    dice.SetActive(true);
                    dice.GetComponent<Animator>().enabled = true;
                    dice.GetComponent<Animator>().Play("dice2");
                    break;
                }
            case 2:
                {
                    dice.SetActive(false);
                    dice.SetActive(true);
                    dice.GetComponent<Animator>().enabled = true;
                    dice.GetComponent<Animator>().Play("dice3");
                    break;
                }
            case 3:
                {
                    dice.SetActive(false);
                    dice.SetActive(true);
                    dice.GetComponent<Animator>().enabled = true;
                    dice.GetComponent<Animator>().Play("dice4");
                    break;
                }
            case 4:
                {
                    dice.SetActive(false);
                    dice.SetActive(true);
                    dice.GetComponent<Animator>().enabled = true;
                    dice.GetComponent<Animator>().Play("dice5");
                    break;
                }
            case 5:
                {
                    dice.SetActive(false);
                    dice.SetActive(true);
                    dice.GetComponent<Animator>().enabled = true;
                    dice.GetComponent<Animator>().Play("dice6");
                    break;
                }
        }
    }

    IEnumerator DelayLeave(int index)
    {
        playerLeftDialog.SetActive(true);

        yield return new WaitForSeconds(5f);
        PhotonNetwork.Disconnect();
    }

    IEnumerator DelayDiceAnimation()
    {
        yield return new WaitForSeconds(2f);

        if(IsAllExceedRange() && moveIndex == 5)
        {
            TurnFormat();
            
            isRollingDice = true;
            PlayDiceAnimation();
        }
        else
        {
            ChangeTurn();
        }                
    }

    IEnumerator DelayToSyncMove(int move, int active)
    {        
        yield return new WaitForSeconds(2f);

        moveIndex = move;

        if (gameTypeIndex == 0)
        {
            if ((currentTurnIndex == 0 && PhotonNetwork.IsMasterClient) || (currentTurnIndex == 1 && !PhotonNetwork.IsMasterClient) && !isMove)
            {
                SyncMove(new object[] { active });
            }
        }
        else
        {
            if (currentTurnIndex == playerIndex)
            {
                SyncMove(new object[] { active });
            }
        }

        turnStateIndex = (int)TurnState.TurnPlay;        
    }

    IEnumerator DelayTo2Finish()
    {
        winning2Dialog.SetActive(true);
        gameStateIndex = (int)GameState.End;
        
        yield return new WaitForSeconds(5f);

        winning2Dialog.SetActive(false);
        PhotonNetwork.Disconnect();
    }

    IEnumerator DelayTo4Finish()
    {
        winning4Dialog.SetActive(true);
        gameStateIndex = (int)GameState.End;

        yield return new WaitForSeconds(5f);

        winning4Dialog.SetActive(false);
        PhotonNetwork.Disconnect();
    }

    #endregion
}
