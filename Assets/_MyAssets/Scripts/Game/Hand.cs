using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand
{
    [SerializeField]
    List<Tile> m_tiles;
    List<Meld> m_lockedMelds;

    public Hand()
    {
        m_tiles = new List<Tile>(14);
        m_lockedMelds = new List<Meld>(5);
    }

    public Hand(Hand other)
    {
        m_tiles = new List<Tile>(other.m_tiles);
        m_lockedMelds = new List<Meld>(other.m_lockedMelds);
    }


    public void ResetHand(List<Tile> initialTiles)
    {
        m_tiles.Clear();
        m_tiles.AddRange(initialTiles);
    }

    public List<Tile> GetTiles() { return m_tiles; }
    public List<Meld> GetMelds() { return m_lockedMelds; }
    public bool IsOpenHand() 
    {
        foreach (var meld in m_lockedMelds)
        {
            if (meld.open)
                return true;
        }
        return false;
    }

    public void AddTileToHand(Tile tile)
    {
        m_tiles.Add(tile);
    }

    public Tile RemoveTileFromHand(int index)
    {
        int lastIndex = m_tiles.Count - 1;
        int indexToRemove = lastIndex - index;
        Tile outputTile = m_tiles[indexToRemove];
        m_tiles.RemoveAt(indexToRemove);
        return outputTile;
    }

    public bool RemoveTileFromHand(Tile tile)
    {
        return m_tiles.Remove(tile);
    }

    public void SwapTiles(int first, int second)
    {
        Tile swappedTile = m_tiles[first];
        m_tiles[first] = m_tiles[second];
        m_tiles[second] = swappedTile;
    }

    public void SortHand(Dictionary<Tile.Suit, int> sortArrangement)
    {
        m_tiles = TileHelpers.SortTiles(m_tiles, sortArrangement);
    }

    public bool CanMakeLateQuad(out Meld outMeld)
    {
        outMeld = null;
        Tile drawnTile = m_tiles[m_tiles.Count - 1];
        foreach (var meld in m_lockedMelds)
        {
            if (meld.Contains(drawnTile))
            {
                outMeld = meld;
                return true;
            }
        }

        return false;
    }

    public void MakeLateQuad(Meld meld)
    {
        m_lockedMelds.Remove(meld);
        Tile drawnTile = m_tiles[m_tiles.Count - 1];

        List<Tile> quad = new List<Tile>(meld.tiles);
        quad.Add(drawnTile);
        m_lockedMelds.Add(new Meld(quad, true));
    }

    public bool CanMakeClosedQuad(out Tile[] tiles)
    {
        tiles = null;
        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(m_tiles);
        Tile drawnTile = m_tiles[m_tiles.Count - 1];

        if (tileMap[drawnTile].Count == 4)
        {
            tiles = tileMap[drawnTile].ToArray();
            return true;
        }


        return false;
    }

    public bool CanMakeTriple(Tile input, out Tile[] tiles)
    {
        tiles = null;
        Dictionary<Tile, List<Tile>> tileMap = TileHelpers.ArrangeTilesByTile(m_tiles);
        Tile drawnTile = m_tiles[m_tiles.Count - 1];

        if (tileMap[drawnTile].Count == 3)
        {
            tiles = tileMap[drawnTile].ToArray();
            return true;
        }

        return false;
    }

    public bool CanMakeSequence(Tile input, out Tile[] tiles)
    {
        tiles = null;
        if (TileHelpers.IsHonor(input))
            return false;

        Dictionary<int, List<Tile>> tileValue = null;

        {
            List<Tile> suit = TileHelpers.GetTilesOfSuit(m_tiles, input.suit);

            if (suit.Count < 2)
                return false;

            tileValue = TileHelpers.ArrangeTilesByValue(suit);

            if (!tileValue.ContainsKey(input.rawValue))
                tileValue[input.rawValue] = new List<Tile>();

            tileValue[input.rawValue].Add(input);
        }

        tiles = TileHelpers.CreateSequenceLowestValue(tileValue, input);

        return tiles != null;
    }

    public void MakeMeld(Tile[] tiles, bool open)
    {
        foreach (var tile in tiles)
        {
            if (m_tiles.Contains(tile))
                m_tiles.Remove(tile);
        }

        m_lockedMelds.Add(new Meld(tiles, open));
    }
}
