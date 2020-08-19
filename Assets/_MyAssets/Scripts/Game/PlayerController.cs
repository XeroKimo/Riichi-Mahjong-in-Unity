using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//Version: 0.34

public abstract class PlayerController : MonoBehaviour
{
    protected Player m_playerData;
    public byte playerID;
    public GameState gameState;

    private void Awake()
    {
        m_playerData = new Player();
    }

    public void AddTile(Tile tile)
    {
        m_playerData.AddTile(tile);

        //Add tile to the display

        OnTileAdded(tile);
    }

    public void RemoveTile(Tile tile)
    {
        //Ask game state if it's my turn

        m_playerData.RemoveTile(tile);

        //Remove tile in the display
        DiscardedTile discardedTile = new DiscardedTile(tile, playerID);
        OnTileDiscarded(discardedTile);

        //Tell game state of discarded tile
    }

    public void SkipHandCalls()
    {
        m_playerData.SkipHandCalls();
        //Tell game state of hand call
    }

    public void CallChi(int index)
    {
        m_playerData.CallChi(index);
        //Add meld to display
        //Tell game state of hand call
    }

    public void CallPon()
    {
        m_playerData.CallPon();
        //Add meld to display
        //Tell game state of hand call
    }

    public void CallKan()
    {
        Player.HandCall oldHandCalls = m_playerData.handCalls;
        m_playerData.CallKan();

        //Add meld to display
        //Tell game state of hand call
        //Check if late kan flag was set, if set, tell game state
        //That was the hand call we used instead of normal kan flag
    }

    public void CallRon()
    {
        m_playerData.CallRon();
        //Tell game state of hand call
    }

    public void CallTsumo()
    {
        if((m_playerData.handCalls & Player.HandCall.Tsumo) != Player.HandCall.Tsumo)
            return;

        m_playerData.CallTsumo();
        //Tell game state of hand call
    }

    public void ResetHand(List<Tile> tiles)
    {
        m_playerData.ResetHand(tiles);
        OnHandReset(tiles);
    }

    public bool EnableHandCalls(DiscardedTile discardedTile, bool checkChi)
    {
        bool enabledHandCalls = m_playerData.EnableHandCalls(discardedTile, checkChi);
        if(enabledHandCalls)
            OnHandCallEnabled(m_playerData.handCalls);
        return enabledHandCalls;
    }

    protected abstract void OnTileAdded(Tile tile);
    protected abstract void OnTileDiscarded(DiscardedTile discardedTile);
    protected abstract void OnHandCallEnabled(Player.HandCall handCalls);
    protected abstract void OnHandReset(List<Tile> tiles);
}
