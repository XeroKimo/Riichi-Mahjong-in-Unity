using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameLogicCallbacks
{
    void OnTileRemoved(Player player, Tile tile);

    void OnHandCallMade(Player player, HandCalls handCall);

    bool IsCurrentPlayer(Player player);

    void StartNextTurn();
}

public interface IHandDisplayCallbacks
{
    void OnTileAdded(Player player, Tile tile);
    void OnTileRemoved(Player player, Tile tile);
    void RefreshHand(Player player, List<Tile> tile);
    void OnHandCallMade(Player player, List<Tile> tile, HandCalls handCall);
}

public class Player : MonoBehaviour
{
    PlayerData m_playerData;
    public IHandDisplayCallbacks handDisplayCallbacks;
    public IGameLogicCallbacks gameCallbacks;

    Tile m_lastDiscardedTile;

    private void Awake()
    {
        m_playerData = new PlayerData();
    }

    public void AddTileToHand(Tile tile)
    {
        m_playerData.hand.AddTileToHand(tile);

        handDisplayCallbacks.OnTileAdded(this, tile);

        EnableHandCalls(tile, false, true);
    }

    public bool RemoveTileFromHand(Tile tile)
    {
        if(!IsCurrentPlayer())
            return false;

        if(!m_playerData.hand.RemoveTileFromHand(tile))
            return false;

        handDisplayCallbacks.OnTileRemoved(this, tile);
        gameCallbacks.OnTileRemoved(this, tile);

        RefreshHand();

        return true;
    }

    public bool EnableHandCalls(Tile tile, bool enableChi, bool isMyTurn)
    {
        
        if(isMyTurn)
        {
            //Enable Kan
            //Enable Tsumo
        }
        else
        {
            m_lastDiscardedTile = tile;
            if(m_lastDiscardedTile == null)
                return false;

            if(enableChi)
            {
                //Enable Chi
            }
            //Enable Pon
            //Enable Kan
            //Enable Ron
        }

        return false;
    }

    public void ResetHand(List<Tile> hand)
    {
        m_playerData.hand.ResetHand(hand);

        RefreshHand();
    }

    public void AddPoints(int points)
    {
        m_playerData.points += points;
    }

    public void RemovePoints(int points)
    {
        m_playerData.points -= points;
    }

    public int GetPoints()
    {
        return m_playerData.points;
    }

    private void RefreshHand()
    {
        handDisplayCallbacks?.RefreshHand(this, m_playerData.hand.GetTiles());
    }

    //public void CallChi()
    //{
    //    if(m_playerData.hand.CanMakeSequence(null, out Tile[] tiles))
    //    {
    //        handDisplayCallbacks.OnHandCallMade(this, new List<Tile>(tiles), HandCalls.Chi);
    //        gameCallbacks.OnHandCallMade(this, HandCalls.Chi);
    //    }

    //    gameCallbacks.StartNextTurn();
    //}

    public void CallPon()
    {
        if(m_playerData.hand.CanMakeTriple(null, out Tile[] tiles))
        {
            gameCallbacks.OnHandCallMade(this, HandCalls.Pon);
            handDisplayCallbacks.OnHandCallMade(this, new List<Tile>(tiles), HandCalls.Pon);
        }
    }

    public bool IsCurrentPlayer()
    {
        return gameCallbacks.IsCurrentPlayer(this);
    }

    public void CreateMeld(Tile input)
    {

    }
}
