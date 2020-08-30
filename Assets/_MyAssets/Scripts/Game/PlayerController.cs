using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//Version: 0.41

public abstract class PlayerController : MonoBehaviour
{
    protected Player m_playerData;
    public byte playerID;
    public GameState gameState;

    private void Awake()
    {
        m_playerData = new Player();
    }

    public void PreTurnStart()
    {
        m_playerData.ClearHandCalls();
    }

    public void AddTile(Tile tile)
    {
        m_playerData.hand.tiles.Add(tile);

        m_playerData.EnableClosedKan(tile);
        m_playerData.EnableLateKan(tile);
        m_playerData.EnableTsumo(tile);

        //Add tile to the display

        OnTileAdded(tile);
    }

    public void RemoveTile(Tile tile)
    {
        //Ask game state if it's my turn
        if(!gameState.CanPlayerDiscard(playerID))
            return;

        m_playerData.hand.tiles.Remove(tile);
        m_playerData.waitingTiles = HandHelper.FindWaitingTiles(m_playerData.hand);

        //Remove tile in the display
        DiscardedTile discardedTile = new DiscardedTile(tile, playerID);
        OnTileDiscarded(discardedTile);

        //Tell game state of discarded tile
        gameState.OnPlayerDiscarded(discardedTile);
    }

    public void RequestSkipHandCalls()
    {
        //Tell game state to request hand call
        OnHandCallRequested(HandCall.None);
        gameState.RequestHandCall(this, HandCall.None, () => {});
    }

    public void RequestChi(int index)
    {

        //Tell game state to request hand call
        OnHandCallRequested(HandCall.Chi);
        gameState.RequestHandCall(this, HandCall.Chi, () => { CallChi(index); });
    }

    public void RequestPon()
    {
        //Tell game state to request hand call
        OnHandCallRequested(HandCall.Pon);
        gameState.RequestHandCall(this, HandCall.Pon, CallPon);
    }

    public void RequestKan()
    {
        //Tell game state to request hand call
        OnHandCallRequested(HandCall.Kan);
        gameState.RequestHandCall(this, (m_playerData.handCalls & HandCall.Kan) | (m_playerData.handCalls & HandCall.LateKan), CallKan);
    }

    public void RequestRon()
    {
        //Tell game state to request hand call
        OnHandCallRequested(HandCall.Ron);
        gameState.RequestHandCall(this, HandCall.Ron, () => { });
    }

    public void RequestTsumo()
    {
        //Tell game state to request hand call
        OnHandCallRequested(HandCall.Tsumo);
        gameState.RequestHandCall(this, HandCall.Tsumo, () => { });
    }

    public void ResetHand(List<Tile> tiles)
    {
        m_playerData.ResetHand(tiles);
        //Update hand UI
        OnHandReset(tiles);
    }

    public bool EnableHandCalls(DiscardedTile discardedTile, bool checkChi)
    {
        if(checkChi)
            m_playerData.EnableChi(discardedTile);
        m_playerData.EnablePon(discardedTile);
        m_playerData.EnableOpenKan(discardedTile);
        m_playerData.EnableRon(discardedTile);

        bool enabledHandCalls = m_playerData.handCalls != HandCall.None;
        if(enabledHandCalls)
            OnHandCallEnabled(m_playerData.handCalls);
        return enabledHandCalls;
    }

    public bool CanStealLateKan(DiscardedTile tile)
    {
        m_playerData.EnableRon(tile);
        return (m_playerData.handCalls & HandCall.Ron) == HandCall.Ron;
    }

    public void NotifyMeldCreated()
    {
        Meld meld = m_playerData.hand.melds[m_playerData.hand.melds.Count - 1];
        //Add meld to display

        OnMeldCreated(meld);
    }
    
    private void CallChi(int index)
    {
        Meld meld = m_playerData.potentialMelds.chiMelds[index];
        m_playerData.AddMeld(meld);
    }
    
    private void CallPon()
    {
        Meld meld = m_playerData.potentialMelds.ponMeld;
        m_playerData.AddMeld(meld);
    }
    
    private void CallKan()
    {
        Meld meld = m_playerData.potentialMelds.kanMeld;
        if((m_playerData.handCalls & HandCall.LateKan) == HandCall.LateKan)
            m_playerData.hand.melds.RemoveAll((Meld compare) => { return compare.tiles[0] == m_playerData.potentialMelds.kanMeld.tiles[0] && compare.type == Meld.Type.Triple; });

        m_playerData.AddMeld(meld);
    }

    public abstract void OnCanStealLateKan(DiscardedTile tile);
    protected abstract void OnTileAdded(Tile tile);
    protected abstract void OnTileDiscarded(DiscardedTile discardedTile);
    protected abstract void OnHandCallEnabled(HandCall handCalls);
    protected abstract void OnHandReset(List<Tile> tiles);
    protected abstract void OnHandCallRequested(HandCall handCall);
    protected abstract void OnMeldCreated(Meld meld);
}
