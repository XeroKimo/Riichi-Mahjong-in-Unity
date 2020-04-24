using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDeck : MonoBehaviour
{
    public DebugTile tilePrefab;
    private Deck m_deck;

    List<DebugTile> m_tiles;

    private void Awake()
    {
        m_deck = new Deck();
        m_tiles = new List<DebugTile>();
        List<Tile> tiles = m_deck.GetTiles();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                float yValue = 100 * i;
                float xValue = 70 * j;

                m_tiles.Add(Instantiate(tilePrefab, new Vector3(xValue + 35, yValue + 50, 0), Quaternion.identity, transform));
            }
        }
        DisplayTiles();


        List<Tile> testTiles = new List<Tile>();
        testTiles.Add(MakeCharacter(1));
        testTiles.Add(MakeCharacter(1));
        testTiles.Add(MakeCharacter(1));

        testTiles.Add(MakeCharacter(2));
        testTiles.Add(MakeCharacter(3));
        testTiles.Add(MakeCharacter(4));
        testTiles.Add(MakeCharacter(5));

        testTiles.Add(MakeCharacter(6));
        testTiles.Add(MakeCharacter(7));
        testTiles.Add(MakeCharacter(8));
        testTiles.Add(MakeCharacter(9));
        testTiles.Add(MakeCharacter(9));
        testTiles.Add(MakeCharacter(9));

        List<Tile> waitingTiles = TileHelpers.GetWaitingTiles(testTiles);

        //testTiles.Add(MakeHonor(Tile.Face.White));
        //testTiles.Add(MakeHonor(Tile.Face.White));
        //testTiles.Add(MakeHonor(Tile.Face.White));


        //List<Meld> melds = HandHelpers.CheckHandState(testTiles);
        if (waitingTiles.Count >= 0)
        {
            int i = 0;
        }
    }

    Tile MakeBamboo(int value)
    {
        return new Tile(Tile.Suit.Bamboo, (Tile.Face)value - 1);
    }

    Tile MakeCircle(int value)
    {
        return new Tile(Tile.Suit.Circle, (Tile.Face)value - 1);
    }

    Tile MakeCharacter(int value)
    {
        return new Tile(Tile.Suit.Character, (Tile.Face)value - 1);
    }

    Tile MakeHonor(Tile.Face face)
    {
        return new Tile(Tile.Suit.Honor, face);
    }

    void DisplayTiles()
    {
        List<Tile> tiles = m_deck.GetTiles();
        for (int i = 0; i < tiles.Count; i++)
        {
            m_tiles[i].Initialize(tiles[i]);
        }
    }

    public void ShuffleDeck()
    {
        m_deck.ShuffleDeck();
        DisplayTiles();
    }
}
