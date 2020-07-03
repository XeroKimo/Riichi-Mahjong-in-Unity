using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Hand
{
    public List<Tile> tiles;
    public List<Meld> melds;

    //public Hand()
    //{
    //    tiles = new List<Tile>(14);
    //    melds = new List<Meld>(5);
    //}

    //public Hand(Hand other)
    //{
    //    tiles = new List<Tile>(other.tiles);
    //    melds = new List<Meld>(other.melds);
    //}

    //public bool IsOpenHand()
    //{
    //    foreach(var meld in melds)
    //    {
    //        if(meld.open)
    //            return true;
    //    }
    //    return false;
    //}

    //public void SwapTiles(int first, int second)
    //{
    //    Tile swappedTile = tiles[first];
    //    tiles[first] = tiles[second];
    //    tiles[second] = swappedTile;
    //}

    //public void SortHand(Dictionary<Tile.Suit, int> sortArrangement)
    //{
    //    //tiles = TileHelpers.SortTiles(tiles, sortArrangement);
    //}
}
