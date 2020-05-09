using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoGameState : MonoBehaviour, IGameLogicCallbacks
{
    public List<Player> m_players;
    public List<PlayerDisplayManager> playerDisplays;
    DiscardPile m_discardPile;
    [SerializeField]
    Deck m_deck;

    int m_currentPlayerIndex;
    int m_currentDealerIndex;
    int m_initialDealerIndex;

    Tile.Face m_currentWind;

    int m_hasHandCallsAvailable;

    Tile m_lastDiscardedTile = null;

    void Awake()
    {
        m_discardPile = new DiscardPile();
        m_deck = new Deck();
        m_hasHandCallsAvailable = 0;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        m_currentWind = (Tile.Face)((int)Tile.Face.East - 1);

        for(int i = 0; i < 4; i++)
        {
            m_players[i].gameCallbacks = this;
        }

        RandomizePlayers();
        SelectInitialDealer();
        StartNextRound(m_initialDealerIndex);
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
        m_currentPlayerIndex = (m_currentPlayerIndex + 1) % 4;
        if(!m_deck.Empty())
        {
            Tile drawnTile = m_deck.DrawTile();
            m_players[m_currentPlayerIndex].AddTileToHand(drawnTile);
            playerDisplays[m_currentPlayerIndex].OnTileAdded(m_players[m_currentPlayerIndex], drawnTile);
        }
        else
            StartNextRound(m_currentDealerIndex);   //TODO: add tenpai checks
    }

    void StartNextWind()
    {
        m_currentWind = (Tile.Face)((int)m_currentWind + 1);
    }

    void StartNextRound(int winnerIndex)
    {
        m_lastDiscardedTile = null;
        ChangeDealers(winnerIndex);
        ResetDeck();
        ResetHands();
        StartNextTurn();
    }

    void ResetDeck()
    {
        m_deck.ShuffleDeck();
        m_deck.BreakDeck(m_currentDealerIndex);
    }

    void SwapPlayers(int left, int right)
    {
        Player player = m_players[left];
        m_players[left] = m_players[right];
        m_players[right] = player;
    }

    void SelectInitialDealer()
    {
        m_currentDealerIndex = m_currentPlayerIndex = (Random.Range(0, 12) % 4) - 1;
        m_initialDealerIndex = m_currentPlayerIndex + 1;
    }

    void ResetHands()
    {
        foreach(var playerDisplay in playerDisplays)
        {
            playerDisplay.CreateHand();
        }
        List<Tile>[] playerTiles = new List<Tile>[4];

        playerTiles[0] = new List<Tile>();
        playerTiles[1] = new List<Tile>();
        playerTiles[2] = new List<Tile>();
        playerTiles[3] = new List<Tile>();

        for(int i = 0; i < 3; i++)
        {
            for(int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                playerTiles[playerIndex].AddRange(m_deck.DrawMultipleTiles(4));
            }
        }

        for(int j = 0; j < 4; j++)
        {
            playerTiles[j].Add(m_deck.DrawTile());
        }

        for(int i = 0; i < 4; i++)
        {
            int playerIndex = (m_currentDealerIndex + i) % 4;
            Player player = m_players[playerIndex];

            player.ResetHand(playerTiles[i]);
            playerDisplays[playerIndex].RefreshHand(player, player.m_playerData.hand.tiles);
        }
    }

    void ChangeDealers(int winnerIndex)
    {
        if(winnerIndex != m_currentDealerIndex)
        {
            m_currentDealerIndex = (m_currentDealerIndex + 1) % 4;
            m_currentPlayerIndex = m_currentDealerIndex - 1;
            if(m_currentDealerIndex == m_initialDealerIndex)
                StartNextWind();
        }
    }

    public void OnTileRemoved(Player player, Tile tile)
    {
        int hasHandCallsEnabled = 0;
        playerDisplays[m_currentPlayerIndex].OnTileRemoved(player, tile);
        playerDisplays[m_currentPlayerIndex].RefreshHand(player, player.m_playerData.hand.tiles);

        m_lastDiscardedTile = tile;
        for(int i = 0; i < 4; i++)
        {
            if(i == (m_currentPlayerIndex + 1) % 4)
            {
                if(m_players[i].EnableHandCalls(tile, true, false))
                {
                    hasHandCallsEnabled++;
                }
            }
            else if(i != m_currentPlayerIndex)
            {
                if(m_players[i].EnableHandCalls(tile, false, false))
                {
                    hasHandCallsEnabled++;
                }
            }
        }
        m_hasHandCallsAvailable = hasHandCallsEnabled;

        if(m_hasHandCallsAvailable < 1)
            StartNextTurn();
    }

    public void OnHandCallMade(Player player, Meld meld)
    {
        int callerIndex = m_players.FindIndex((Player other) => { return other == player; });

        if(callerIndex != m_currentPlayerIndex)
        {
            playerDisplays[callerIndex].TransferTileIn(playerDisplays[m_currentPlayerIndex].ExtractLastDiscardedTile());
            m_currentPlayerIndex = callerIndex;
        }

        playerDisplays[callerIndex].OnMeldMade(player, meld);
        playerDisplays[callerIndex].RefreshHand(player, player.m_playerData.hand.tiles);
    }

    public bool IsCurrentPlayer(Player player)
    {
        return m_players.FindIndex((Player other) => { return other == player; }) == m_currentPlayerIndex;
    }

    public void OnWinningCallMade(Player player, HandCall handCall)
    {
        int callerIndex = m_players.FindIndex((Player other) => { return other == player; });
        if((handCall & HandCall.Ron) == HandCall.Ron)
        {
            m_players[m_currentPlayerIndex].points -= 0;
            m_players[callerIndex].points += 0;
            StartNextRound(callerIndex);
        }
        else if((handCall & HandCall.Tsumo) == HandCall.Tsumo)
        {
            for(int i = 0; i < 4; i++)
            {
                if(i == callerIndex)
                {
                    m_players[callerIndex].points += 0;
                }
                else
                {
                    m_players[m_currentPlayerIndex].points -= 0;
                }
            }
            StartNextRound(callerIndex);
        }
    }

    public void SkipHandCall()
    {
        m_hasHandCallsAvailable--;
        if(m_hasHandCallsAvailable < 1)
            StartNextTurn();
    }

    //Pseudo stuff
    public void SkipHandCalls()
    {
        for(int i = 0; i < 4; i++)
        {
            m_players[i].SkipHandCall();
        }
    }
    public void CallKan()
    {
        for(int i = 0; i < 4; i++)
        {
            m_players[i].CallKan();
        }
    }
    public void CallPon()
    {
        for(int i = 0; i < 4; i++)
        {
            m_players[i].CallPon();
        }
    }
    public void CallChi()
    {
        for(int i = 0; i < 4; i++)
        {
            m_players[i].CallChi(0);
        }
    }
}
