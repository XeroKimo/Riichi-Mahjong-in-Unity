using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.32

public struct Hand
{
    public List<Tile> tiles;
    public List<Meld> melds;

    public void Init()
    {
        tiles = new List<Tile>();
        melds = new List<Meld>();
    }
}
