using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardDisplayManager : MonoBehaviour
{
    Stack<TileDisplay> m_discardedTiles;

    private void Awake()
    {
        m_discardedTiles = new Stack<TileDisplay>();
    }

    public void AddTile(TileDisplay tileDisplay)
    {
        
    }

    public TileDisplay PeekLastDiscardedTile()
    {
        return null;
    }

    public TileDisplay ExtractLastDiscardedTile()
    {
        return null;
    }
}
