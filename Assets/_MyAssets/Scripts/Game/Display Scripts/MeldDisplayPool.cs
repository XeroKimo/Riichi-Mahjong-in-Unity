using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeldDisplayPool : MonoBehaviour
{
    public int meldCount = 20;

    public MeldDisplay tilePrefab;
    private Stack<MeldDisplay> m_tilePool;

    private void Awake()
    {
        m_tilePool = new Stack<MeldDisplay>();
        for(int i = 0; i < meldCount; i++)
        {
            m_tilePool.Push(Instantiate(tilePrefab, transform));
            m_tilePool.Peek().displayPool = this;
        }
    }

    public MeldDisplay GetMeldFromPool()
    {
        return m_tilePool.Pop();
    }

    public void ReturnMeldToPool(MeldDisplay tile)
    {
        m_tilePool.Push(tile);
    }
}
