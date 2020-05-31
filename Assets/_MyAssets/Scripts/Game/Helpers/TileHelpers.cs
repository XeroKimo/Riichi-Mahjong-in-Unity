using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHelpers
{
    public static Dictionary<Tile.Suit, List<Tile>> ArrangeTilesBySuit(List<Tile> hand)
    {
        Dictionary<Tile.Suit, List<Tile>> output = new Dictionary<Tile.Suit, List<Tile>>();
        for(int i = 0; i < hand.Count; i++)
        {
            Tile.Suit suit = hand[i].suit;
            if(!output.ContainsKey(suit))
                output[suit] = new List<Tile>();
            output[suit].Add(hand[i]);
        }

        return output;
    }

    public static Dictionary<Tile, List<Tile>> ArrangeTilesByTile(List<Tile> hand)
    {
        Dictionary<Tile, List<Tile>> output = new Dictionary<Tile, List<Tile>>();
        foreach(var tile in hand)
        {
            if(!output.ContainsKey(tile))
                output[tile] = new List<Tile>();
            output[tile].Add(tile);
        }

        return output;
    }

    public static Dictionary<Tile.Face, List<Tile>> ArrangeTilesByValue(List<Tile> hand)
    {
        Dictionary<Tile.Face, List<Tile>> output = new Dictionary<Tile.Face, List<Tile>>();
        foreach(var tile in hand)
        {
            if(!output.ContainsKey(tile.face))
                output[tile.face] = new List<Tile>();
            output[tile.face].Add(tile);
        }

        return output;
    }

    public static List<Tile> SortTiles(List<Tile> tiles)
    {
        Dictionary<Tile.Suit, int> suitOrder = new Dictionary<Tile.Suit, int>();
        suitOrder[Tile.Suit.Honor] = 4;
        suitOrder[Tile.Suit.Character] = 1;
        suitOrder[Tile.Suit.Circle] = 2;
        suitOrder[Tile.Suit.Bamboo] = 3;

        return SortTiles(tiles, suitOrder);
    }

    public static List<Tile> SortTiles(List<Tile> tiles, Dictionary<Tile.Suit, int> suitOrder)
    {
        List<Tile> sortedTiles = new List<Tile>(tiles.Count);
        List<Tile.Suit> order = DecodeSuitOrder(suitOrder);

        Dictionary<Tile.Suit, List<Tile>> sortedSuits = TileHelpers.ArrangeTilesBySuit(tiles);

        for(int i = 0; i < (int)Tile.suitCount; i++)
        {
            if(sortedSuits.ContainsKey((Tile.Suit)i))
                sortedSuits[(Tile.Suit)i] = TileHelpers.SortSequence((sortedSuits[(Tile.Suit)i]));
        }

        for(int i = 0; i < order.Count; i++)
        {
            if(sortedSuits.ContainsKey(order[i]))
                sortedTiles.AddRange(sortedSuits[order[i]]);
        }

        return sortedTiles;
    }

    static List<Tile.Suit> DecodeSuitOrder(Dictionary<Tile.Suit, int> suitOrder)
    {
        List<int> order = new List<int>(suitOrder.Values);
        Dictionary<int, Tile.Suit> flippedDictionary = new Dictionary<int, Tile.Suit>(4);

        foreach(var pair in suitOrder)
        {
            flippedDictionary[pair.Value] = pair.Key;
        }

        order.Sort();

        List<Tile.Suit> suits = new List<Tile.Suit>(4);
        foreach(var value in order)
        {
            suits.Add(flippedDictionary[value]);
        }

        return suits;
    }

    public static List<Tile> SortSequence(List<Tile> sequence)
    {
        List<Tile> sortedSequnce = new List<Tile>(sequence);
        sortedSequnce.Sort((Tile lh, Tile rh) => { return lh.face.CompareTo(rh.face); });
        return sortedSequnce;
    }

    public static List<Tile> GetTilesOfSuit(List<Tile> hand, Tile.Suit suit)
    {
        List<Tile> output = new List<Tile>();

        foreach(Tile tile in hand)
        {
            if(tile.suit == suit)
                output.Add(tile);
        }

        return output;
    }

    public static int CountOfTile(List<Tile> tiles, Tile compare)
    {
        int count = 0;
        foreach(var tile in tiles)
        {
            if(compare.Equals(tile))
                count++;
        }
        return count;
    }

    public static Tile[] CreateSequenceLowestValue(Dictionary<int, List<Tile>> tiles, Tile input)
    {
        if(tiles.Count < 3)
            return null;
        int count = 1;
        for(int i = (int)input.face - 2; i <= (int)input.face + 2; i++, count++)
        {
            if(!tiles.ContainsKey(i))
                count = 0;
            if(count == 3)
            {
                return new Tile[3] { tiles[i - 2][0], tiles[i - 1][0], tiles[i][0] };
            }
        }

        return null;
    }
    public static Tile[] CreateSequenceHighestValue(Dictionary<int, List<Tile>> tiles, Tile input)
    {
        if(tiles.Count < 3)
            return null;
        int count = 1;
        for(int i = (int)input.face + 2; i >= (int)input.face - 2; i--, count++)
        {
            if(tiles[i].Count == 0)
                count = 0;
            if(count == 3)
            {
                return new Tile[3] { tiles[i + 2][0], tiles[i + 1][0], tiles[i][0] };
            }
        }

        return null;
    }
    //public static Tile[] CreateSequenceLowestCommon(Dictionary<int, List<Tile>> tiles, Tile input)
    //{
    //    if(tiles.Count < 3)
    //        return null;
    //    int count = 1;
    //    for(int i = input.rawValue + 2; i >= input.rawValue - 2; i--, count++)
    //    {
    //        if(tiles[i].Count == 0)
    //            count = 0;
    //        if(count == 3)
    //        {
    //            return new Tile[3] { tiles[i + 2][0], tiles[i + 1][0], tiles[i][0] };
    //        }
    //    }

    //    return null;
    //}

    //public static Tile[] CreateSequenceHighestCommon(Dictionary<int, List<Tile>> tiles, Tile input)
    //{
    //    if(tiles.Count < 3)
    //        return null;
    //    int count = 1;
    //    for(int i = input.rawValue + 2; i >= input.rawValue - 2; i--, count++)
    //    {
    //        if(tiles[i].Count == 0)
    //            count = 0;
    //        if(count == 3)
    //        {
    //            return new Tile[3] { tiles[i + 2][0], tiles[i + 1][0], tiles[i][0] };
    //        }
    //    }

    //    return null;
    //}

    public static List<Tile> GetWaitingTiles(List<Tile> tiles)
    {
        Dictionary<Tile.Suit, List<Tile>> tilesBySuit = ArrangeTilesBySuit(tiles);
        List<Tile.Suit> failedSuits = new List<Tile.Suit>();

        if(tilesBySuit.Count > 1)
        {
            foreach(var pair in tilesBySuit)
            {
                if(!MakeAllMelds(pair.Value))
                    failedSuits.Add(pair.Key);
                else if(!MakeAllMelds2(pair.Value))
                    failedSuits.Add(pair.Key);
            }
        }
        else
        {
            Tile.Suit[] keyArray = new Tile.Suit[1];
            tilesBySuit.Keys.CopyTo(keyArray, 0);
            failedSuits.Add(keyArray[0]);
        }

        if(failedSuits.Count > 1)
            return new List<Tile>();

        return FindWaitingTiles(tilesBySuit[failedSuits[0]]);
    }

    static bool MakeAllMelds(List<Tile> tiles)
    {
        if(tiles.Count < 2)
            return false;

        Dictionary<Tile.Face, List<Tile>> tileByValues = ArrangeTilesByValue(tiles);
        HashSet<int> errorIndex = new HashSet<int>();
        int round = tiles.Count % 3;
        int count = (round == 0) ? tiles.Count / 3 : (tiles.Count / 3) + 1;

        for(int i = 0; i < count; i++)
        {
            Tile.Face lowestValue = (Tile.Face)99;
            int lowestCount = 99;
            if(count == 1)
            {
                Tile.Face[] keys = new Tile.Face[tileByValues.Count];
                tileByValues.Keys.CopyTo(keys, 0);
                lowestValue = keys[0];
            }
            else
            {
                foreach(var pair in tileByValues)
                {
                    if(pair.Value.Count < lowestCount)
                    {
                        lowestCount = pair.Value.Count;
                        lowestValue = pair.Key;
                    }
                }
            }

            int duplicateCount = tileByValues[lowestValue].Count;
            if(FindSequenceLower(tileByValues, lowestValue))
            {
            }
            else if(FindSame(tileByValues, lowestValue))
            {
                if(duplicateCount == 4)
                    count--;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    static bool MakeAllMelds2(List<Tile> tiles)
    {
        if(tiles.Count < 2)
            return false;

        Dictionary<Tile.Face, List<Tile>> tileByValues = ArrangeTilesByValue(tiles);
        HashSet<int> errorIndex = new HashSet<int>();
        int round = tiles.Count % 3;
        int count = (round == 0) ? tiles.Count / 3 : (tiles.Count / 3) + 1;

        for(int i = 0; i < count; i++)
        {
            Tile.Face lowestValue = (Tile.Face)99;
            int lowestCount = 99;

            if(count == 1)
            {
                Tile.Face[] keys = new Tile.Face[tileByValues.Count];
                tileByValues.Keys.CopyTo(keys, 0);
                lowestValue = keys[0];
            }
            else
            {
                foreach(var pair in tileByValues)
                {
                    if(pair.Value.Count < lowestCount)
                    {
                        lowestCount = pair.Value.Count;
                        lowestValue = pair.Key;
                    }
                }
            }

            int duplicateCount = tileByValues[lowestValue].Count;
            if(FindSequenceHigher(tileByValues, lowestValue))
            {
            }
            else if(FindSame(tileByValues, lowestValue))
            {
                if(duplicateCount == 4)
                    count--;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    static List<Tile> FindWaitingTiles(List<Tile> tiles)
    {
        List<Tile> waitingTiles = new List<Tile>();
        if(tiles[0].suit != Tile.Suit.Honor)
        {
            for(int i = 0; i < Tile.numberCount; i++)
            {
                List<Tile> fakeHand = new List<Tile>(tiles);
                fakeHand.Add(new Tile(tiles[0].suit, (Tile.Face)i));
                if(MakeAllMelds(fakeHand))
                {
                    waitingTiles.Add(fakeHand[fakeHand.Count - 1]);
                }
                else if(MakeAllMelds2(fakeHand))
                {
                    waitingTiles.Add(fakeHand[fakeHand.Count - 1]);
                }
            }
        }
        else
        {
            Dictionary<Tile.Face, List<Tile>> tileMap = ArrangeTilesByValue(tiles);
            foreach(var pair in tileMap)
            {
                List<Tile> fakeHand = new List<Tile>(tiles);
                fakeHand.Add(new Tile(Tile.Suit.Honor, pair.Value[0].face));
                if(MakeAllMelds(fakeHand))
                {
                    waitingTiles.Add(fakeHand[fakeHand.Count - 1]);
                }
                else if(MakeAllMelds2(fakeHand))
                {
                    waitingTiles.Add(fakeHand[fakeHand.Count - 1]);
                }
            }
        }

        return waitingTiles;
    }

    static bool FindSequenceLower(Dictionary<Tile.Face, List<Tile>> tileByValues, Tile.Face startingValue)
    {
        if(tileByValues[startingValue][0].suit == Tile.Suit.Honor)
            return false;

        int count = 0;
        for(int i = (int)startingValue - 3; i < (int)startingValue + 3; i++, count++)
        {
            if(!tileByValues.ContainsKey((Tile.Face)i))
            {
                count = 0;
            }
            if(count == 3)
            {
                tileByValues[(Tile.Face)i - 2].RemoveAt(0);
                tileByValues[(Tile.Face)i - 1].RemoveAt(0);
                tileByValues[(Tile.Face)i].RemoveAt(0);

                if(tileByValues[(Tile.Face)i - 2].Count == 0)
                {
                    tileByValues.Remove((Tile.Face)i - 2);
                }
                if(tileByValues[(Tile.Face)i - 1].Count == 0)
                {
                    tileByValues.Remove((Tile.Face)i - 1);
                }
                if(tileByValues[(Tile.Face)i].Count == 0)
                {
                    tileByValues.Remove((Tile.Face)i);
                }
                return true;
            }
        }
        return false;
    }
    static bool FindSequenceHigher(Dictionary<Tile.Face, List<Tile>> tileByValues, Tile.Face startingValue)
    {
        if(tileByValues[startingValue][0].suit == Tile.Suit.Honor)
            return false;

        int count = 0;
        for(int i = (int)startingValue + 3; i > (int)startingValue - 3; i--, count++)
        {
            if(!tileByValues.ContainsKey((Tile.Face)i))
            {
                count = 0;
            }
            if(count == 3)
            {
                tileByValues[(Tile.Face)i + 2].RemoveAt(0);
                tileByValues[(Tile.Face)i + 1].RemoveAt(0);
                tileByValues[(Tile.Face)i].RemoveAt(0);

                if(tileByValues[(Tile.Face)i + 2].Count == 0)
                {
                    tileByValues.Remove((Tile.Face)i + 2);
                }
                if(tileByValues[(Tile.Face)i + 1].Count == 0)
                {
                    tileByValues.Remove((Tile.Face) i + 1);
                }
                if(tileByValues[(Tile.Face)i].Count == 0)
                {
                    tileByValues.Remove((Tile.Face) i);
                }
                return true;
            }
        }
        return false;
    }

    static bool FindSame(Dictionary<Tile.Face, List<Tile>> tileByValues, Tile.Face startingValue)
    {
        if(tileByValues[startingValue].Count < 2)
        {
            return false;
        }

        tileByValues.Remove(startingValue);
        return true;
    }

    public static bool IsSimple(Tile tile)
    {
        return !IsTerminal(tile);
    }
    public static bool IsTerminal(Tile tile)
    {
        return (!IsHonor(tile)) && (tile.face == Tile.Face.One || tile.face == Tile.Face.Nine);
    }
    public static bool IsHonor(Tile tile)
    {
        return tile.suit == Tile.Suit.Honor;
    }
    public static bool IsDragon(Tile tile)
    {
        if(!IsHonor(tile))
            return false;

        return (byte)tile.face >= Tile.dragonMin && (byte)tile.face < Tile.dragonMax;
    }
    public static bool IsPrevalentWind(Tile tile, Tile.Face prevalentWind)
    {
        if(!IsHonor(tile))
            return false;

        return tile.face == prevalentWind;
    }
    public static bool IsSeatWind(Tile tile, Tile.Face seatWind)
    {
        if(!IsHonor(tile))
            return false;

        return tile.face == seatWind;
    }
}
