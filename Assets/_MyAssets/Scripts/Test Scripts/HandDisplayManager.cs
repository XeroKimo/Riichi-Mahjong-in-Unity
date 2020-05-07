using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDisplayManager : MonoBehaviour
{
    public List<TileDisplay> tiles { get; private set; }
    public Transform meldPile;

    public List<MeldDisplay> melds;

    private void Awake()
    {
        tiles = new List<TileDisplay>();
        melds = new List<MeldDisplay>();
    }

    public void AddTile(TileDisplay tile)
    {
        tiles.Add(tile);

        RectTransform tileTransform = tile.transform as RectTransform;
        Vector2 tileSize = tileTransform.rect.size;
        float xOffset = tileSize.x * (tiles.Count - 1);

        tileTransform.SetParent(transform);
        tileTransform.localScale = Vector3.one;
        tileTransform.localRotation = Quaternion.identity;

        tileTransform.anchoredPosition = new Vector2(xOffset, 0);
    }

    public void AddDrawnTile(TileDisplay tile)
    {
        AddTile(tile);

        RectTransform tileTransform = tile.transform as RectTransform;
        Vector2 tileSize = tileTransform.rect.size;
        float xOffset = tileTransform.anchoredPosition.x;

        tileTransform.SetParent(transform);
        tileTransform.localScale = Vector3.one;
        tileTransform.localRotation = Quaternion.identity;

        tileTransform.anchoredPosition = new Vector2(xOffset + tileSize.x / 2, 0);
    }

    public TileDisplay RemoveTile(Tile tile)
    {
        TileDisplay toRemoveTile = tiles.Find((TileDisplay match) => { return match.tile == tile; });
        TileDisplay lastTile = tiles[tiles.Count - 1];
        tiles.Remove(lastTile);

        toRemoveTile.SetTile(lastTile.tile);
        lastTile.SetTile(tile);

        return lastTile;
    }

    public void RefreshHand(List<TileDisplay> tiles)
    {
        this.tiles = tiles; 
    }

    public void DisplayMeld(Meld meld)
    {
        TileDisplay inputTile = this.tiles[this.tiles.Count - 1];
        List<TileDisplay> toMeld = new List<TileDisplay>();
        //foreach(var tile in tiles)
        //{
        //    toMeld.Add(RemoveTile(tile));
        //}
        

        //Spawn meld object
        //Transfer tiles into meld
        //Offset meld

    }

    public void ClearHand()
    {
        foreach(var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();

        foreach(var meld in melds)
        {
            Destroy(meld.gameObject);
        }
        melds.Clear();
    }
}
