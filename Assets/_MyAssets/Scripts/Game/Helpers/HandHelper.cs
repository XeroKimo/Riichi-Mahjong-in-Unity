using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;


//Version: 0.32

public static class HandHelper
{

    enum MeldSearchOrder
    {
        SequenceLowestFirst,
        SequenceHighestFirst,
        SameFace
    }

    //Gets the remaining tiles required to be able to have a complete hand
    //Only works if you're in Tenpai
    public static List<Tile> GetRemainingTilesInTenpai(Hand hand, out bool inTenpai)
    {
        inTenpai = false;
        Dictionary<Tile.Suit, List<Tile>> arrangedTiles = TileHelper.ArrangeTilesBySuit(hand.tiles);
        HashSet<Tile.Suit> failedSuits = new HashSet<Tile.Suit>();

        Dictionary<Meld.Type, List<Meld>> allMeldsMade = MeldHelper.ArrangeMeldsByType(hand.melds);
        List<Tile> allRemainingTiles = new List<Tile>();

        for(int i = 0; i < 4; i++)
        {
            Meld.Type type = (Meld.Type)i;
            Tile.Suit suit = (Tile.Suit)i;
            if(!allMeldsMade.ContainsKey(type))
                allMeldsMade[type] = new List<Meld>();

        }


        //NOTES:
        //Remaining tiles will be used to quickly determine if I'm missing a pair, just use the last tile not in a meld
        //Melds that were made in the process will determine if I'm in tenpai
        //Failed suits will be used to find which suit to attempt to find all melds

        Dictionary<Tile.Suit, List<Tile>> remainingTilesBySuit = new Dictionary<Tile.Suit, List<Tile>>();
        foreach(var pair in arrangedTiles)
        {
            if(!MakeAllMeldsBestResult(pair.Value, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles))
            {
                failedSuits.Add(pair.Key);
            }
            allRemainingTiles.AddRange(remainingTiles);
            foreach(var meldPair in meldsMade)
            {
                allMeldsMade[meldPair.Key].AddRange(meldPair.Value);
            }
        }

        int pairCount = allMeldsMade[Meld.Type.Pair].Count;
        int totalMeldCount = allMeldsMade[Meld.Type.Sequence].Count + allMeldsMade[Meld.Type.Triple].Count + allMeldsMade[Meld.Type.Quad].Count;

        if(!((totalMeldCount == 4 && pairCount == 0) ||
              (totalMeldCount == 3 && pairCount == 2) ||
              (pairCount == 6)))
        {
            return new List<Tile>();
        }

        inTenpai = true;

        if(pairCount == 2)
        {
            List<Tile> tilesNeeded = new List<Tile>();
            foreach(Meld meld in allMeldsMade[Meld.Type.Pair])
            {
                tilesNeeded.Add(meld.tiles[0]);
            }
            return tilesNeeded;
        }
        if(allRemainingTiles.Count == 1)
        {
            return allRemainingTiles;
        }

        Tile.Suit[] failedSuitsArray = new Tile.Suit[1];
        failedSuits.CopyTo(failedSuitsArray, 0, 1);
        return FindAllPossibleMissingTiles(arrangedTiles[failedSuitsArray[0]]);
    }

    //Tries all search orders and if none can make all melds, return the one closest to making all melds
    private static bool MakeAllMeldsBestResult(List<Tile> tiles, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles)
    {
        if(MakeAllMeldsSequenceLowestFirst(tiles, out meldsMade, out remainingTiles))
        {
            return true;
        }
        else if(MakeAllMeldsSequenceHighestFirst(tiles, out meldsMade, out remainingTiles))
        {
            return true;
        }
        else
        {
            return MakeAllMeldsSameFace(tiles, out meldsMade, out remainingTiles);
        }
    }

    private static bool MakeAllMeldsSequenceLowestFirst(List<Tile> tiles, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles)
    {
        HashSet<MeldSearchOrder> searchOrder = new HashSet<MeldSearchOrder>();
        searchOrder.Add(MeldSearchOrder.SequenceLowestFirst);
        searchOrder.Add(MeldSearchOrder.SequenceHighestFirst);
        searchOrder.Add(MeldSearchOrder.SameFace);

        return MakeAllMelds(tiles, searchOrder, true, out meldsMade, out remainingTiles);
    }

    private static bool MakeAllMeldsSequenceHighestFirst(List<Tile> tiles, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles)
    {
        HashSet<MeldSearchOrder> searchOrder = new HashSet<MeldSearchOrder>();
        searchOrder.Add(MeldSearchOrder.SequenceHighestFirst);
        searchOrder.Add(MeldSearchOrder.SequenceLowestFirst);
        searchOrder.Add(MeldSearchOrder.SameFace);

        return MakeAllMelds(tiles, searchOrder, true, out meldsMade, out remainingTiles);
    }

    private static bool MakeAllMeldsSameFace(List<Tile> tiles, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles)
    {
        HashSet<MeldSearchOrder> searchOrder = new HashSet<MeldSearchOrder>();
        searchOrder.Add(MeldSearchOrder.SameFace);
        searchOrder.Add(MeldSearchOrder.SequenceLowestFirst);
        searchOrder.Add(MeldSearchOrder.SequenceHighestFirst);

        return MakeAllMelds(tiles, searchOrder, false, out meldsMade, out remainingTiles);
    }

    //Attempt to use the given set of tiles to make melds
    private static bool MakeAllMelds(List<Tile> tiles, HashSet<MeldSearchOrder> searchOrder, bool startLowestCommonTile, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles)
    {
        meldsMade = new Dictionary<Meld.Type, List<Meld>>();
        remainingTiles = new List<Tile>(tiles);
        Dictionary<Tile.Face, List<Tile>> tilesByFace = TileHelper.ArrangeTilesByFace(tiles);

        for(int i = 0; i < 4; i++)
        {
            Meld.Type meldType = (Meld.Type)i;
            meldsMade[meldType] = new List<Meld>();
        }

        while(remainingTiles.Count > 0)
        {
            Tile.Face currentFace = (startLowestCommonTile) ? GetLowestCommonTile(tilesByFace) : GetHighestCommonTile(tilesByFace);

            bool meldFound = false;

            foreach(MeldSearchOrder order in searchOrder)
            {
                List<Meld> outMelds;
                Meld outMeld;
                switch(order)
                {
                case MeldSearchOrder.SequenceLowestFirst:
                    meldFound = MeldHelper.MakePossibleSequences(tilesByFace[currentFace][0], tilesByFace, out outMelds);
                    outMeld = (outMelds.Count > 0) ? outMelds[0] : Meld.emptyMeld;
                    break;
                case MeldSearchOrder.SequenceHighestFirst:
                    meldFound = MeldHelper.MakePossibleSequences(tilesByFace[currentFace][0], tilesByFace, out outMelds);
                    outMeld = (outMelds.Count > 0) ? outMelds[outMelds.Count - 1] : Meld.emptyMeld;
                    break;
                case MeldSearchOrder.SameFace:
                    meldFound = MeldHelper.MakeTriple(tilesByFace[currentFace][0], tilesByFace[currentFace], out outMeld);
                    if(!meldFound)
                        meldFound = MeldHelper.MakePair(tilesByFace[currentFace][0], tilesByFace[currentFace], out outMeld);
                    break;
                default:
                    outMeld = Meld.emptyMeld;
                    break;
                }

                if(meldFound)
                {
                    meldsMade[outMeld.type].Add(outMeld);

                    foreach(Tile tile in outMeld.tiles)
                    {
                        remainingTiles.Remove(tile);
                        tilesByFace[tile.face].RemoveAt(0);
                    }
                    break;
                }
            }

            if(!meldFound)
                return false;
        }

        return true;
    }

    private static Tile.Face GetLowestCommonTile(Dictionary<Tile.Face, List<Tile>> tilesByFace)
    {
        Tile.Face lowestFace = Tile.Face.East;
        int lowestCount = 99;
        foreach(var pair in tilesByFace)
        {
            if(pair.Value.Count < lowestCount)
            {
                lowestFace = pair.Key;
                lowestCount = pair.Value.Count;
            }
        }

        return lowestFace;
    }

    private static Tile.Face GetHighestCommonTile(Dictionary<Tile.Face, List<Tile>> tilesByFace)
    {
        Tile.Face highestFace = Tile.Face.East;
        int highestCount = 0;
        foreach(var pair in tilesByFace)
        {
            if(pair.Value.Count > highestCount)
            {
                highestFace = pair.Key;
                highestCount = pair.Value.Count;
            }
        }

        return highestFace;
    }

    //Finds all possible tiles that would complete the hand
    private static List<Tile> FindAllPossibleMissingTiles(List<Tile> tiles)
    {
        List<Tile> foundTiles = new List<Tile>();
        Tile.Suit suitToUse = tiles[0].suit;
        int faceCount = (suitToUse == Tile.Suit.Honor) ? Tile.honorCount : Tile.numberCount;

        for(int i = 0; i < faceCount; i++)
        {
            List<Tile> copy = new List<Tile>(tiles);
            copy.Add(new Tile(suitToUse, (Tile.Face)i));
            if(MakeAllMeldsBestResult(copy, out Dictionary<Meld.Type, List<Meld>> meldsMade, out List<Tile> remainingTiles))
            {
                foundTiles.Add(new Tile(suitToUse, (Tile.Face)i));
            }
        }

        return foundTiles;
    }
}
