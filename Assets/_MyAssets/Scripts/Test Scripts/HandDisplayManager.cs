using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDisplayManager : MonoBehaviour
{
    public MeldDisplay meldPrefab;
    public List<TileDisplay> tiles { get; private set; }
    public Transform hand;
    public Transform meldPile;

    public List<MeldDisplay> melds;

    private void Awake()
    {
        tiles = new List<TileDisplay>();
        //for(int i = 0; i < 13; i++)
        //{
        //    tiles.Add(Instantiate(tilePrefab, hand));
        //    (tiles[i].transform as RectTransform).anchoredPosition = new Vector2((tilePrefab.transform as RectTransform).rect.width * i, 0);
        //}
    }

    public void AddTile(TileDisplay tile)
    {
        tiles.Add(tile);

        RectTransform tileTransform = tile.transform as RectTransform;
        Vector2 tileSize = tileTransform.rect.size;
        float xOffset = tileSize.x * (tiles.Count - 1);

        tileTransform.SetParent(hand);
        tileTransform.anchoredPosition = new Vector2(xOffset, 0);
    }

    public void AddDrawnTile(TileDisplay tile)
    {
        AddTile(tile);

        RectTransform tileTransform = tile.transform as RectTransform;
        Vector2 tileSize = tileTransform.rect.size;
        float xOffset = tileTransform.anchoredPosition.x;

        tileTransform.anchoredPosition = new Vector2(xOffset + tileSize.x / 2, 0);
    }

    public TileDisplay RemoveTile(Tile tile)
    {
        TileDisplay toRemoveTile = tiles.Find((TileDisplay match) => { return match.tile == tile; });
        tiles.Remove(toRemoveTile);
        return toRemoveTile;
    }

    public void RefreshHand(List<TileDisplay> tiles)
    {
        this.tiles = tiles; 
    }

    public void OnHandCallMade(List<Tile> tiles)
    {
        TileDisplay inputTile = this.tiles[this.tiles.Count - 1];
        List<TileDisplay> toMeld = new List<TileDisplay>();
        foreach(var tile in tiles)
        {
            toMeld.Add(RemoveTile(tile));
        }
        

        //Spawn meld object
        //Transfer tiles into meld
        //Offset meld

    }
}
