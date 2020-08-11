using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

//Version: 0.32
public struct Tile
{
    byte m_tileID;
    public Suit suit { get => (Suit)(m_tileID >> suitBitOffset); }
    public Face face { get => (Face)((m_tileID | ((suitCount - 1) << suitBitOffset)) ^ ((suitCount - 1) << suitBitOffset)); }


    public Tile(Suit suit, Face face)
    {
        m_tileID = 0;
        m_tileID += (byte)((byte)suit << suitBitOffset);
        m_tileID += (byte)face;
    }
    public Tile(Tile other)
    {
        m_tileID = other.m_tileID;
    }
    private Tile(byte value)
    {
        m_tileID = value;
    }

    public static bool operator ==(Tile lh, Tile rh)
    {
        return lh.m_tileID == rh.m_tileID;
    }
    public static bool operator !=(Tile lh, Tile rh)
    {
        return lh.m_tileID != rh.m_tileID;
    }
    public override bool Equals(object obj)
    {
        return obj is Tile tile &&
               m_tileID == tile.m_tileID;
    }
    public override int GetHashCode()
    {
        var hashCode = -1733114086;
        hashCode = hashCode * -1521134295 + m_tileID.GetHashCode();
        return hashCode;
    }


    public static readonly Tile nullTile = new Tile(byte.MaxValue);
    const byte suitBitOffset = 6;
    public enum Suit : byte
    {
        Honor = 0,
        Character,
        Circle,
        Bamboo
    }
    public enum Face : byte
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,

        East = 0,
        South = 1,
        West = 2,
        North = 3,
        Green = 4,
        Red = 5,
        White = 6
    }

    public const byte suitCount = 4;
    public const byte numberCount = 9;
    public const byte honorCount = 7;
    public const byte windMin = 0;
    public const byte windMax = 3;
    public const byte dragonMin = 4;
    public const byte dragonMax = 7;
}

public struct DiscardedTile
{
    public readonly Tile tile;
    public readonly byte originalOwnerID;

    public const byte noOwnerID = byte.MaxValue;

    public DiscardedTile(Tile tile, byte originalOwnerID)
    {
        this.tile = tile;
        this.originalOwnerID = originalOwnerID;
    }
}