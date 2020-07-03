using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    const int wallHeight = 2;
    protected int m_wallLength;

    List<Tile> m_tiles;
    int m_lastTileIndex;
    int m_lastPlayableTileIndex;
    int m_currentTileIndex;

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
        int startingWall = normalizedDealerIndex * m_wallLength * wallHeight;
        int startingTile = (startingWall + (playerTraverse * m_wallLength * wallHeight) + wallTraverse) % m_tiles.Count;

        m_currentTileIndex = startingTile;
        m_lastPlayableTileIndex = startingTile - (7 * wallHeight);
        m_lastTileIndex = startingTile - (1 * wallHeight);

        if(m_lastPlayableTileIndex < 0)
            m_lastPlayableTileIndex += m_tiles.Count;
        if(m_lastTileIndex < 0)
            m_lastTileIndex += m_tiles.Count;
    }

    public Tile DrawTile()
    {
        Tile output;
        if(m_currentTileIndex == m_lastPlayableTileIndex)
            return Tile.nullTile;

        output = m_tiles[m_currentTileIndex++];
        m_currentTileIndex = m_currentTileIndex % m_tiles.Count;
        //Debug.Log(output.suit + " " + output.face);

        return output;
    }
    public Tile DrawDeadTile()
    {
        Tile output;
        if(m_lastTileIndex == m_currentTileIndex)
            return Tile.nullTile;

        output = m_tiles[m_lastTileIndex--];
        m_lastPlayableTileIndex--;

        if(m_lastTileIndex < 0)
            m_lastTileIndex += m_tiles.Count;
        if(m_lastPlayableTileIndex < 0)
            m_lastPlayableTileIndex += m_tiles.Count;

        return output;
    }
    public List<Tile> DrawMultipleTiles(int count)
    {
        List<Tile> amountToDraw = new List<Tile>();
        for(int i = 0; i < count; i++)
            amountToDraw.Add(DrawTile());
        return amountToDraw;
    }

    public bool IsEmpty()
    {
        return m_currentTileIndex == m_lastPlayableTileIndex;
    }
    public int GetRemainingTileCount()
    {
        int count = m_lastPlayableTileIndex - m_currentTileIndex;
        return (count < 0) ? count + m_tiles.Count : count;
    }
    public List<Tile> GetDeadWall()
    {
        List<Tile> tiles = new List<Tile>();
        int deadWallCount = m_lastTileIndex - m_lastPlayableTileIndex;

        if(deadWallCount < 0)
            deadWallCount += m_tiles.Count;

        for(int i = 0; i < deadWallCount; i++)
        {
            tiles.Add(m_tiles[(m_lastPlayableTileIndex + i) % m_tiles.Count]);
        }

        return tiles;
    }

    private void BuildDeck()
    {
        for(int j = 1; j < 4; j++)
        {
            for(int i = 0; i < (int)Tile.numberCount; i++)
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

    public Deck()
    {
        m_tiles = new List<Tile>();
        m_wallLength = 17;
        BuildDeck();
    }
}
