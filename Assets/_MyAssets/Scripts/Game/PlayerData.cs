using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum HandCall
{
    None = 0,
    Chi = 1,
    Pon = 2,
    Kan = 4,
    Ron = 8,
    Tsumo = 16,
    LateKan = 32
}

public class PlayerData
{
    public Hand hand;
    public int points;
    public HandCall handCalls;

    public PlayerData()
    {
        hand = new Hand();
        points = 0;
        handCalls = HandCall.None;
    }

    public bool CanCallLateKan(out Meld outMeld)
    {
        outMeld = null;
        Tile drawnTile = hand.tiles[hand.tiles.Count - 1];
        foreach(var meld in hand.melds)
        {
            if(meld.type != Meld.Type.Triple)
                continue;

            if(meld.Contains(drawnTile))
            {
                outMeld = meld;
                handCalls |= HandCall.LateKan;
                return true;
            }
        }

        return false;
    }

    public bool CanCallClosedKan(out Meld meld)
    {
        meld = null;
        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(hand.tiles);
        Tile drawnTile = hand.tiles[hand.tiles.Count - 1];

        if(tileMap[drawnTile].Count == 4)
        {
            meld = new Meld(tileMap[drawnTile], true);
            handCalls |= HandCall.Kan;
            return true;
        }
        return false;
    }

    public bool CanCallOpenKan(Tile input, out Meld meld)
    {
        meld = null;
        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(hand.tiles);
        if(!tileMap.ContainsKey(input))
            return false;
        tileMap[input].Add(input);
        if(tileMap[input].Count == 4)
        {
            meld = new Meld(tileMap[input], true);
            handCalls |= HandCall.Kan;
            return true;
        }

        return false;
    }

    public bool CanCallPon(Tile input, out Meld meld)
    {
        meld = null;
        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(hand.tiles);
        if(!tileMap.ContainsKey(input))
            return false;
        tileMap[input].Add(input);
        if(tileMap[input].Count == 3)
        {

            meld = new Meld(tileMap[input], true);
            handCalls |= HandCall.Pon;
            return true;
        }

        return false;
    }

    public bool CanCallChi(Tile input, out Meld meld)
    {
        meld = null;
        if(TileHelpers.IsHonor(input))
            return false;

        Dictionary<int, List<Tile>> tileValue = null;

        {
            List<Tile> suit = TileHelpers.GetTilesOfSuit(hand.tiles, input.suit);

            if(suit.Count < 2)
                return false;

            tileValue = TileHelpers.ArrangeTilesByValue(suit);

            if(!tileValue.ContainsKey(input.rawValue))
                tileValue[input.rawValue] = new List<Tile>();

            tileValue[input.rawValue].Add(input);
        }

        Tile[] sequence = TileHelpers.CreateSequenceLowestValue(tileValue, input);
        if(sequence == null)
            return false;

        meld = new Meld(sequence, true);
        handCalls |= HandCall.Chi;

        return true;
    }
}
