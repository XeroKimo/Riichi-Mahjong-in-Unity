using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoGameState : MonoBehaviour, IGameLogicCallbacks
{
    List<Player> m_players;
    public List<PlayerDisplayManager> playerDisplays;
    DiscardPile m_discardPile;
    Deck m_deck;

    int m_currentPlayer;
    int m_currentDealer;
    int m_initialDealer;

    Tile.Face m_currentWind;

    int m_hasHandCallsAvailable;

    public void Awake()
    {
        m_discardPile = new DiscardPile();
        m_deck = new Deck();
        m_hasHandCallsAvailable = 0;
    }

    public void StartGame()
    {
        m_currentWind = (Tile.Face)((int)Tile.Face.East - 1);

        for(int i = 0; i < 4; i++)
        {
            m_players[i].gameCallbacks = this;
        }

        RandomizePlayers();
        SelectDealer();
        StartNextRound(m_initialDealer);
    }

    void RandomizePlayers()
    {
        for(int i = 0; i < 4; i++)
        {
            SwapPlayers(i, Random.Range(i, 4));
        }
    }

    public void StartNextTurn()
    {
        m_currentPlayer = (m_currentPlayer + 1) % 4;
        m_players[m_currentPlayer].AddTileToHand(m_deck.DrawTile());
    }

    void StartNextWind()
    {
        m_currentWind = (Tile.Face)((int)m_currentWind + 1);
        StartNextRound(m_currentDealer);
    }

    void StartNextRound(int winner)
    {
        if(winner != m_currentDealer)
        {
            m_currentPlayer = ++m_currentDealer;
            if((winner + 1) % 4 == m_initialDealer)
                StartNextWind();
        }
        ResetDeck();
        ResetHands();
    }

    void ResetDeck()
    {
        m_deck.ShuffleDeck();
        m_deck.BreakDeck(m_currentDealer);
    }

    void SwapPlayers(int left, int right)
    {
        Player player = m_players[left];
        m_players[left] = m_players[right];
        m_players[right] = player;
    }

    void SelectDealer()
    {
        m_currentDealer = m_currentPlayer = (Random.Range(0, 12) % 4) - 1;
        m_initialDealer = m_currentPlayer + 1;
    }

    void ResetHands()
    {
        List<Tile>[] playerTiles = new List<Tile>[4];
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                for(int k = 0; k < 3; k++)
                {
                    playerTiles[j].Add(m_deck.DrawTile());
                }
            }
        }

        for(int j = 0; j < 4; j++)
        {
            playerTiles[j].Add(m_deck.DrawTile());
        }

        playerTiles[0].Add(m_deck.DrawTile());

        for(int i = 0; i < 4; i++)
        {
            m_players[(m_currentDealer + i) % 4].ResetHand(playerTiles[i]);
        }
    }

    public void OnTileRemoved(Player player, Tile tile)
    {
        int hasHandCallsEnabled = 0;
        for(int i = 0; i < 4; i++)
        {
            if(i == (m_currentPlayer + 1) % 4)
            {
                if(m_players[i].EnableHandCalls(tile, true, false))
                {
                    hasHandCallsEnabled++;
                }
            }
            else if(i != m_currentPlayer)
            {
                if(m_players[i].EnableHandCalls(tile, false, false))
                {
                    hasHandCallsEnabled++;
                }
            }
        }
        m_hasHandCallsAvailable = hasHandCallsEnabled;

        if(m_hasHandCallsAvailable == 0)
            StartNextTurn();
    }

    public void OnHandCallMade(Player player, HandCalls handCall)
    {
        int callerIndex = m_players.FindIndex((Player other) => { return other == player; });
        if(handCall.IsSet(HandCalls.Flag.Chi) || handCall.IsSet(HandCalls.Flag.Pon) || handCall.IsSet(HandCalls.Flag.Kan))
        {
            playerDisplays[callerIndex].TransferTileIn(playerDisplays[m_currentPlayer].ExtractLastDiscardedTile());
            m_currentPlayer = callerIndex;
        }
        else if(handCall.IsSet(HandCalls.Flag.Ron))
        {
            m_players[m_currentPlayer].RemovePoints(0);
            m_players[callerIndex].AddPoints(0);
            StartNextRound(callerIndex);
        }
        else if(handCall.IsSet(HandCalls.Flag.Tsumo))
        {
            for(int i = 0; i < 4; i++)
            {
                if(i == callerIndex)
                {
                    m_players[callerIndex].AddPoints(0);
                }
                else
                {
                    m_players[i].RemovePoints(0);
                }
            }
            StartNextRound(callerIndex);
        }
    }

    public bool IsCurrentPlayer(Player player)
    {
        return m_players.FindIndex((Player other) => { return other == player; }) == m_currentPlayer;
    }
}
