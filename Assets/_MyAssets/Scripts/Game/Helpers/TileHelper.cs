using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.32
public static class TileHelper
{
    public static Dictionary<Tile.Face, List<Tile>> ArrangeTilesByFace(List<Tile> tiles)
    {
        List<Tile> copiedTiles = new List<Tile>(tiles);
        Dictionary<Tile.Face, List<Tile>> arrangedTiles = new Dictionary<Tile.Face, List<Tile>>();

        for(int i = copiedTiles.Count - 1; i >= 0; i--)
        {
            Tile.Face currentFace = copiedTiles[i].face;

            if(!arrangedTiles.ContainsKey(currentFace))
                arrangedTiles[currentFace] = new List<Tile>();

            arrangedTiles[currentFace].Add(copiedTiles[i]);
            copiedTiles.RemoveAt(i);
        }

        return arrangedTiles;
    }
    public static Dictionary<Tile.Suit, List<Tile>> ArrangeTilesBySuit(List<Tile> tiles)
    {
        List<Tile> copiedTiles = new List<Tile>(tiles);
        Dictionary<Tile.Suit, List<Tile>> arrangedTiles = new Dictionary<Tile.Suit, List<Tile>>();

        for(int i = copiedTiles.Count - 1; i >= 0; i--)
        {
            Tile.Suit currentSuit = copiedTiles[i].suit;

            if(!arrangedTiles.ContainsKey(currentSuit))
                arrangedTiles[currentSuit] = new List<Tile>();

            arrangedTiles[currentSuit].Add(copiedTiles[i]);
            copiedTiles.RemoveAt(i);
        }

        return arrangedTiles;
    }

    public static List<Tile> GetAllTilesOfFace(Tile.Face number, List<Tile> tiles)
    {
        return tiles.FindAll((Tile compare) => { return compare.face == number; });
    }
    public static List<Tile> GetAllTilesOfSuit(Tile.Suit suit, List<Tile> tiles)
    {
        return tiles.FindAll((Tile compare) => { return compare.suit == suit; });
    }
    public static List<Tile> GetAllTilesOfType(Tile type, List<Tile> tiles)
    {
        return tiles.FindAll((Tile compare) => { return compare == type; });
    }

    public static bool IsWind(Tile tile)
    {
        switch(tile.face)
        {
        case Tile.Face.East:
        case Tile.Face.South:
        case Tile.Face.West:
        case Tile.Face.North:
            return IsHonor(tile);
        default:
            return false;
        }
    }
    public static bool IsDragon(Tile tile)
    {
        switch(tile.face)
        {
        case Tile.Face.Green:
        case Tile.Face.Red:
        case Tile.Face.White:
            return IsHonor(tile);
        default:
            return false;
        }
    }
    public static bool IsSimple(Tile tile)
    {
        return !IsTerminal(tile);
    }
    public static bool IsTerminal(Tile tile)
    {
        return !IsHonor(tile) && (tile.face == Tile.Face.One || tile.face == Tile.Face.Nine);
    }
    public static bool IsHonor(Tile tile)
    {
        return tile.suit == Tile.Suit.Honor;
    }
}
