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
        tileDisplay.transform.SetParent(this.transform);

        RectTransform transform = tileDisplay.transform as RectTransform;
        transform.anchorMin = new Vector2(0, 1);
        transform.anchorMax = new Vector2(0, 1);
        transform.pivot = new Vector2(0, 1);
        transform.localScale = Vector3.one;
        Vector2 size = transform.rect.size;

        float xOffset = size.x * (m_discardedTiles.Count % 6);
        float yOffset = -size.y * (float)(m_discardedTiles.Count / 6);

        transform.anchoredPosition = new Vector2(xOffset, yOffset);

        m_discardedTiles.Push(tileDisplay);
    }

    public TileDisplay PeekLastDiscardedTile()
    {
        return m_discardedTiles.Peek();
    }

    public TileDisplay ExtractLastDiscardedTile()
    {
        return m_discardedTiles.Pop();
    }

    public void ClearPile()
    {
        if(m_discardedTiles == null)
        while(m_discardedTiles.Count > 0)
        {
            Destroy(m_discardedTiles.Pop().gameObject);
        }
    }
}
