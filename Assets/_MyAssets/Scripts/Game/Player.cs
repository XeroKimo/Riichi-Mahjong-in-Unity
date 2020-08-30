using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.4

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

public class Player
{
    public int points;
    public Hand hand; 
    public HandCall handCalls;

    //Cached melds for hand call purposes
    public PotentialMelds potentialMelds;

    //List of possible tiles that will complete the hand
    public List<Tile> waitingTiles; 

    public Player()
    {
        hand.Init();
        potentialMelds.Init();
        waitingTiles = new List<Tile>();
    }


    public void ClearHandCalls()
    {
        handCalls = HandCall.None;
        potentialMelds.Clear();
    }


    public void ResetHand(List<Tile> tiles)
    {
        hand.tiles = tiles;
        hand.melds.Clear();
        waitingTiles = HandHelper.FindWaitingTiles(hand);
    }


    public void EnableChi(DiscardedTile discardedTile)
    {
        List<Tile> potentialHand = hand.tiles;
        potentialHand.Add(discardedTile.tile);

        if(potentialHand.Count < 3)
            return;

        if(MeldHelper.MakePossibleSequences(discardedTile, potentialHand, out potentialMelds.chiMelds))
            handCalls |= HandCall.Chi;

        potentialHand.RemoveAt(potentialHand.Count - 1);
    }
    public void EnablePon(DiscardedTile discardedTile)
    {
        List<Tile> potentialHand = hand.tiles;
        potentialHand.Add(discardedTile.tile);

        if(potentialHand.Count < 3)
            return;

        if(MeldHelper.MakeTriple(discardedTile, potentialHand, out potentialMelds.ponMeld))
            handCalls |= HandCall.Pon;

        potentialHand.RemoveAt(potentialHand.Count - 1);
    }
    public void EnableOpenKan(DiscardedTile discardedTile)
    {
        List<Tile> potentialHand = hand.tiles;
        potentialHand.Add(discardedTile.tile);

        if(potentialHand.Count < 4)
            return;

        if(MeldHelper.MakeQuad(discardedTile, potentialHand, out potentialMelds.kanMeld))
            handCalls |= HandCall.Kan;

        potentialHand.RemoveAt(potentialHand.Count - 1);
    }
    public void EnableRon(DiscardedTile discardedTile)
    {
        if(waitingTiles.Contains(discardedTile.tile))
            handCalls |= HandCall.Ron;
    }
    public void EnableClosedKan(Tile drawnTile)
    {
        if(hand.tiles.Count < 4)
            return;

        if(MeldHelper.MakeQuad(drawnTile, hand.tiles, out potentialMelds.kanMeld))
        {
            handCalls |= HandCall.Kan;
        }
    }
    public void EnableLateKan(Tile drawnTile)
    {
        Meld meld = hand.melds.Find((Meld compare) => { return compare.tiles[0] == drawnTile; });

        if(MeldHelper.MakeQuad(meld, drawnTile, out potentialMelds.kanMeld))
            handCalls |= HandCall.LateKan;
    }
    public void EnableTsumo(Tile drawnTile)
    {
        if(waitingTiles.Contains(drawnTile))
            handCalls |= HandCall.Tsumo;
    }


    public void AddMeld(Meld meld)
    {
        hand.melds.Add(meld);

        foreach(Tile tile in meld.tiles)
        {
            hand.tiles.Remove(tile);
        }
    }
}
