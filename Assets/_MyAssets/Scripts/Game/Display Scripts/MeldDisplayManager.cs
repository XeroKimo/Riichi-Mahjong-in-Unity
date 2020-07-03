using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeldDisplayManager : MonoBehaviour
{
    public TileDisplayPool tileDisplayPool;
    public MeldDisplayPool meldDisplayPool;
    List<MeldDisplay> m_melds;

    private void Awake()
    {
        m_melds = new List<MeldDisplay>();
    }

    public void DisplayMeld(Meld meld, IPlayerActions player)
    {
        MeldDisplay display = meldDisplayPool.GetMeldFromPool();

        List<TileDisplay> tileDisplays = new List<TileDisplay>();
        foreach(Tile tile in meld.tiles)
        {
            TileDisplay tileDisplay = tileDisplayPool.GetTileFromPool();
            tileDisplay.SetTile(tile);
            tileDisplays.Add(tileDisplay);
        }
        display.SetTiles(tileDisplays, meld.input.tile, meld.type);

        float totalOffset = 0;
        foreach(var displayMelds in m_melds)
        {
            totalOffset += displayMelds.totalLength;
        }
        (display.transform as RectTransform).anchoredPosition = new Vector2(-totalOffset, 0);

        m_melds.Add(display);

    }

    public void Clear()
    {
        foreach(var meld in m_melds)
        {
            meld.Clear();
        }
        m_melds.Clear();
    }
}
