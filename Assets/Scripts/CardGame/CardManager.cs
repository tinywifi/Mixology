using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] CardStack mainStack;
    [SerializeField] CardStack playerStack;
    [SerializeField] CardStack opponentStack;
    [SerializeField] PlayerLocal player1;
    [SerializeField] PlayerLocal player2;

    [Header("Board")]
    [SerializeField] CardStack leftStack;
    [SerializeField] CardStack rightStack;
    [SerializeField] RectTransform playerBoard;
    [SerializeField] RectTransform opponentBoard;
    [SerializeField] Text infos;
    [SerializeField] Button leaveBtn;

    NetGameManager network;
    bool pauseGame;

    void Awake()
    {
        network = GetComponent<NetGameManager>();
    }

    
    IEnumerator Start()
    {
        player1.IsLocked = player2.IsLocked = true;
        pauseGame = true;
        infos.text = "";
        leaveBtn.gameObject.SetActive(false);

        yield return new WaitUntil(() => network.Timer > 0);

        MainDeck.instance.MixCards(network.RoomAlea());

        if (network.IsOpponentPlayer) 
        {
            CardStack tempStack = playerStack;
            playerStack = opponentStack;
            opponentStack = tempStack;

            RectTransform tempTsf = playerBoard;
            playerBoard = opponentBoard;
            opponentBoard = tempTsf;

            tempStack = leftStack;
            leftStack = rightStack;
            rightStack = tempStack;
        }

        yield return new WaitForSeconds(network.Timer);

        yield return ServeCards();

        yield return ServeBoard();

        yield return StartGame();
    }

    

    IEnumerator ServeCards()
    {
        while (mainStack.Count > 0)
        {
            mainStack.ServeTo(playerStack);
            yield return new WaitForSeconds(0.25f);

            mainStack.ServeTo(opponentStack);
            yield return new WaitForSeconds(0.25f);

        }
    }

    IEnumerator ServeBoard()
    {
        int currentBoard = 0;
        CardStack player = playerBoard.GetChild(0).GetComponent<CardStack>();
        CardStack opponent = opponentBoard.GetChild(0).GetComponent<CardStack>();

        while (currentBoard < 5)
        {
            playerStack.ServeTo(player, true);
            opponentStack.ServeTo(opponent, true);
            yield return new WaitForSeconds(0.25f);

            if (player.IsFull)
            {
                currentBoard++;

                if (currentBoard < playerBoard.childCount)
                {
                    player = playerBoard.GetChild(currentBoard).GetComponent<CardStack>();
                    opponent = opponentBoard.GetChild(currentBoard).GetComponent<CardStack>();
                }
            }
        }
    }

    private IEnumerator StartGame()
    {
        for (int i = 0; i < 5; i++)
        {
            CardStack player = playerBoard.GetChild(i).GetComponent<CardStack>();
            CardStack opponent = opponentBoard.GetChild(i).GetComponent<CardStack>();

            player.Evaluate();
            opponent.Evaluate();
            yield return new WaitForSeconds(0.25f);

        }

        infos.text = "Pret ?";

        yield return new WaitForSeconds(2f);

        PlayerService();

    }

    private void PlayerService()
    {
        infos.text = "Go !";
        playerStack.ServeTo(rightStack, false, PlayersReady);
        opponentStack.ServeTo(leftStack);
    }

    private void PlayersReady()
    {
        player1.PlayerCanPlay();
        player2.PlayerCanPlay();
        player1.IsLocked = player2.IsLocked = false;
        pauseGame = false;
        infos.text = "";
    }

    private IEnumerator AwaitDeck()
    {
        pauseGame = true;
        network.ResetTime();

        playerStack.Selection();
        opponentStack.Selection();

        player1.IsReady = player2.IsReady = false;
        player1.IsLocked = player2.IsLocked = true;

        infos.text = "Nouvelle carte";

        yield return new WaitUntil(() => player1.IsReady && player2.IsReady);

        playerStack.Deselect();
        opponentStack.Deselect();

        yield return new WaitUntil(() => network.Timer > 0);

        yield return new WaitForSeconds(network.Timer);

        PlayerService();
    }

    void SetWinner(string message)
    {
        pauseGame = true;
        infos.text = message;
        player1.IsLocked = player2.IsLocked = true;
        leaveBtn.gameObject.SetActive(true);
    }


    
    void Update()
    {
        if (pauseGame)
            return;

        if (player1.Count == 0 || player2.Count == 0)
        {
            SetWinner(((player1.Count == 0) ? network.Me : network.Opponent) + " gagnant !");
        }

        if (!player1.CanPlay && !player2.CanPlay)
        {
            if (playerStack.Count == 0 && opponentStack.Count == 0)
            {
                SetWinner("Egalite !");
            }
            else
                StartCoroutine("AwaitDeck");
        }
    }
}
