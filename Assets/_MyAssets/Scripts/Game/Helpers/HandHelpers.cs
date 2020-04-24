using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandState
{
    Searching,
    Waiting,
    Complete
}

public static class HandHelpers
{
    static public HandState CheckHandState(Hand hand, out List<Tile> waitingTiles)
    {
        List<Tile> tiles = new List<Tile>(hand.GetTiles());

        waitingTiles = TileHelpers.GetWaitingTiles(tiles);
        if (waitingTiles.Count == 0)
        {
            waitingTiles = CheckSpecialHands(tiles);
        }
        if (waitingTiles.Count > 0)
        {
            return HandState.Waiting;
        }
        return HandState.Searching;
    }

    static List<Tile> CheckSpecialHands(List<Tile> tiles)
    {
        List<Tile> waitingTiles = CheckSevenPairs(tiles);
        if (waitingTiles.Count == 0)
            waitingTiles = CheckThirteenOrphans(tiles);

        return waitingTiles;
    }

    static List<Tile> CheckSevenPairs(List<Tile> tiles)
    {
        if (tiles.Count < 13)
            return new List<Tile>();

        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(tiles);
        if (tileMap.Count < 7)
            return new List<Tile>();

        List<Tile> waitingTiles = new List<Tile>();
        foreach (var tile in tileMap.Values)
        {
            if (tile.Count > 2)
                return new List<Tile>();
            if (tile.Count == 1)
                waitingTiles.Add(new Tile(tile[0]));
        }

        return waitingTiles;
    }

    static List<Tile> CheckThirteenOrphans(List<Tile> tiles)
    {
        if (tiles.Count < 13)
            return new List<Tile>();

        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(tiles);
        if (tileMap.Count < 12)
            return new List<Tile>();

        foreach (var tile in tileMap.Values)
        {
            if (TileHelpers.IsSimple(tile[0]))
                return new List<Tile>();
        }

        return FindMissingOrphan(tileMap);
    }

    static List<Tile> FindMissingOrphan(Dictionary<Tile, List<Tile>> tileMap)
    {
        List<Tile> orphans = GetOrphans();
        List<Tile> waitingTiles = new List<Tile>();
        foreach (var tile in orphans)
        {
            if (!tileMap.ContainsKey(tile))
            {
                waitingTiles.Add(tile);
                break;
            }
            else
            {
                if (tileMap[tile].Count < 2)
                    waitingTiles.Add(tile);
            }
        }

        return waitingTiles;
    }

    static List<Tile> GetOrphans()
    {
        List<Tile> tile = new List<Tile>();
        for (int i = Tile.honorMin; i < Tile.honorMax; i++)
        {
            tile.Add(new Tile(Tile.Suit.Honor, (Tile.Face)i));
        }
        for (int i = 1; i < (int)Tile.Suit.Count; i++)
        {
            tile.Add(new Tile((Tile.Suit)i, Tile.Face.One));
            tile.Add(new Tile((Tile.Suit)i, Tile.Face.Nine));
        }
        return tile;
    }

    static public bool CanDeclareRiichi(Hand hand, out List<Tile> waitingTiles)
    {
        List<Tile> tiles = new List<Tile>(hand.GetTiles());
        waitingTiles = null;
        return false;
    }
}
