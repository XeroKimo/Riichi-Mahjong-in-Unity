using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeldHelpers
{
    public static Dictionary<Meld.Type, List<Meld>> ArrangeMeldsByType(List<Meld> melds)
    {
        Dictionary<Meld.Type, List<Meld>> output = new Dictionary<Meld.Type, List<Meld>>();

        foreach (var meld in melds)
        {
            Meld.Type key = meld.type;
            if (!output.ContainsKey(key))
                output[key] = new List<Meld>();
            output[key].Add(meld);
        }

        return output;
    }

    public static Dictionary<Tile.Suit, List<Meld>> ArrangeMeldsBySuit(List<Meld> melds)
    {
        Dictionary<Tile.Suit, List<Meld>> output = new Dictionary<Tile.Suit, List<Meld>>();

        foreach (var meld in melds)
        {
            Tile.Suit key = meld.tiles[0].suit;
            if (!output.ContainsKey(key))
                output[key] = new List<Meld>();
            output[key].Add(meld);
        }

        return output;
    }

    public static int CountOfType(List<Meld> melds, Meld.Type type)
    {
        int count = 0;
        foreach (var meld in melds)
        {
            if (meld.type == type)
                count++;
        }
        return count;
    }

    public static int CountSuits(List<Meld> melds)
    {
        HashSet<Meld.Type> count = new HashSet<Meld.Type>();
        foreach (var meld in melds)
        {
            if (count.Contains(meld.type))
                continue;

            count.Add(meld.type);
        }
        return count.Count;
    }

    public static int CountSuitsOfType(List<Meld> melds, Tile.Suit suit)
    {
        int count = 0;
        foreach (var meld in melds)
        {
            if (meld.tiles[0].suit == suit)
                count++;
        }
        return count;
    }

    public static List<Meld> GetMeldsByType(List<Meld> melds, Meld.Type type)
    {
        List<Meld> output = new List<Meld>(melds.Count);

        foreach (var meld in melds)
        {
            if (meld.type == type)
                output.Add(meld);
        }

        return output;
    }

    public static bool IsSimple(Meld meld)
    {
        foreach (Tile tile in meld.tiles)
        {
            if (!TileHelpers.IsSimple(tile))
                return false;
        }
        return true;
    }

    public static bool IsTerminal(Meld meld)
    {
        foreach (Tile tile in meld.tiles)
        {
            if (!TileHelpers.IsTerminal(tile))
                return false;
        }
        return true;
    }

    public static bool IsDragon(Meld meld)
    {
        return TileHelpers.IsDragon(meld.tiles[0]);
    }
    public static bool IsHonor(Meld meld)
    {
        return TileHelpers.IsHonor(meld.tiles[0]);
    }
    public static bool IsSeatWind(Meld meld, Tile.Face seatWind)
    {
        return TileHelpers.IsSeatWind(meld.tiles[0], seatWind);
    }
    public static bool IsPrevalentWind(Meld meld, Tile.Face prevalentWind)
    {
        return TileHelpers.IsPrevalentWind(meld.tiles[0], prevalentWind);
    }
}
