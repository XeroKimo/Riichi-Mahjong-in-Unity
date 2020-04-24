using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternsMatched
{
    public PatternsMatched()
    {
        totalHanValue = 0;
        patternsMatched = new List<IHandPattern>();
    }
    public int totalHanValue;
    public List<IHandPattern> patternsMatched;
}

public static class HandPatternRegistry
{
    static List<IHandPattern> s_handPatterns;

    public static void RegisterHandPattern(IHandPattern pattern)
    {
        if (s_handPatterns == null)
            s_handPatterns = new List<IHandPattern>();

        s_handPatterns.Add(pattern);
    }

    public static PatternsMatched ComputeHand(List<Meld> melds, bool isOpenHand, Tile.Face prevalentWind, Tile.Face seatWind, ref int flags)
    {
        PatternsMatched matched = new PatternsMatched();
        foreach (var pattern in s_handPatterns)
        {
            int hanValue = pattern.GetHandValue(melds, isOpenHand, prevalentWind, seatWind, ref flags);
            if (hanValue == 0)
                continue;
            matched.totalHanValue += hanValue;
            matched.patternsMatched.Add(pattern);
        }

        return matched;
    }
}
