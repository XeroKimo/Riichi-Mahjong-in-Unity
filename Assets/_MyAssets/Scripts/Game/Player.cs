using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Player : IPlayerActions
{
    public int points;
    private byte m_playerID;
    private bool m_hasDiscardedTile;

    private Hand m_hand;
    private HandCall m_callFlags;
    private CachedMelds m_futureMelds;

    private IGameCallbacks m_gameCallbacks;
    private IPlayerEventCallbacks m_eventCallbacks;

    //Public functions

    public void AddTile(Tile tile)
    {
        m_hasDiscardedTile = false;
        m_hand.tiles.Add(tile);
        m_eventCallbacks.OnTileAdded(tile);

        EnableHandCalls(tile);
    }
    public bool EnableHandCalls(Tile lastDiscardedTile, bool enableChi)
    {
        if(enableChi)
            EnableChi(lastDiscardedTile);
        EnablePon(lastDiscardedTile);
        EnableKan(lastDiscardedTile, true);
        EnableRon(lastDiscardedTile);

        bool hasFlagsSet = m_callFlags != HandCall.None;
        if(hasFlagsSet)
            m_eventCallbacks.OnHandCallAvailable(m_callFlags, m_futureMelds.chi);
        return hasFlagsSet;
    }
    public void ResetHand(Hand hand)
    {
        m_hand = hand;
        m_eventCallbacks.OnHandReset(hand);
    }

    //IPlayerActions functions

    public void RemoveTile(Tile tile)
    {
        if(m_hasDiscardedTile)
            return;
        if(!m_gameCallbacks.IsCurrentTurn(m_playerID))
            return;

        m_hasDiscardedTile = true;
        m_hand.tiles.Remove(tile);
        m_gameCallbacks.OnTileDiscarded(tile);
    }

    public void CallChi(int index)
    {
        if((m_callFlags & HandCall.Chi) != HandCall.Chi)
            return;

        Meld meld = m_futureMelds.chi[index];

        CreateMeld(meld);
        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Chi);
    }
    public void CallPon()
    {
        if((m_callFlags & HandCall.Pon) != HandCall.Pon)
            return;

        Meld meld = m_futureMelds.pon;

        CreateMeld(meld);
        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Pon);
    }
    public void CallKan()
    {
        if((m_callFlags & HandCall.Kan) != HandCall.Kan)
            return;

        Meld meld = m_futureMelds.kan;

        CreateMeld(meld);
        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Kan);
    }
    public void CallRon()
    {
        if((m_callFlags & HandCall.Ron) != HandCall.Ron)
            return;

        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Ron);
    }
    public void CallTsumo()
    {
        if((m_callFlags & HandCall.Ron) != HandCall.Ron)
            return;

        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Tsumo);
    }
    public void SkipCalls()
    {
        HandCall handCalls = m_callFlags;
        ClearCallFlags();
        if(handCalls != HandCall.None)
            m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.None);
    }

    public byte GetPlayerID()
    {
        return m_playerID;
    }

    //Private functions

    private void EnableHandCalls(Tile drawnTile)
    {
        EnableKan(drawnTile, false);
        EnableLateKan(drawnTile);
        EnableTsumo();

        if(m_callFlags != HandCall.None)
            m_eventCallbacks.OnHandCallAvailable(m_callFlags, m_futureMelds.chi);
    }

    private void EnableChi(Tile input)
    {
        Dictionary<Tile.Suit, List<Tile>> groupedSuits = TileHelpers.ArrangeTilesBySuit(m_hand.tiles);

        if(!groupedSuits.ContainsKey(input.suit))
            return;
        if(groupedSuits[input.suit].Count < 3)
            return;

        Dictionary<Tile.Face, List<Tile>> groupedFaces = TileHelpers.ArrangeTilesByValue(groupedSuits[input.suit]);
        if(!groupedFaces.ContainsKey(input.face))
            groupedFaces[input.face] = new List<Tile>();
        groupedFaces[input.face].Add(input);

        int count = 1;
        for(int i = (int)input.face - 2; i <= (int)input.face + 2; i++, count++)
        {
            if(!groupedFaces.ContainsKey((Tile.Face)i))
                count = 0;
            else if(count >= 3)
            {
                Tile.Face lowest = (Tile.Face)i - 2;
                Tile.Face middle = (Tile.Face)i - 1;
                Tile.Face highest = (Tile.Face)i;
                Tile[] tiles = { groupedFaces[lowest][0], groupedFaces[middle][0], groupedFaces[highest][0] };

                m_futureMelds.chi.Add(new Meld(tiles, true));
            }
        }

        if(m_futureMelds.chi.Count > 0)
            m_callFlags |= HandCall.Chi;
    }
    private void EnablePon(Tile input)
    {
        Dictionary<Tile, List<Tile>> groupedTiles = TileHelpers.ArrangeTilesByTile(m_hand.tiles);

        if(!groupedTiles.ContainsKey(input))
            return;

        groupedTiles[input].Add(input);

        if(groupedTiles[input].Count < 3)
            return;

        Tile[] tiles = { input, groupedTiles[input][0], groupedTiles[input][1] };
        m_futureMelds.pon = (new Meld(tiles, true));

        m_callFlags |= HandCall.Pon;
    }
    private void EnableKan(Tile input, bool isOpen)
    {
        Dictionary<Tile, List<Tile>> groupedTiles = TileHelpers.ArrangeTilesByTile(m_hand.tiles);

        if(!groupedTiles.ContainsKey(input))
            return;

        groupedTiles[input].Add(input);

        if(groupedTiles[input].Count < 4)
            return;

        Tile[] tiles = { input, groupedTiles[input][0], groupedTiles[input][1] };
        m_futureMelds.kan = (new Meld(tiles, isOpen));

        m_callFlags |= HandCall.Kan;
    }
    private void EnableLateKan(Tile input)
    {
        if(m_hand.melds.Count == 0)
            return;

        foreach(Meld meld in m_hand.melds)
        {
            if(meld.type != Meld.Type.Triple)
                continue;

            if(meld.Contains(input))
            {
                List<Tile> tiles = new List<Tile>(meld.tiles);
                tiles.Add(input);
                m_futureMelds.kan = (new Meld(tiles, true));

                m_callFlags |= HandCall.LateKan;

                return;
            }
        }
    }
    private void EnableRon(Tile input)
    {
    }
    private void EnableTsumo()
    {

    }

    private void CreateMeld(Meld meld)
    {
        foreach(Tile tile in meld.tiles)
            m_hand.tiles.Remove(tile);

        m_hand.melds.Add(meld);
        ClearCallFlags();
        m_hasDiscardedTile = false;

        m_eventCallbacks.OnMeldCreated(meld);
    }
    private void ClearCallFlags()
    {
        m_callFlags = HandCall.None;
        m_futureMelds.Clear();
    }

    //Constructor

    public Player(IGameCallbacks gameCallbacks, IPlayerEventCallbacks eventCallbacks, byte playerID)
    {
        m_gameCallbacks = gameCallbacks;
        m_eventCallbacks = eventCallbacks;

        m_futureMelds.chi = new List<Meld>();
        m_playerID = playerID;
        m_eventCallbacks.OnPlayerCreated(this);
    }

    //Custom type

    private struct CachedMelds
    {
        public List<Meld> chi;
        public Meld pon;
        public Meld kan;

        public void Clear()
        {
            chi.Clear();
        }
    }
    //public PlayerData m_playerData { get; private set; }
    //public IGameLogicCallbacks gameCallbacks;

    //Dictionary<HandCall, List<Meld>> m_cachedMelds;

    //public int points { get => m_playerData.points; set => m_playerData.points = value; }
    //bool m_hasDiscardTile = false;

    //private void Awake()
    //{
    //    m_playerData = new PlayerData();
    //    m_cachedMelds = new Dictionary<HandCall, List<Meld>>();
    //    m_cachedMelds[HandCall.Chi] = new List<Meld>();
    //    m_cachedMelds[HandCall.Pon] = new List<Meld>();
    //    m_cachedMelds[HandCall.Kan] = new List<Meld>();
    //}

    //public void AddTileToHand(Tile tile)
    //{
    //    m_playerData.hand.tiles.Add(tile);
    //    m_hasDiscardTile = false;

    //    EnableHandCalls(tile, false, true);
    //}
    //public bool RemoveTileFromHand(Tile tile)
    //{
    //    if(m_hasDiscardTile)
    //        return false;

    //    if(!IsCurrentPlayer())
    //        return false;

    //    if(!m_playerData.hand.tiles.Remove(tile))
    //        return false;

    //    m_hasDiscardTile = true;

    //    ClearCachedMelds();
    //    RefreshHand();

    //    gameCallbacks.OnTileRemoved(this, tile);


    //    return true;
    //}

    //public bool EnableHandCalls(Tile tile, bool enableChi, bool isMyTurn)
    //{
    //    if(isMyTurn)
    //    {
    //        EnableKan(tile, false);
    //    }
    //    else
    //    {
    //        if(enableChi)
    //        {
    //            EnableChi(tile);
    //        }
    //        EnablePon(tile);
    //        EnableKan(tile, true);
    //    }

    //    return m_playerData.handCalls != HandCall.None;
    //}

    //public void ResetHand(List<Tile> hand)
    //{
    //    m_playerData.hand.tiles.Clear();
    //    m_playerData.hand.tiles.AddRange(hand);

    //    RefreshHand();
    //}

    //private void RefreshHand()
    //{
    //    Dictionary<Tile.Suit, int> handArrangement = new Dictionary<Tile.Suit, int>();
    //    handArrangement[Tile.Suit.Bamboo] = 2;
    //    handArrangement[Tile.Suit.Character] = 0;
    //    handArrangement[Tile.Suit.Circle] = 1;
    //    handArrangement[Tile.Suit.Honor] = 3;
    //    //m_playerData.hand.SortHand(handArrangement);
    //}

    //bool EnableChi(Tile input)
    //{
    //    if(m_playerData.CanCallChi(input, out Meld meld))
    //    {
    //        m_cachedMelds[HandCall.Chi].Add(meld);
    //        Debug.Log("Chi Available");
    //        return true;
    //    }
    //    return false;
    //}

    //bool EnablePon(Tile input)
    //{
    //    if(m_playerData.CanCallPon(input, out Meld meld))
    //    {
    //        m_cachedMelds[HandCall.Pon].Add(meld);
    //        Debug.Log("Pon Available");
    //        return true;
    //    }
    //    return false;
    //}

    //bool EnableKan(Tile input, bool open)
    //{
    //    if(open)
    //    {
    //        if(m_playerData.CanCallOpenKan(input, out Meld meld))
    //        {
    //            m_cachedMelds[HandCall.Kan].Add(meld);
    //            Debug.Log("Kan available");
    //            return true;
    //        }
    //        return false;
    //    }
    //    else
    //    {
    //        if(m_playerData.CanCallClosedKan(out Meld meld))
    //        {
    //            m_cachedMelds[HandCall.Kan].Add(meld);
    //            Debug.Log("Kan available");
    //            return true;
    //        }
    //        else if(m_playerData.CanCallLateKan(out meld))
    //        {
    //            m_cachedMelds[HandCall.Kan].Add(meld);
    //            Debug.Log("Kan available");
    //            return true;
    //        }
    //        return false;
    //    }
    //}

    //public void CallChi(int index)
    //{
    //    if((m_playerData.handCalls & HandCall.Chi) != HandCall.Chi)
    //        return;
    //    AddMeld(m_cachedMelds[HandCall.Chi][index]);
    //    OnHandCallMade(m_cachedMelds[HandCall.Chi][index]);
    //    ClearCachedMelds();
    //}

    //public void CallPon()
    //{
    //    if((m_playerData.handCalls & HandCall.Pon) != HandCall.Pon)
    //        return;
    //    AddMeld(m_cachedMelds[HandCall.Pon][0]);
    //    OnHandCallMade(m_cachedMelds[HandCall.Pon][0]);
    //    ClearCachedMelds();
    //}

    //public void CallKan()
    //{
    //    if((m_playerData.handCalls & HandCall.Kan) != HandCall.Kan)
    //        return;
    //    if((m_playerData.handCalls & HandCall.LateKan) == HandCall.LateKan)
    //    {
    //        Meld cachedMeld = m_cachedMelds[HandCall.Kan][0];
    //        List<Tile> newMeldTiles = new List<Tile>(cachedMeld.tiles);
    //        newMeldTiles.Add(m_playerData.hand.tiles[m_playerData.hand.tiles.Count - 1]);
    //        m_playerData.hand.melds.Remove(cachedMeld);
    //        m_playerData.hand.melds.Add(new Meld(newMeldTiles, true));
    //    }
    //    else
    //    {
    //        AddMeld(m_cachedMelds[HandCall.Kan][0]);
    //    }
    //    OnHandCallMade(m_cachedMelds[HandCall.Kan][0]);
    //    ClearCachedMelds();
    //}

    //public void SkipHandCall()
    //{
    //    if(gameCallbacks.IsCurrentPlayer(this))
    //        return;
    //    if(m_playerData.handCalls == HandCall.None)
    //        return;
    //    ClearCachedMelds();
    //    gameCallbacks.SkipHandCall();
    //}

    //public bool IsCurrentPlayer()
    //{
    //    return gameCallbacks.IsCurrentPlayer(this);
    //}

    //private void AddMeld(Meld meld)
    //{
    //    foreach(Tile tile in meld.tiles)
    //    {
    //        m_playerData.hand.tiles.Remove(tile);
    //    }

    //    m_playerData.hand.melds.Add(meld);
    //    m_hasDiscardTile = false;
    //}

    //private void ClearCachedMelds()
    //{
    //    m_cachedMelds[HandCall.Chi].Clear();
    //    m_cachedMelds[HandCall.Pon].Clear();
    //    m_cachedMelds[HandCall.Kan].Clear();
    //    m_playerData.handCalls = HandCall.None;
    //}

    //private void OnHandCallMade(Meld meld)
    //{
    //    gameCallbacks.OnHandCallMade(this, meld);
    //}

    //private void OnWinningCallMade(HandCall handCall)
    //{
    //    gameCallbacks.OnWinningCallMade(this, handCall);
    //}
}


public interface IGameCallbacks
{
    void OnTileDiscarded(Tile tile);
    void OnHandCallMade(byte playerID, HandCall handCall);
    bool IsCurrentTurn(byte playerID);
}

public interface IPlayerEventCallbacks
{
    void OnTileAdded(Tile tile);
    void OnHandCallAvailable(HandCall callFlags, List<Meld> cachedChi);
    void OnMeldCreated(Meld meld);
    void OnHandReset(Hand hand);

    void OnPlayerCreated(IPlayerActions player);
}

public interface IPlayerActions
{
    void RemoveTile(Tile tile);

    void CallChi(int index);
    void CallPon();
    void CallKan();
    void CallRon();
    void CallTsumo();
    void SkipCalls();

    byte GetPlayerID();
}
