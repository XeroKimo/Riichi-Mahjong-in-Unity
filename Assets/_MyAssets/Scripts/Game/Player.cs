using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameLogicCallbacks
{
    void OnTileRemoved(Player player, Tile tile);

    //Potentially seperate this function into 2. For making melds, and for winning hand calls
    void OnHandCallMade(Player player);

    void OnWinningCallMade(Player player, HandCall handCall);

    bool IsCurrentPlayer(Player player);

    void SkipHandCall();
}

public interface IHandDisplayCallbacks
{
    void OnTileAdded(Player player, Tile tile);
    void OnTileRemoved(Player player, Tile tile);
    void RefreshHand(Player player, List<Tile> tile);
    void OnMeldMade(Player player, Meld meld, Tile input);
}

public class Player : MonoBehaviour
{
    public PlayerData m_playerData { get; private set; }
    public IGameLogicCallbacks gameCallbacks;

    Dictionary<HandCall, List<Meld>> m_cachedMelds;

    public int points { get => m_playerData.points; set => m_playerData.points = value; }
    bool m_hasDiscardTile = false;

    private void Awake()
    {
        m_playerData = new PlayerData();
        m_cachedMelds = new Dictionary<HandCall, List<Meld>>();
        m_cachedMelds[HandCall.Chi] = new List<Meld>();
        m_cachedMelds[HandCall.Pon] = new List<Meld>();
        m_cachedMelds[HandCall.Kan] = new List<Meld>();
    }

    public void AddTileToHand(Tile tile)
    {
        m_playerData.hand.tiles.Add(tile);
        m_hasDiscardTile = false;

        EnableHandCalls(tile, false, true);
    }
    public bool RemoveTileFromHand(Tile tile)
    {
        if(m_hasDiscardTile)
            return false;

        if(!IsCurrentPlayer())
            return false;

        if(!m_playerData.hand.tiles.Remove(tile))
            return false;

        m_hasDiscardTile = true;

        gameCallbacks.OnTileRemoved(this, tile);

        ClearCachedMelds();
        RefreshHand();

        return true;
    }

    public bool EnableHandCalls(Tile tile, bool enableChi, bool isMyTurn)
    {
        if(isMyTurn)
        {
            EnableKan(tile, false);
        }
        else
        {
            if(enableChi)
            {
                EnableChi(tile);
            }
            EnablePon(tile);
            EnableKan(tile, true);
        }

        return m_playerData.handCalls != HandCall.None;
    }

    public void ResetHand(List<Tile> hand)
    {
        m_playerData.hand.tiles.Clear();
        m_playerData.hand.tiles.AddRange(hand);

        RefreshHand();
    }

    private void RefreshHand()
    {
        Dictionary<Tile.Suit, int> handArrangement = new Dictionary<Tile.Suit, int>();
        handArrangement[Tile.Suit.Bamboo] = 2;
        handArrangement[Tile.Suit.Character] = 0;
        handArrangement[Tile.Suit.Circle] = 1;
        handArrangement[Tile.Suit.Honor] = 3;
        m_playerData.hand.SortHand(handArrangement);
    }

    bool EnableChi(Tile input)
    {
        if(m_playerData.CanCallChi(input, out Meld meld))
        {
            m_cachedMelds[HandCall.Chi].Add(meld);
            Debug.Log("Chi Available");
            return true;
        }
        return false;
    }

    bool EnablePon(Tile input)
    {
        if(m_playerData.CanCallPon(input, out Meld meld))
        {
            m_cachedMelds[HandCall.Pon].Add(meld);
            Debug.Log("Pon Available");
            return true;
        }
        return false;
    }

    bool EnableKan(Tile input, bool open)
    {
        if(open)
        {
            if(m_playerData.CanCallOpenKan(input, out Meld meld))
            {
                m_cachedMelds[HandCall.Kan].Add(meld);
                Debug.Log("Kan available");
                return true;
            }
            return false;
        }
        else
        {
            if(m_playerData.CanCallClosedKan(out Meld meld))
            {
                m_cachedMelds[HandCall.Kan].Add(meld);
                Debug.Log("Kan available");
                return true;
            }
            else if(m_playerData.CanCallLateKan(out meld))
            {
                m_cachedMelds[HandCall.Kan].Add(meld);
                Debug.Log("Kan available");
                return true;
            }
            return false;
        }
    }

    public void CallChi(int index)
    {
        if((m_playerData.handCalls & HandCall.Chi) != HandCall.Chi)
            return;
        AddMeld(m_cachedMelds[HandCall.Chi][index]);
        OnHandCallMade();
        ClearCachedMelds();
    }

    public void CallPon()
    {
        if((m_playerData.handCalls & HandCall.Pon) != HandCall.Pon)
            return;
        AddMeld(m_cachedMelds[HandCall.Pon][0]);
        OnHandCallMade();
        ClearCachedMelds();
    }

    public void CallKan()
    {
        if((m_playerData.handCalls & HandCall.Kan) != HandCall.Kan)
            return;
        if((m_playerData.handCalls & HandCall.LateKan) == HandCall.LateKan)
        {
            Meld cachedMeld = m_cachedMelds[HandCall.Kan][0];
            List<Tile> newMeldTiles = new List<Tile>(cachedMeld.tiles);
            newMeldTiles.Add(m_playerData.hand.tiles[m_playerData.hand.tiles.Count - 1]);
            m_playerData.hand.melds.Remove(cachedMeld);
            m_playerData.hand.melds.Add(new Meld(newMeldTiles, true));
        }
        else
        {
            AddMeld(m_cachedMelds[HandCall.Kan][0]);
        }
        OnHandCallMade();
        ClearCachedMelds();
    }

    public void SkipHandCall()
    {
        if(gameCallbacks.IsCurrentPlayer(this))
            return;
        if(m_playerData.handCalls == HandCall.None)
            return;
        ClearCachedMelds();
        gameCallbacks.SkipHandCall();
    }

    public bool IsCurrentPlayer()
    {
        return gameCallbacks.IsCurrentPlayer(this);
    }

    private void AddMeld(Meld meld)
    {
        foreach(Tile tile in meld.tiles)
        {
            m_playerData.hand.tiles.Remove(tile);
        }

        m_playerData.hand.melds.Add(meld);
        m_hasDiscardTile = false;
    }

    private void ClearCachedMelds()
    {
        m_cachedMelds[HandCall.Chi].Clear();
        m_cachedMelds[HandCall.Pon].Clear();
        m_cachedMelds[HandCall.Kan].Clear();
        m_playerData.handCalls = HandCall.None;
    }

    private void OnHandCallMade()
    {
        gameCallbacks.OnHandCallMade(this);
    }

    private void OnWinningCallMade(HandCall handCall)
    {
        gameCallbacks.OnWinningCallMade(this, handCall);
    }
}
