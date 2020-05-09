﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MeldDisplay : MonoBehaviour
{
    List<TileDisplay> tiles;
    public float totalLength;
    private void Awake()
    {
        tiles = new List<TileDisplay>();
    }

    public void SetTiles(List<TileDisplay> tiles, TileDisplay inputTile, Meld.Type type)
    {
        this.tiles = tiles;
        Rect rect = (tiles[0].transform as RectTransform).rect;

        float width = rect.width;
        totalLength = width * (tiles.Count - 1);
        totalLength += rect.height;

        for(int i = tiles.Count - 1; i >= 0; i--)
        {
            RectTransform rectTransform = tiles[i].transform as RectTransform;
            rectTransform.transform.SetParent(transform);
            rectTransform.pivot = rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(1.0f, 0.5f);
            rectTransform.localScale = Vector2.one;
            rectTransform.anchoredPosition = new Vector2(-width * i, 0);
        }
    }


}
