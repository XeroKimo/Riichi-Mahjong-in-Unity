using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//Version: 0.32

public struct Meld
{
    public readonly Tile[] tiles;
    public readonly DiscardedTile discardedTile;

    public readonly Type type;
    public readonly bool isOpen;

    public Meld(Tile[] tiles)
    {
        this.tiles = tiles;
        discardedTile = new DiscardedTile(Tile.nullTile, DiscardedTile.noOwnerID);

        this.type = Type.Pair;
        this.isOpen = false;

        this.type = DetermineType(tiles);

        if(type == Type.Sequence)
        {
            if(!IsSequenceOrdered())
                OrderSequence();
        }
    }

    public Meld(Tile[] tiles, DiscardedTile discardedTile, bool isOpen) : this(tiles)
    {
        this.discardedTile = discardedTile;
        this.isOpen = isOpen;
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
    public override bool Equals(object obj)
    {
        if (!(obj is Meld))
            return false;

        Meld meld = (Meld)obj;
        if (type != meld.type)
                return false;
        for (int i = 0; i < meld.tiles.Length; i++)
        {
            if (meld.tiles[i] != tiles[i])
                return false;
        }
        return true;
    }

    public static bool operator==(Meld lh, Meld rh)
    {
        return lh.Equals(rh);
    }
    public static bool operator !=(Meld lh, Meld rh)
    {
        return !lh.Equals(rh);
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
            if (tile == compare)
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
