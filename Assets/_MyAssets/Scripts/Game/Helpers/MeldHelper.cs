using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//Version: 0.32

public static class MeldHelper
{
    public static Dictionary<Meld.Type, List<Meld>> ArrangeMeldsByType(List<Meld> melds)
    {
        Dictionary<Meld.Type, List<Meld>> arrangedMelds = new Dictionary<Meld.Type, List<Meld>>();
        arrangedMelds[Meld.Type.Pair] = new List<Meld>();
        arrangedMelds[Meld.Type.Triple] = new List<Meld>();
        arrangedMelds[Meld.Type.Quad] = new List<Meld>();
        arrangedMelds[Meld.Type.Sequence] = new List<Meld>();

        foreach(Meld meld in melds)
        {
            arrangedMelds[meld.type].Add(meld);
        }

        return arrangedMelds;
    }

    public static bool MakePair(Tile type, List<Tile> tiles, out Meld createdMeld)
    {
        List<Tile> matchedTiles = TileHelper.GetAllTilesOfType(type, tiles);
        if(matchedTiles.Count < 2)
        {
            createdMeld = Meld.emptyMeld;
            return false;
        }

        createdMeld = new Meld(matchedTiles.GetRange(0, 2).ToArray());
        return true;
    }

    public static bool MakeTriple(Tile type, List<Tile> tiles, out Meld createdMeld)
    {
        return MakeTriple(new DiscardedTile(type, DiscardedTile.noOwnerID), tiles, out createdMeld, false);
    }

    public static bool MakeTriple(DiscardedTile type, List<Tile> tiles, out Meld createdMeld)
    {
        return MakeTriple(type, tiles, out createdMeld, true);
    }    

    public static bool MakeQuad(Tile type, List<Tile> tiles, out Meld createdMeld)
    {
        return MakeQuad(new DiscardedTile(type, DiscardedTile.noOwnerID), tiles, out createdMeld, false);
    }

    public static bool MakeQuad(DiscardedTile type, List<Tile> tiles, out Meld createdMeld)
    {
        return MakeQuad(type, tiles, out createdMeld, true);
    }

    public static bool MakeQuad(Meld triplet, Tile tile, out Meld createdMeld)
    {
        if(triplet == Meld.emptyMeld ||
            triplet.type != Meld.Type.Triple ||
            triplet.tiles[0] != tile)
        {
            createdMeld = Meld.emptyMeld;
            return false;
        }

        List<Tile> tileArray = new List<Tile>(triplet.tiles);
        tileArray.Add(tile);

        createdMeld = new Meld(tileArray.ToArray(), triplet.discardedTile, triplet.isOpen);
        return true;
    }

    public static bool MakePossibleSequences(Tile pivotTile, List<Tile> tiles, out List<Meld> createdMelds)
    {
        Dictionary<Tile.Suit, List<Tile>> tilesBySuit = TileHelper.ArrangeTilesBySuit(tiles);

        if(!tilesBySuit.ContainsKey(pivotTile.suit) || tilesBySuit[pivotTile.suit].Count < 3)
        {
            createdMelds = new List<Meld>();
            return false;
        }

        return MakePossibleSequences(pivotTile, TileHelper.ArrangeTilesByFace(tilesBySuit[pivotTile.suit]), out createdMelds);
    }

    public static bool MakePossibleSequences(Tile pivotTile, Dictionary<Tile.Face, List<Tile>> arrangedTiles, out List<Meld> createdMelds)
    {
        if(arrangedTiles.Count < 3)
        {
            createdMelds = new List<Meld>();
            return false;
        }

        return MakePossibleSequences(new DiscardedTile(pivotTile, DiscardedTile.noOwnerID), arrangedTiles, out createdMelds, false);
    }

    public static bool MakePossibleSequences(DiscardedTile pivotTile, List<Tile> tiles, out List<Meld> createdMelds)
    {
        Dictionary<Tile.Suit, List<Tile>> tilesBySuit = TileHelper.ArrangeTilesBySuit(tiles);

        if(!tilesBySuit.ContainsKey(pivotTile.tile.suit) || tilesBySuit[pivotTile.tile.suit].Count < 3)
        {
            createdMelds = new List<Meld>();
            return false;
        }

        return MakePossibleSequences(pivotTile, TileHelper.ArrangeTilesByFace(tilesBySuit[pivotTile.tile.suit]), out createdMelds, true);
    }

    private static bool MakeTriple(DiscardedTile type, List<Tile> tiles, out Meld createdMeld, bool isOpen)
    {
        List<Tile> matchedTiles = TileHelper.GetAllTilesOfType(type.tile, tiles);
        if(matchedTiles.Count < 3)
        {
            createdMeld = Meld.emptyMeld;
            return false;
        }

        createdMeld = new Meld(matchedTiles.GetRange(0, 3).ToArray(), type, isOpen);
        return true;
    }

    private static bool MakeQuad(DiscardedTile type, List<Tile> tiles, out Meld createdMeld, bool isOpen)
    {
        List<Tile> matchedTiles = TileHelper.GetAllTilesOfType(type.tile, tiles);
        if(matchedTiles.Count < 4)
        {
            createdMeld = Meld.emptyMeld;
            return false;
        }

        createdMeld = new Meld(matchedTiles.ToArray(), type, isOpen);
        return true;
    }

    private static bool MakePossibleSequences(DiscardedTile pivotTile, Dictionary<Tile.Face, List<Tile>> arrangedTiles, out List<Meld> createdMelds, bool isOpen)
    {
        createdMelds = new List<Meld>();

        for(int i = (int)pivotTile.tile.face - 2; i <= (int)pivotTile.tile.face + 2; i++)
        {
            if(!arrangedTiles.ContainsKey((Tile.Face)i) &&
                !arrangedTiles.ContainsKey((Tile.Face)i + 1) &&
                !arrangedTiles.ContainsKey((Tile.Face)i + 2))
            {
                continue;
            }

            Tile[] tiles = new Tile[3];
            tiles[0] = arrangedTiles[(Tile.Face)i][0];
            tiles[1] = arrangedTiles[(Tile.Face)i + 1][0];
            tiles[2] = arrangedTiles[(Tile.Face)i + 2][0];

            createdMelds.Add(new Meld(tiles, pivotTile, isOpen));
        }

        return createdMelds.Count != 0;
    }
}
