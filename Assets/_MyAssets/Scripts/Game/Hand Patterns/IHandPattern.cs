using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandPattern
{
    int closedValue { get; }
    int openValue { get; }
    bool isClosedOnly { get; }
    string descripiton { get; } 
    string patternName { get; }

    int GetHandValue(List<Meld> melds, bool isOpenHand, Tile.Face prevalentWind, Tile.Face seatWind, ref int flags);
}
