using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.34

public class Player
{
    [Flags]
    public enum HandCall
    {
        None,
        Chi = 1,
        Pon = 2,
        Kan = 4,
        LateKan = 8,
        Ron = 16,
        Tsumo = 32
    }

    public struct PotentialMelds
    {
        public List<Meld> chiMelds;
        public Meld ponMeld;
        public Meld kanMeld;

        public void Init()
        {
            chiMelds = new List<Meld>();
        }

        public void Clear()
        {
            chiMelds.Clear();
            ponMeld = Meld.emptyMeld;
            kanMeld = Meld.emptyMeld;
        }
    }

    Hand m_hand;
    HandCall m_handCalls;
    //Cached melds for hand call purposes
    PotentialMelds m_potentialMelds;
    //List of possible tiles that will complete the hand
    List<Tile> m_waitingTiles;

    public int points;
    public Hand hand { get => m_hand; } 
    public HandCall handCalls { get => m_handCalls; }
    public PotentialMelds potentialMelds { get => m_potentialMelds; }
    public List<Tile> waitingTiles { get => m_waitingTiles; }

    public Player()
    {
        m_hand.Init();
        m_potentialMelds.Init();
        m_waitingTiles = new List<Tile>();
    }

    public void AddTile(Tile tile)
    {
        m_hand.tiles.Add(tile);
        EnableHandCalls(tile);
    }

    public void RemoveTile(Tile tile)
    {
        m_hand.tiles.Remove(tile);

        m_waitingTiles = HandHelper.FindWaitingTiles(m_hand);
    }

    public void SkipHandCalls()
    {
        m_handCalls = HandCall.None;
        m_potentialMelds.Clear();
    }

    public void CallChi(int index)
    {
        CheckFlag(HandCall.Chi);

        AddMeld(m_potentialMelds.chiMelds[index]);

        SkipHandCalls();
    }

    public void CallPon()
    {
        CheckFlag(HandCall.Pon);
        AddMeld(m_potentialMelds.ponMeld);

        SkipHandCalls();
    }

    public void CallKan()
    {
        CheckFlag(HandCall.Kan);
        CheckFlag(HandCall.LateKan);

        if((m_handCalls & HandCall.LateKan) == HandCall.LateKan)
            m_hand.melds.RemoveAll((Meld compare) => { return compare.tiles[0] == m_potentialMelds.kanMeld.tiles[0]; });

        AddMeld(m_potentialMelds.kanMeld);

        SkipHandCalls();
    }

    public void CallRon()
    {
        CheckFlag(HandCall.Ron);

        SkipHandCalls();
    }

    public void CallTsumo()
    {
        CheckFlag(HandCall.Tsumo);

        SkipHandCalls();
    }

    public void ResetHand(List<Tile> tiles)
    {
        m_hand.tiles = tiles;
        m_hand.melds.Clear();
        m_waitingTiles = HandHelper.FindWaitingTiles(m_hand);
    }

    public bool EnableHandCalls(DiscardedTile discardedTile, bool checkChi)
    {
        List<Tile> potentialHand = new List<Tile>(m_hand.tiles);
        potentialHand.Add(discardedTile.tile);
        if(checkChi)
            EnableChi(discardedTile, potentialHand);
        EnablePon(discardedTile, potentialHand);
        EnableOpenKan(discardedTile, potentialHand);
        EnableRon(discardedTile);

        return m_handCalls != HandCall.None;
    }

    private void EnableHandCalls(Tile drawnTile)
    {
        EnableClosedKan(drawnTile);
        EnableLateKan(drawnTile);
        EnableTsumo(drawnTile);
    }

    private void EnableChi(DiscardedTile discardedTile, List<Tile> potentialHand)
    {
        if(potentialHand.Count < 3)
            return;

        if(MeldHelper.MakePossibleSequences(discardedTile, potentialHand, out m_potentialMelds.chiMelds))
            m_handCalls |= HandCall.Chi;
    }
    private void EnablePon(DiscardedTile discardedTile, List<Tile> potentialHand)
    {
        if(potentialHand.Count < 3)
            return;

        if(MeldHelper.MakeTriple(discardedTile, potentialHand, out m_potentialMelds.ponMeld))
            m_handCalls |= HandCall.Pon;
    }
    private void EnableOpenKan(DiscardedTile discardedTile, List<Tile> potentialHand)
    {
        if(potentialHand.Count < 4)
            return;

        if(MeldHelper.MakeQuad(discardedTile, potentialHand, out m_potentialMelds.kanMeld))
            m_handCalls |= HandCall.Kan;
    }
    private void EnableRon(DiscardedTile discardedTile)
    {
        if(m_waitingTiles.Contains(discardedTile.tile))
            m_handCalls |= HandCall.Ron;
    }
    private void EnableClosedKan(Tile drawnTile)
    {
        if(m_hand.tiles.Count < 4)
            return;

        if(MeldHelper.MakeQuad(drawnTile, m_hand.tiles, out m_potentialMelds.kanMeld))
        {
            m_handCalls |= HandCall.Kan;
        }
    }
    private void EnableLateKan(Tile drawnTile)
    {
        Meld meld = m_hand.melds.Find((Meld compare) => { return compare.tiles[0] == drawnTile; });

        if(MeldHelper.MakeQuad(meld, drawnTile, out m_potentialMelds.kanMeld))
            m_handCalls |= HandCall.LateKan;
    }
    private void EnableTsumo(Tile drawnTile)
    {
        if(m_waitingTiles.Contains(drawnTile))
            m_handCalls |= HandCall.Tsumo;
    }

    private void CheckFlag(HandCall value)
    {
        Debug.Assert((m_handCalls & value) == value, value.ToString() + " flag is not set");
    }

    private void AddMeld(Meld meld)
    {
        m_hand.melds.Add(meld);

        foreach(Tile tile in meld.tiles)
        {
            m_hand.tiles.Remove(tile);
        }
    }
}
