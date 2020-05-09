using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Meld
{
    public enum Type
    {
        Pair,
        Triple,
        Quad,
        Sequence,
        Count
    }

    public Tile[] tiles { get; private set; }
    public Type type { get; private set; }
    public bool open { get; private set; }

    public static readonly Meld emptyMeld = new Meld();

    public Meld(Tile[] tiles, bool open)
    {
        this.tiles = tiles;
        this.type = Type.Count; 
        this.open = open;

        this.type = DetermineType(this.tiles);
    }

    public Meld(List<Tile> tiles, bool open)
    {
        this.tiles = tiles.ToArray();
        this.type = Type.Count;
        this.open = open;

        this.type = DetermineType(this.tiles);
    }

    private Type DetermineType(Tile[] tiles)
    {
        if (tiles.Length == 2)
            return Type.Pair;
        if (tiles.Length == 4)
            return Type.Quad;
        if (tiles[0] == tiles[1])
            return Type.Triple;
        return Type.Sequence;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Meld))
            return false;

        Meld meld = (Meld)obj;
        if (meld.tiles.Length != tiles.Length)
                return false;
        for (int i = 0; i < meld.tiles.Length; i++)
        {
            if (!meld.tiles[i].Equals(tiles[i]))
                return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hashValue = 0;
        foreach (var tile in tiles)
        {
            hashValue += tile.GetHashCode();
        }
        return 1037213438 + hashValue;
    }

    public bool EqualValues(Meld other)
    {
        if (other.type != type)
            return false;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].EqualValue(other.tiles[i]))
                return false;
        }

        return true;
    }

    public bool Contains(Tile compare)
    {
        foreach (var tile in tiles)
        {
            if (tile.Equals(compare))
                return true;
        }
        return false;
    }
}
