using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class HandPatterns
{

    #region Sequence based patterns
    //All Sequences
    static public int Pinfu(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> sequences = MeldHelpers.GetMeldsByType(melds, Meld.Type.Sequence);
        if (sequences.Count == 4)
            return 1;

        return 0;
    }

    //Double Sequence
    static public int Iipeikou(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> sequences = MeldHelpers.GetMeldsByType(melds, Meld.Type.Sequence);
        if (sequences.Count < 2)
            return 0;
        if (flags.IsFlagSet(MahjongFlags.Flag.HP_Ryanpeikou))
            return 0;

        int iipeikouCount = 0;
        HashSet<int> values = new HashSet<int>();

        for (int i = 0; i < sequences.Count - 1; i++)
        {
            if (values.Contains(i))
                continue;
            for (int j = i + 1; j < sequences.Count; j++)
            {
                if (values.Contains(j))
                    continue;
                if (sequences[i].Equals(sequences[j].tiles))
                {
                    iipeikouCount++;

                    values.Add(i);
                    values.Add(j);

                    break;
                }
            }
        }
        if (iipeikouCount == 2)
        {
            flags.SetFlag(MahjongFlags.Flag.HP_Ryanpeikou);
            return 0;
        }
        if (iipeikouCount == 1)
            return 1;
        
        return 0;
    }
    //Two double sequences
    static public int Ryanpeikou(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> sequences = MeldHelpers.GetMeldsByType(melds, Meld.Type.Sequence);
        if (sequences.Count < 4)
            return 0;
        if (flags.IsFlagSet(MahjongFlags.Flag.HP_Ryanpeikou))
            return 1;

        int iipeikouCount = 0;
        HashSet<int> values = new HashSet<int>();

        for (int i = 0; i < sequences.Count - 1; i++)
        {
            if (values.Contains(i))
                continue;
            for (int j = i + 1; j < sequences.Count; j++)
            {
                if (values.Contains(j))
                    continue;
                if (sequences[i].Equals(sequences[j].tiles))
                {
                    iipeikouCount++;

                    values.Add(i);
                    values.Add(j);

                    break;
                }
            }
        }
        if (iipeikouCount == 2)
            return 1;
        return 0;
    }

    //Straight
    static public int Ikkitsuukan(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> sequences = MeldHelpers.GetMeldsByType(melds, Meld.Type.Sequence);
        if (sequences.Count < 3)
            return 0;

        Dictionary<Tile.Suit, List<Meld>> meldBySuit = MeldHelpers.ArrangeMeldsBySuit(sequences);
        for (int i = 1; i < (int)Tile.Suit.Count; i++)
        {
            Tile.Suit suit = (Tile.Suit)i;

            if (meldBySuit[suit].Count < 3)
                continue;

            List<List<Tile>> sortTiles = new List<List<Tile>>(4);
            foreach (var sequence in meldBySuit[suit])
            {
                sortTiles.Add(TileHelpers.SortSequence(new List<Tile>(sequence.tiles)));
            }


            int count = 0;
            for (int j = 0; j < 2; j++)
            {
                foreach (var sequence in sortTiles)
                {
                    int multiplier = j * 3;
                    if (IsSequenceValue(sequence, j + multiplier, j + 1 + multiplier, j + 2 + multiplier))
                    {
                        count++;
                        sortTiles.Remove(sequence);
                        break;
                    }
                    if (count == 3)
                        return 1;
                }
            }
        }

        return 0;
    }
    //Triple Run
    static public int SanshoukuDoujin(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> sequences = MeldHelpers.GetMeldsByType(melds, Meld.Type.Sequence);
        if (sequences.Count < 3)
            return 0;

        Dictionary<Tile.Suit, List<Meld>> meldBySuit = MeldHelpers.ArrangeMeldsBySuit(sequences);

        if (meldBySuit[Tile.Suit.Bamboo].Count > 0 && meldBySuit[Tile.Suit.Character].Count > 0 && meldBySuit[Tile.Suit.Circle].Count > 0)
        {
            int highestSequenceCount = 0;
            Tile.Suit baseSuit = Tile.Suit.Honor;
            for (int i = 1; i < (int)Tile.Suit.Count; i++)
            {
                Tile.Suit suit = (Tile.Suit)i;
                if (meldBySuit[suit].Count > highestSequenceCount)
                {
                    baseSuit = suit;
                    highestSequenceCount = meldBySuit[suit].Count;
                }
            }

            //For each Sequence
            for (int i = 0; i < meldBySuit[baseSuit].Count; i++)
            {
                int sameCount = 0;
                //For each suit
                for (int j = 0; j < 2; j++)
                {
                    Tile.Suit suitToCompare = (Tile.Suit)((((int)baseSuit + j) % 4) + 1);
                    if (meldBySuit[baseSuit][i].EqualValues(meldBySuit[suitToCompare][0]))
                        sameCount++;
                }

                if (sameCount == 2)
                    return 1;
            }
        }

        return 0;
    }

    static public bool IsSequenceValue(List<Tile> sequence, int first, int second, int third)
    {
        return sequence[0].rawValue == first && sequence[1].rawValue == second && sequence[2].rawValue == third;
    }

    #endregion

    #region Triplet hands

    //Honor Tiles
    static public int Fanpai(List<Meld> melds, Tile.Face roundWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> triples = MeldHelpers.GetMeldsByType(melds, Meld.Type.Triple);

        if (triples.Count == 0)
            return 0;

        int fanpaiCount = 0;

        foreach (var meld in triples)
        {
            if (TileHelpers.IsDragon(meld.tiles[0]) || TileHelpers.IsPrevalentWind(meld.tiles[0], roundWind) || TileHelpers.IsSeatWind(meld.tiles[0], seatWind))
                fanpaiCount++;
        }

        return fanpaiCount;
    }

    //Dirty Terminals
    static public int Honroutou(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        if (MeldHelpers.CountOfType(melds, Meld.Type.Sequence) > 0)
            return 0;

        foreach (var meld in melds)
        {
            if (MeldHelpers.IsSimple(meld))
                return 0;
        }

        return 1;
    }
    //All triples
    static public int Toitoihou(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> triples = MeldHelpers.GetMeldsByType(melds, Meld.Type.Triple);
        if (triples.Count == 4)
            return 1;
        return 0;
    }

    //Little Three Dragons
    static public int ShouSangen(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        Dictionary<Meld.Type, List<Meld>> meldMap = MeldHelpers.ArrangeMeldsByType(melds);
        if (meldMap[Meld.Type.Pair].Count > 1)
            return 0;

        if (!MeldHelpers.IsDragon(meldMap[Meld.Type.Pair][0]))
            return 0;

        List<Meld> triples = new List<Meld>(meldMap[Meld.Type.Triple]);
        triples.AddRange(meldMap[Meld.Type.Quad]);
        if (triples.Count < 2)
            return 0;

        int count = 0;
        foreach (var meld in triples)
        {
            if (MeldHelpers.IsDragon(meld))
                count++;
        }
        if (count == 2)
            return 1;

        return 0;
    }
    //Three concealed Triples
    static public int SanAnkou(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> triples = MeldHelpers.GetMeldsByType(melds, Meld.Type.Triple);
        triples.AddRange(MeldHelpers.GetMeldsByType(melds, Meld.Type.Quad));
        if (triples.Count < 3)
            return 0;

        int closedCount = 0;
        foreach (var meld in triples)
        {
            if (!meld.open)
                closedCount++;
        }

        return (closedCount == 3) ? 1 : 0;
    }
    //Three Colored Triples
    static public int SanshouDoukou(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> triples = MeldHelpers.GetMeldsByType(melds, Meld.Type.Triple);
        triples.AddRange(MeldHelpers.GetMeldsByType(melds, Meld.Type.Quad));
        if (triples.Count < 3)
            return 0;

        int count = 0;
        Tile baseTile = triples[0].tiles[0];
        for (int i = 1; i < triples.Count; i++)
        {
            if (baseTile.EqualValue(triples[i].tiles[0]))
                count++;
        }

        return (count == 3) ? 1 : 0;
    }
    //Four Quads
    static public int SanKantsu(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        List<Meld> quads = MeldHelpers.GetMeldsByType(melds, Meld.Type.Quad);
        if (quads.Count < 3)
            return 0;

        return 1;
    }

    #endregion

    #region Mixed Hands
    //All simples
    static public int Tanyao(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        foreach (var meld in melds)
        {
            if (!MeldHelpers.IsSimple(meld))
                return 0;
        }

        return 1;
    }
    //Terminal or honor in each meld
    static public int Chantaiyao(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        if (MeldHelpers.CountOfType(melds, Meld.Type.Sequence) < 1)
            return 0;

        foreach (var meld in melds)
        {
            if (MeldHelpers.IsSimple(meld))
                return 0;
        }

        return 1;
    }
    //Terminal in each meld
    static public int JunchanTayao(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        if (MeldHelpers.CountOfType(melds, Meld.Type.Sequence) < 1)
            return 0;

        foreach (var meld in melds)
        {
            if (!MeldHelpers.IsTerminal(meld))
                return 0;
        }

        return 1;
    }
    //Half flust
    static public int Honitsu(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        if (MeldHelpers.CountSuits(melds) > 2)
            return 0;

        if (MeldHelpers.CountSuitsOfType(melds, Tile.Suit.Honor) < 1)
            return 0;

        return 1;
    }
    //Full flush
    static public int Chinitsu(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        if (MeldHelpers.CountSuits(melds) > 1)
            return 0;

        return 1;
    }

    #endregion

    //Seven Pairs
    static public int Chiitoitsu(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, MahjongFlags flags)
    {
        if (MeldHelpers.CountOfType(melds, Meld.Type.Pair) == 7)
            return 1;
        else
            return 0;
    }

    static public Vector2Int CalculateHandValue(int fu, int han, bool dealer, HandCall handCall)
    {
        int baseValue = CalculateBaseHandValue(fu, han);

        Vector2Int values = new Vector2Int();

        int dealerMultiplier = (dealer) ? 1 : 0;

        if ((handCall & HandCall.Tsumo) == HandCall.Tsumo)
        {
            values.x = RoundToNearestHundred(baseValue * 1 + (1 * dealerMultiplier));
            values.y = RoundToNearestHundred(baseValue * 2);
        }
        else if((handCall & HandCall.Ron) == HandCall.Ron)
        {
            values.x = values.y = RoundToNearestHundred(baseValue * (4 + (2 * dealerMultiplier)));
        }

        return values;
    }

    static int RoundToNearestHundred(int value)
    {
        int rounding = value % 100;
        if (rounding > 0)
            return value + (100 - (value % 100));
        return value;
    }

    static int CalculateBaseHandValue(int fu, int han)
    {
        if (han > 4)
            return GetMangenValue(han);

        int value = fu * Pow(2, 2 + han);
        if (value > 2000)
            value = 2000;
        return value;
    }

    static int Pow(int baseValue, int power)
    {
        if (power == 0)
            return 1;

        return baseValue * Pow(baseValue, power - 1);
    }

    static int GetMangenValue(int han)
    {
        if (han == 5)
            return 2000;
        if (han >= 6 && han <= 7)
            return 3000;
        if (han >= 8 && han <= 10)
            return 4000;
        if (han >= 11 && han <= 12)
            return 6000;
        if (han >= 13)
            return 8000;
        return 0;
    }
}
