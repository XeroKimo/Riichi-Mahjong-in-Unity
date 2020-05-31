using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Meld
{
    public Tile[] tiles { get; private set; }
    public Type type { get; private set; }
    public bool open { get; private set; }


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
    private bool IsSequenceOrdered()
    {
        for(int i = 0; i < 2; i++)
        {
            if((int)tiles[i].face + 1 != (int)tiles[i + 1].face)
                return false;
        }
        return true;
    }
    private void OrderSequence()
    {
        for(int i = 0; i < 3; i++)
        {
            for(int v = 0; v < 3; v++)
            {
                if((int)tiles[i].face > (int)tiles[v].face)
                {
                    Tile temp = tiles[i];
                    tiles[i] = tiles[v];
                    tiles[v] = temp;
                }
            }
        }
    }

    public Meld(Tile[] tiles, bool open)
    {
        this.tiles = tiles;
        this.type = Type.Pair;
        this.open = open;

        this.type = DetermineType(this.tiles);
        if(type == Type.Pair)
            if(!IsSequenceOrdered())
                OrderSequence();
    }
    public Meld(List<Tile> tiles, bool open) : this(tiles.ToArray(), open)
    {
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
            if (tiles[i].face != other.tiles[i].face)
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



    public static readonly Meld emptyMeld = new Meld();
    public enum Type
    {
        Pair,
        Triple,
        Quad,
        Sequence
    }
}
