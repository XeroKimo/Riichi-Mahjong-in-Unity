using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDisplayManager : MonoBehaviour
{
    List<TileDisplay> m_tiles;
    public TileDisplayPool displayPool;

    private void Awake()
    {
        m_tiles = new List<TileDisplay>();
    }

    public void AddTile(Tile tile, IPlayerActions player)
    {
        TileDisplay tileDisplay = displayPool.GetTileFromPool();
        tileDisplay.SetTile(tile);
        tileDisplay.SetOwningPlayer(player);

        m_tiles.Add(tileDisplay);

        RectTransform tileTransform = tileDisplay.transform as RectTransform;
        Vector2 tileSize = tileTransform.rect.size;
        float xOffset = tileSize.x * (m_tiles.Count - 1);

        tileTransform.SetParent(transform);
        tileTransform.pivot = tileTransform.anchorMin = tileTransform.anchorMax = new Vector2(0, 0.5f);
        tileTransform.localScale = Vector3.one;
        tileTransform.localRotation = Quaternion.identity;

        tileTransform.anchoredPosition = new Vector2(xOffset, 0);
    }

    public void AddDrawnTile(Tile tile, IPlayerActions player)
    {
        AddTile(tile, player);

        RectTransform tileTransform = m_tiles[m_tiles.Count - 1].transform as RectTransform;
        Vector2 tileSize = tileTransform.rect.size;
        float xOffset = tileTransform.anchoredPosition.x;

        tileTransform.anchoredPosition = new Vector2(xOffset + tileSize.x / 2, 0);
    }

    public void RemoveTile(Tile tile)
    {
        TileDisplay findTile = m_tiles.Find((TileDisplay match) => { return match.tile == tile; });
        if(findTile)
        {
            SwapTiles(findTile, m_tiles[m_tiles.Count - 1]);
            m_tiles.Remove(m_tiles[m_tiles.Count - 1]);
        }
    }

    void SwapTiles(TileDisplay lh, TileDisplay rh)
    {
        Tile temp = lh.tile;
        lh.SetTile(rh.tile);
        rh.SetTile(temp);
    }

    public void Clear(Hand hand, IPlayerActions playerActions)
    {
        foreach(TileDisplay tile in m_tiles)
        {
            tile.SetTile(Tile.nullTile);
        }
        m_tiles.Clear();

        foreach(Tile tile in hand.tiles)
        {
            AddTile(tile, playerActions);
        }
    }
}
