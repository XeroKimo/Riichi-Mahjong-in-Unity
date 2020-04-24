using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    const int wallHeight = 2;
    protected int m_wallLength;

    [SerializeField]
    List<Tile> m_tiles;
    [SerializeField]
    int m_lastTile;
    [SerializeField]
    int m_lastPlayableTile;
    [SerializeField]
    protected int m_currentTile;

    public Deck()
    {
        m_tiles = new List<Tile>();
        m_wallLength = 17;
        BuildDeck();
    }

    public List<Tile> GetTiles()
    {
        return new List<Tile>(m_tiles);
    }

    public Tile DrawTile()
    {
        Tile output;
        if (m_currentTile == m_lastPlayableTile)
            return null;

        output = m_tiles[m_currentTile++];
        m_currentTile = m_currentTile % m_tiles.Count;

        return output;
    }

    public Tile DrawDeadTile()
    {
        Tile output;
        if (m_lastTile == m_currentTile)
            return null;

        output = m_tiles[m_lastTile--];
        m_lastPlayableTile--;

        if (m_lastTile < 0)
            m_lastTile += m_tiles.Count;
        if (m_lastPlayableTile < 0)
            m_lastPlayableTile += m_tiles.Count;

        return output;
    }

    public bool Empty()
    {
        return m_currentTile == m_lastPlayableTile;
    }

    public void ShuffleDeck()
    {
        for (int i = m_tiles.Count - 1; i > 0; i--)
        {
            int randomInt = Random.Range(0, m_tiles.Count - 1);

            Tile tempTile = m_tiles[i];
            m_tiles[i] = m_tiles[randomInt];
            m_tiles[randomInt] = tempTile;
        }
    }

    public void BreakDeck(int dealerIndex)
    {
        int normalizedDealerIndex = dealerIndex % 4;

        int D12 = Random.Range(0, 6) + Random.Range(0, 6) + 2;
        int playerTraverse = D12 % 4;
        int wallTraverse = D12;
        int startingWall = normalizedDealerIndex * m_wallLength * wallHeight;
        int startingTile = (startingWall + (playerTraverse * m_wallLength * wallHeight) + wallTraverse) % m_tiles.Count;


        m_currentTile = startingTile;
        m_lastPlayableTile = startingTile - (7 * wallHeight);
        m_lastTile = startingTile - (1 * wallHeight);

        if (m_lastPlayableTile < 0)
            m_lastPlayableTile += m_tiles.Count;
        if (m_lastTile < 0)
            m_lastTile += m_tiles.Count;
    }

    private void BuildDeck()
    {
        for (int i = 0; i < (int)Tile.Face.Count; i++)
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
