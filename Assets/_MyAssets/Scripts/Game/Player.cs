using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.32

public class Player : MonoBehaviour
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
    public int points;
    public HandCall handCalls;
    public PotentialMelds potentialMelds;
    private List<Tile> m_waitingTiles = new List<Tile>();

    void Awake()
    {
        m_hand.Init();
        potentialMelds.Init();
    }

    public void AddTile(Tile tile)
    {
        m_hand.tiles.Add(tile);
        EnableHandCalls(tile);
    }

    public void RemoveTile(Tile tile)
    {
        m_hand.tiles.Remove(tile);

        m_waitingTiles = HandHelper.GetRemainingTilesInTenpai(m_hand, out bool inTenpai);
    }

    public void SkipHandCalls()
    {
        handCalls = HandCall.None;
        potentialMelds.Clear();
    }

    public void CallChi(int index)
    {
        CheckFlag(HandCall.Chi);

        AddMeld(potentialMelds.chiMelds[index]);

        SkipHandCalls();
    }

    public void CallPon()
    {
        CheckFlag(HandCall.Pon);
        AddMeld(potentialMelds.ponMeld);

        SkipHandCalls();
    }

    public void CallKan()
    {
        CheckFlag(HandCall.Kan);
        CheckFlag(HandCall.LateKan);

        if((handCalls & HandCall.LateKan) == HandCall.LateKan)
            m_hand.melds.RemoveAll((Meld compare) => { return compare.tiles[0] == potentialMelds.kanMeld.tiles[0]; });

        AddMeld(potentialMelds.kanMeld);

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

    public bool EnableHandCalls(DiscardedTile discardedTile, bool checkChi)
    {
        List<Tile> potentialHand = new List<Tile>(m_hand.tiles);
        potentialHand.Add(discardedTile.tile);
        if(checkChi)
            EnableChi(discardedTile, potentialHand);
        EnablePon(discardedTile, potentialHand);
        EnableOpenKan(discardedTile, potentialHand);
        EnableRon(discardedTile);

        return handCalls != HandCall.None;
    }

    public void ResetHand(List<Tile> tiles)
    {
        m_hand.tiles = tiles;
        m_hand.melds.Clear();
        m_waitingTiles = HandHelper.GetRemainingTilesInTenpai(m_hand, out bool inTenpai);
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

        if(MeldHelper.MakePossibleSequences(discardedTile, potentialHand, out potentialMelds.chiMelds))
            handCalls |= HandCall.Chi;
    }
    private void EnablePon(DiscardedTile discardedTile, List<Tile> potentialHand)
    {
        if(potentialHand.Count < 3)
            return;

        if(MeldHelper.MakeTriple(discardedTile, potentialHand, out potentialMelds.ponMeld))
            handCalls |= HandCall.Pon;
    }
    private void EnableOpenKan(DiscardedTile discardedTile, List<Tile> potentialHand)
    {
        if(potentialHand.Count < 4)
            return;

        if(MeldHelper.MakeQuad(discardedTile, potentialHand, out potentialMelds.kanMeld))
            handCalls |= HandCall.Kan;
    }
    private void EnableRon(DiscardedTile discardedTile)
    {
        if(m_waitingTiles.Contains(discardedTile.tile))
            handCalls |= HandCall.Ron;
    }
    private void EnableClosedKan(Tile drawnTile)
    {
        if(m_hand.tiles.Count < 4)
            return;

        if(MeldHelper.MakeQuad(drawnTile, m_hand.tiles, out potentialMelds.kanMeld))
        {
            handCalls |= HandCall.Kan;
        }
    }
    private void EnableLateKan(Tile drawnTile)
    {
        Meld meld = m_hand.melds.Find((Meld compare) => { return compare.tiles[0] == drawnTile; });

        if(MeldHelper.MakeQuad(meld, drawnTile, out potentialMelds.kanMeld))
            handCalls |= HandCall.LateKan;
    }
    private void EnableTsumo(Tile drawnTile)
    {
        if(m_waitingTiles.Contains(drawnTile))
            handCalls |= HandCall.Tsumo;
    }

    private void CheckFlag(HandCall value)
    {
        Debug.Assert((handCalls & value) == value, value.ToString() + " flag is not set");
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
