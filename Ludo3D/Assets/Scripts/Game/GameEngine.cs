using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

public class GameEngine : MonoBehaviour
{
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

    public GameObject[] player1Pawns;
    public GameObject[] player2Pawns;
    public GameObject[] player3Pawns;
    public GameObject[] player4Pawns;
    public GameObject[] player1Path;
    public GameObject[] player2Path;
    public GameObject[] player3Path;
    public GameObject[] player4Path; 
    public GameObject[] originObjects;
    public GameObject dice;
    public GameObject selectedPawn;

    public AnimationClip[] diceAnimations;
   
    public int currentTurnIndex;
    public int gameTypeIndex;
    public int gameStateIndex;
    public int turnStateIndex;
    public int moveIndex;
    public int increasePathIndex;

    public float moveSpeed;

    public bool isMove;

    // Start is called before the first frame update
    void Start()
    {        
        SetTurnIndex();
        GameFormat();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStateIndex == (int)GameState.Start)
        {
            if (Input.GetMouseButtonDown(0))
            {               
                // Creates a Ray from the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    print(hit.transform.gameObject.tag);
                    if (hit.transform.gameObject.tag == "Pawn" && turnStateIndex == (int)TurnState.TurnPlay && hit.transform.gameObject.GetComponent<PlayerInfo>().teamIndex == currentTurnIndex)
                    {
                        if (hit.transform.gameObject.GetComponent<PlayerInfo>().isActive)
                        {
                            isMove = true;
                            selectedPawn = hit.transform.gameObject;
                            increasePathIndex = 0;
                        }
                        else
                        {
                            if (moveIndex == 0 || moveIndex == 5)
                            {
                                selectedPawn = hit.transform.gameObject;                                
                                SetOrigin();
                            }
                        }
                    }
                    else if (hit.transform.gameObject.tag == "Dice" && turnStateIndex == (int)TurnState.TurnStart)
                    {
                        turnStateIndex = (int)TurnState.TurnPlay;
                        PlayDiceAnimation();
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

    public void SetTurnIndex()
    {
        gameTypeIndex = 0;

        if (gameTypeIndex == 0)
        {
            currentTurnIndex = UnityEngine.Random.RandomRange(0, 2);
        }
        else
        {
            currentTurnIndex = UnityEngine.Random.RandomRange(0, 4);
        }        
    }

    public void GameFormat()
    {
        gameStateIndex = (int)GameState.Start;
        moveSpeed = 4f;

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
        }

        TurnForamt();
    }

    public void TurnForamt()
    {
        increasePathIndex = 0;
        moveIndex = 0;
        turnStateIndex = (int)TurnState.TurnStart;
        dice.GetComponent<Animator>().enabled = false;

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

    public void PlayDiceAnimation()
    { 
        moveIndex = UnityEngine.Random.RandomRange(0, 6);

        switch (moveIndex)
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
                            CheckToKillPawn();
                            ChangeTurn();
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
                            CheckToKillPawn();
                            ChangeTurn();
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
                            CheckToKillPawn();
                            ChangeTurn();
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
                            CheckToKillPawn();
                            ChangeTurn();
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
        if (gameTypeIndex == 0)
        {
            currentTurnIndex = (currentTurnIndex + 1) % 2;
        }
        else
        {
            currentTurnIndex = (currentTurnIndex + 1) % 4;
        }

        TurnForamt();        
    }

    public void SetOrigin()
    {
        switch (currentTurnIndex)
        {
            case 0:
                {
                    selectedPawn.transform.position = originObjects[0].transform.position;                    
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[0];
                    break;
                }
            case 1:
                {
                    selectedPawn.transform.position = originObjects[1].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[1];
                    break;
                }
            case 2:
                {
                    selectedPawn.transform.position = originObjects[2].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[2];
                    break;
                }
            case 3:
                {
                    selectedPawn.transform.position = originObjects[3].transform.position;
                    selectedPawn.GetComponent<PlayerInfo>().currentPosTile = originObjects[3];
                    break;
                }
        }

        selectedPawn.GetComponent<PlayerInfo>().isActive = true;

        if (moveIndex == 0)
        {
            ChangeTurn();
        }
        else
        {
            turnStateIndex = (int)TurnState.TurnStart;
        }
    }

    public void CheckToKillPawn()
    {
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
                        player1Pawns[i].transform.position = originObjects[0].transform.position;
                        player1Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
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
                        player2Pawns[i].transform.position = originObjects[1].transform.position;
                        player2Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
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
                        player3Pawns[i].transform.position = originObjects[2].transform.position;
                        player3Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
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
                        player4Pawns[i].transform.position = originObjects[3].transform.position;
                        player4Pawns[i].GetComponent<PlayerInfo>().currentPosIndex = 0;
                    }
                }
            }
        }
    }

}
