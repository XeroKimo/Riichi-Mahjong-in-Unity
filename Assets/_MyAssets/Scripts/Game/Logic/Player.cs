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
    private CachedMelds m_cachedMelds;

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
    public bool EnableHandCalls(Tile lastDiscardedTile, byte currentPlayerID)
    {
        bool enableChi = (m_playerID == 0) ? currentPlayerID == 3 : currentPlayerID == m_playerID - 1;

        if(enableChi)
            EnableChi(lastDiscardedTile, currentPlayerID);
        EnablePon(lastDiscardedTile, currentPlayerID);
        EnableKan(lastDiscardedTile, true, currentPlayerID);
        EnableRon(lastDiscardedTile);

        bool hasFlagsSet = m_callFlags != HandCall.None;
        if(hasFlagsSet)
            m_eventCallbacks.OnHandCallAvailable(m_callFlags, m_cachedMelds.chi);
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
        m_eventCallbacks.OnTileRemoved(tile);
        m_gameCallbacks.OnTileDiscarded(tile);
    }

    public void CallChi(int index)
    {
        if((m_callFlags & HandCall.Chi) != HandCall.Chi)
            return;

        Meld meld = m_cachedMelds.chi[index];

        CreateMeld(meld);
        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Chi);
    }
    public void CallPon()
    {
        if((m_callFlags & HandCall.Pon) != HandCall.Pon)
            return;

        Meld meld = m_cachedMelds.pon;

        CreateMeld(meld);
        m_gameCallbacks.OnHandCallMade(m_playerID, HandCall.Pon);
    }
    public void CallKan()
    {
        if((m_callFlags & HandCall.Kan) != HandCall.Kan)
            return;

        Meld meld = m_cachedMelds.kan;

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
        EnableKan(drawnTile, false, m_playerID);
        EnableLateKan(drawnTile);
        EnableTsumo();

        if(m_callFlags != HandCall.None)
            m_eventCallbacks.OnHandCallAvailable(m_callFlags, m_cachedMelds.chi);
    }

    private void EnableChi(Tile input, byte inputPlayerID)
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

                m_cachedMelds.chi.Add(new Meld(tiles, true, new Meld.InputTile(inputPlayerID, input)));
            }
        }

        if(m_cachedMelds.chi.Count > 0)
            m_callFlags |= HandCall.Chi;
    }
    private void EnablePon(Tile input, byte inputPlayerID)
    {
        Dictionary<Tile, List<Tile>> groupedTiles = TileHelpers.ArrangeTilesByTile(m_hand.tiles);

        if(!groupedTiles.ContainsKey(input))
            return;

        groupedTiles[input].Add(input);

        if(groupedTiles[input].Count < 3)
            return;

        Tile[] tiles = { input, groupedTiles[input][0], groupedTiles[input][1] };
        m_cachedMelds.pon = (new Meld(tiles, true, new Meld.InputTile(inputPlayerID, input)));

        m_callFlags |= HandCall.Pon;
    }
    private void EnableKan(Tile input, bool isOpen, byte inputPlayerID)
    {
        Dictionary<Tile, List<Tile>> groupedTiles = TileHelpers.ArrangeTilesByTile(m_hand.tiles);

        if(!groupedTiles.ContainsKey(input))
            return;

        groupedTiles[input].Add(input);

        if(groupedTiles[input].Count < 4)
            return;

        Tile[] tiles = { input, groupedTiles[input][0], groupedTiles[input][1] };
        m_cachedMelds.kan = (new Meld(tiles, isOpen, new Meld.InputTile(inputPlayerID, input)));

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
                m_cachedMelds.kan = (new Meld(tiles, true, new Meld.InputTile(meld.input.playerID, input)));

                m_callFlags |= HandCall.LateKan;

                return;
            }
        }
    }
    private void EnableRon(Tile input)
    {
        //TODO: fill out enable ron
    }
    private void EnableTsumo()
    {
        //TODO: fill out enable tsumo
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
        m_cachedMelds.Clear();
    }

    //Constructor

    public Player(IGameCallbacks gameCallbacks, IPlayerEventCallbacks eventCallbacks, byte playerID)
    {
        m_gameCallbacks = gameCallbacks;
        m_eventCallbacks = eventCallbacks;

        m_cachedMelds.chi = new List<Meld>();
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
    void OnTileRemoved(Tile tile);
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
