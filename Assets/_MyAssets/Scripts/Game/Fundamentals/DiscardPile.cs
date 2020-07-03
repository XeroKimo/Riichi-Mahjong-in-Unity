using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile
{
    List<Tile> m_tiles;

    public DiscardPile()
    {
        m_tiles = new List<Tile>();
    }
    private void Awake()
    {
        m_tiles = new List<Tile>();
    }

    public void AddTile(Tile tile)
    {
        m_tiles.Add(tile);
    }

    public Tile PeekLastDiscardedTile()
    {
        return m_tiles[m_tiles.Count - 1];
    }

    public Tile RetrieveLastDiscardedTile()
    {
        Tile output = m_tiles[m_tiles.Count - 1];
        m_tiles.Remove(output);
        return output;
    }

    public void Reset()
    {
        m_tiles.Clear();
    }

}
