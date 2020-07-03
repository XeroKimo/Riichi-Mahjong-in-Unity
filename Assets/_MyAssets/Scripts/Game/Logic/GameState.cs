using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Flags]
public enum MahjongFlags
{
    HP_Riichi = 0x0001,
    HP_DoubleRiichi = 0x0002,
    HP_LateKan = 0x0004,
    HP_Ippatsu = 0x0008,
    HP_RinshanKaihou = 0x0010,
    HP_NagashiMangan = 0x0020,
    HP_KokushiMusou = 0x0040,
    HP_Ryanpeikou = 0x0080,

    FU_SingleTileWait = 0x0100,

    DP_FirstTile = 0x0200,
    DP_LastTile = 0x0400,
}

public class GameState : IGameCallbacks
{
    IEventCallbacks m_eventCallbacks;

    List<Player> m_players;
    byte m_currentPlayerIndex;
    byte m_currentDealerIndex;
    byte m_initialDealerIndex;
    byte m_prevalentWind;

    byte m_winnerIndex;
    byte m_countOfPlayersWithCallFlags;

    Deck m_deck;
    DiscardPile m_discardPile;

    public void AddPlayer(IPlayerEventCallbacks playerCallbacks)
    {
        m_players.Add(new Player(this, playerCallbacks, (byte)m_players.Count));
    }
    public void StartGame()
    {
        m_prevalentWind = 255;  //When game starts gets called, wind rolls over to east
        StartNextRound();
    }



    public byte currentPlayerIndex { get => m_currentPlayerIndex; }
    public byte currentDealerIndex { get => m_currentDealerIndex; }
    public Tile.Face prevalentWind { get => (Tile.Face)m_prevalentWind; }
    public IPlayerActions GetPlayer(int index) { return m_players[index]; }
    public int GetRemainingTileCount() { return m_deck.GetRemainingTileCount(); }



    public void OnTileDiscarded(Tile tile)
    {
        m_discardPile.AddTile(tile);

        byte countEnabledCallFlags = 0;
        for(int i = 0; i < m_players.Count; i++)
        {
            if(i != m_currentPlayerIndex)
            {
                if(m_players[i].EnableHandCalls(tile, m_currentPlayerIndex))
                    countEnabledCallFlags++;
            }
        }

        m_countOfPlayersWithCallFlags = countEnabledCallFlags;
        if(m_countOfPlayersWithCallFlags < 1)
            StartNextTurn();
    }
    public void OnHandCallMade(byte playerID, HandCall handCall)
    {
        switch(handCall)
        {
        case HandCall.None:
            SkipCalls();
            break;
        case HandCall.Chi:
        case HandCall.Pon:
            m_currentPlayerIndex = playerID;
            break;
        case HandCall.Kan:
            m_currentPlayerIndex = playerID;
            m_players[playerID].AddTile(m_deck.DrawDeadTile());
            break;
        case HandCall.Ron:
            break;
        case HandCall.Tsumo:
            break;
        default:
            break;
        }
    }
    public bool IsCurrentTurn(byte playerID)
    {
        return m_currentPlayerIndex == playerID;
    }



    private void SelectInitialDealer()
    {
        m_initialDealerIndex = (byte)((Random.Range(0, 12) % 4));
        m_currentDealerIndex = (byte)(m_initialDealerIndex - 1);    //Game start gets called after this function, which will increment the current Dealer
        m_winnerIndex = m_initialDealerIndex;
    }

    private void StartNextWind()
    {
        m_prevalentWind++;
    }
    public void StartNextRound()
    {
        if(m_winnerIndex == noWinner)
            return;
        if(DealerChanged(m_winnerIndex))
        {
            if(m_currentDealerIndex == m_initialDealerIndex)
                StartNextWind();
        }

        ResetDeck();
        ResetHands();

        m_currentPlayerIndex = (byte)(m_currentDealerIndex - 1);
        m_winnerIndex = noWinner;
        StartNextTurn();
    }
    private void StartNextTurn()
    {
        m_currentPlayerIndex = (byte)((m_currentPlayerIndex + 1) % 4);
        if(!m_deck.IsEmpty())
            m_players[m_currentPlayerIndex].AddTile(m_deck.DrawTile());
        else
            CheckPlayerTenpais();
    }
    private bool DealerChanged(byte winnerIndex)
    {
        if(winnerIndex == m_currentDealerIndex)
            return false;

        m_currentDealerIndex = (byte)((m_currentDealerIndex + 1) % 4);

        return true;
    }
    private void CheckPlayerTenpais()
    {
        m_winnerIndex = m_currentDealerIndex;
        StartNextRound();
    }

    private void ResetDeck()
    {
        m_deck.ShuffleDeck();
        m_deck.BreakDeck(m_currentDealerIndex, m_players.Count);
    }
    private void ResetHands()
    {
        Hand[] hands = new Hand[m_players.Count];

        for(int i = 0; i < hands.Length; i++)
        {
            hands[i].tiles = new List<Tile>();
            hands[i].melds = new List<Meld>();
        }

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < hands.Length; i++)
            {
                hands[j].tiles.AddRange(m_deck.DrawMultipleTiles(4));
            }
        }

        for(int i = 0; i < hands.Length; i++)
        {
            hands[i].tiles.Add(m_deck.DrawTile());
            m_players[i].ResetHand(hands[i]);
        }
    }

    private void SkipCalls()
    {
        if(m_countOfPlayersWithCallFlags < 1)
            return;
        m_countOfPlayersWithCallFlags--;
        if(m_countOfPlayersWithCallFlags < 1)
            StartNextTurn();
    }

    public GameState(IEventCallbacks gameStateCallbacks)
    {
        m_eventCallbacks = gameStateCallbacks;
        m_players = new List<Player>();
        m_deck = new Deck();
        m_discardPile = new DiscardPile();
    }

    public interface IEventCallbacks
    {
        void OnKanCalled();
        void OnPlayerWin(byte winnerIndex, HandCall handCall, PointHandOut points, List<byte> payeeIndex);
    }

    const int noWinner = 255;
}

public struct PointHandOut
{
    int nonDealerPoints;
    int dealerPoints;
}