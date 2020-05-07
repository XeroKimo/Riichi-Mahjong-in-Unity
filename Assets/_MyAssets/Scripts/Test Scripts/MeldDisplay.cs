using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MeldDisplay : MonoBehaviour
{
    List<TileDisplay> tiles;

    private void Awake()
    {
        tiles = new List<TileDisplay>();
    }

    public void SetTiles(List<TileDisplay> tiles, TileDisplay inputTile)
    {
        this.tiles = tiles;
    }
}
