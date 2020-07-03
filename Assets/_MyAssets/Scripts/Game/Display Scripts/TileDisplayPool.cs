using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDisplayPool : MonoBehaviour
{
    public int tileCount = 144;

    public TileDisplay tilePrefab;
    private Stack<TileDisplay> m_tilePool;

    private void Awake()
    {
        m_tilePool = new Stack<TileDisplay>();
        for(int i = 0; i < tileCount; i++)
        {
            m_tilePool.Push(Instantiate(tilePrefab, transform));
            m_tilePool.Peek().displayPool = this;
        }
    }

    public TileDisplay GetTileFromPool()
    {
        return m_tilePool.Pop();
    }

    public void ReturnTileToPool(TileDisplay tile)
    {
        tile.SetOwningPlayer(null);
        m_tilePool.Push(tile);
    }
}
