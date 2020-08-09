using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version 0.3

public class Deck
{
    const int wallHeight = 2;
    const int wallLength = 17;

    List<Tile> m_tiles;
    int m_currentTileIndex;
    int m_lastTileIndex;
    int m_deadWallStartIndex;
    int m_doraWallStartIndex;

    public Deck()
    {
        m_tiles = new List<Tile>();
        BuildDeck();
    }

    public void ShuffleDeck()
    {
        for(int i = m_tiles.Count - 1; i > 0; i--)
        {
            int randomInt = Random.Range(0, m_tiles.Count - 1);

            Tile tempTile = m_tiles[i];
            m_tiles[i] = m_tiles[randomInt];
            m_tiles[randomInt] = tempTile;
        }
    }
    public void BreakDeck(int dealerIndex, int playerCount)
    {
        int normalizedDealerIndex = dealerIndex % playerCount;

        int D12 = Random.Range(0, 6) + Random.Range(0, 6) + 2;
        int playerTraverse = D12 % playerCount;
        int wallTraverse = D12;
        int startingWall = normalizedDealerIndex * wallLength * wallHeight;
        int startingTile = (startingWall + (playerTraverse * wallLength * wallHeight) + wallTraverse) % m_tiles.Count;

        m_currentTileIndex = startingTile;
        m_deadWallStartIndex = startingTile - (7 * wallHeight);
        m_doraWallStartIndex = m_deadWallStartIndex + 2 * wallHeight;
        m_lastTileIndex = m_deadWallStartIndex - 1;

        if(m_deadWallStartIndex < 0)
            m_deadWallStartIndex += m_tiles.Count;
        if(m_doraWallStartIndex < 0)
            m_doraWallStartIndex += m_tiles.Count;
        if(m_lastTileIndex < 0)
            m_lastTileIndex += m_tiles.Count;
    }

    public Tile DrawTile()
    {
        Tile output;
        if(m_currentTileIndex == m_lastTileIndex)
            return Tile.nullTile;

        output = m_tiles[m_currentTileIndex++];
        m_currentTileIndex = m_currentTileIndex % m_tiles.Count;

        return output;
    }
    public Tile DrawDeadTile()
    {
        Tile output;
        if(m_lastTileIndex == m_currentTileIndex)
            return Tile.nullTile;

        output = m_tiles[m_lastTileIndex--];
        m_deadWallStartIndex = (m_deadWallStartIndex + 1) %  m_tiles.Count;

        if(m_lastTileIndex < 0)
            m_lastTileIndex += m_tiles.Count;

        return output;
    }

    public bool IsEmpty()
    {
        return m_currentTileIndex == m_lastTileIndex;
    }
    public int GetRemainingTileCount()
    {
        int count = m_lastTileIndex - m_currentTileIndex;
        return (count < 0) ? count + m_tiles.Count : count;
    }
    public List<Tile> GetDoraTiles()
    {
        List<Tile> tiles = new List<Tile>();
        const int doraWall = 10;

        for(int i = 0; i < doraWall; i++)
        {
            tiles.Add(m_tiles[(m_doraWallStartIndex + i) % m_tiles.Count]);
        }

        return tiles;
    }

    private void BuildDeck()
    {
        for(int j = 1; j < 4; j++)
        {
            for(int i = 0; i < Tile.numberCount; i++)
            {
                SpawnTiles((Tile.Suit)j, i);
            }
        }
        for(int i = 0; i < Tile.honorCount; i++)
        {
            SpawnTiles(Tile.Suit.Honor, i);
        }
    }
    private void SpawnTiles(Tile.Suit suit, int value)
    {
        for (int i = 0; i < 4; i++)
        {
            Tile tile = new Tile(suit, (Tile.Face)value);

            m_tiles.Add(tile);
        }
    }
}
