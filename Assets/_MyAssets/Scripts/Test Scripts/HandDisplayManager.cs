using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDisplayManager : MonoBehaviour
{
    public List<TileDisplay> tiles { get; private set; }
    public RectTransform meldPile;

    public MeldDisplay meldPrefab;
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
        foreach(var tile in meld.tiles)
        {
            TileDisplay findTile = tiles.Find((TileDisplay obj) => { return obj.tile.Equals(tile); });
            toMeld.Add(findTile);
            tiles.Remove(findTile);

            SwapTilePos(findTile, tiles[tiles.Count - 1]);
        }

        MeldDisplay display = Instantiate(meldPrefab, meldPile);
        display.SetTiles(toMeld, inputTile, meld.type);

        float totalOffset = 0;
        foreach(var displayMelds in melds)
        {
            totalOffset += displayMelds.totalLength;
        }
        (display.transform as RectTransform).anchoredPosition = new Vector2(-totalOffset, 0);

        melds.Add(display);

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

    void SwapTilePos(TileDisplay lh, TileDisplay rh)
    {
        Vector3 tempPos = lh.transform.position;
        lh.transform.position = rh.transform.position;
        rh.transform.position = tempPos;
    }
}
