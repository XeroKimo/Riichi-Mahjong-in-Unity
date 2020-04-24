using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HandPatternRiichi : IHandPattern
{
    int m_closedValue;
    int m_openValue;
    bool m_isClosedOnly;
    string m_description;
    string m_patternName;

    public delegate int ValidateHand(List<Meld> melds, Tile.Face prevalentWind, Tile.Face seatWind, ref int flags);

    ValidateHand m_validateHand;

    public HandPatternRiichi(int closedValue, int openValue, bool isClosedOnly, string description, string patternName, ValidateHand validateHand)
    {
        m_closedValue = closedValue;
        m_openValue = openValue;
        m_isClosedOnly = isClosedOnly;
        m_description = description;
        m_patternName = patternName;
        m_validateHand = validateHand;
    }    
    
    public HandPatternRiichi(PatternInitializer initializer)
    {
        m_closedValue =     initializer.closedValue;
        m_openValue =       initializer.openValue;
        m_isClosedOnly =    initializer.isClosedOnly;
        m_description =     initializer.description;
        m_patternName =     initializer.name;
        m_validateHand = initializer.validateFunction;
    }

    public int closedValue { get => m_closedValue; }

    public int openValue { get => m_openValue; }

    public bool isClosedOnly { get => m_isClosedOnly; }

    public string descripiton{ get => m_description; }

    public string patternName { get => m_patternName; }

    public int GetHandValue(List<Meld> melds, bool isOpenHand, Tile.Face prevalentWind, Tile.Face seatWind, ref int flags)
    {
        if (m_isClosedOnly && isOpenHand)
            return 0;

        int multiplier = m_validateHand(melds, prevalentWind, seatWind, ref flags);

        if (isOpenHand)
            return openValue * multiplier;
        else
            return closedValue * multiplier;
    }

    public struct PatternInitializer
    {
        public int closedValue;
        public int openValue;
        public bool isClosedOnly;
        public string description;
        public string name;
        public ValidateHand validateFunction;
    }
}
