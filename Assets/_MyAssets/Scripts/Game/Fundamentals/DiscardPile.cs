using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile
{
    List<DiscardedTile> m_tiles;

    public DiscardPile()
    {
        m_tiles = new List<DiscardedTile>();
    }
    private void Awake()
    {
        m_tiles = new List<DiscardedTile>();
    }

    public void AddTile(DiscardedTile tile)
    {
        m_tiles.Add(tile);
    }

    public DiscardedTile GetLastDiscardedTile()
    {
        return m_tiles[m_tiles.Count - 1];
    }

    public void Reset()
    {
        m_tiles.Clear();
    }

}
