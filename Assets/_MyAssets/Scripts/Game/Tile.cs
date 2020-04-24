﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum Suit
    {
        Honor = 0,
        Character,
        Bamboo,
        Circle,
        Count
    }
    public enum Face
    {
        One = 0,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        East,
        South,
        West,
        North,
        Red,
        White,
        Green,
        Count
    }
    static public int numberMin { get => (int)Face.One; }
    static public int numberMax { get => (int)Face.Nine + 1; }
    static public int honorMin { get => (int)Face.East; }
    static public int honorMax { get => (int)Face.Green + 1; }
    static public int windMin { get => (int)Face.East; }
    static public int windMax { get => (int)Face.North + 1; }
    static public int dragonMin { get => (int)Face.Red; }
    static public int dragonMax { get => (int)Face.Green + 1; }

    [SerializeField]
    public Suit suit { get; private set; }
    public int rawValue { get; private set; }
    public Face face { get => (Face)rawValue; private set => this.rawValue = (int)value; }


    public Tile(Suit suit, Face face)
    {
        this.suit = suit;
        this.face = face;
    }

    public Tile(Tile other)
    {
        this.suit = other.suit;
        this.face = other.face;
    }

    public override bool Equals(object obj)
    {
        return obj is Tile tile &&
               suit == tile.suit &&
               rawValue == tile.rawValue;
    }

    public override int GetHashCode()
    {
        var hashCode = -1733114086;
        hashCode = hashCode * -1521134295 + suit.GetHashCode();
        hashCode = hashCode * -1521134295 + rawValue.GetHashCode();
        return hashCode;
    }

    public bool EqualValue(Tile other)
    {
        return rawValue == other.rawValue;
    }

}
