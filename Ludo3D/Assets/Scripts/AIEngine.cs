using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIEngine : MonoBehaviour
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

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
