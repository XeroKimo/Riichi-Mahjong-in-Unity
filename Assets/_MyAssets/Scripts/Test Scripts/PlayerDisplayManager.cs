using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum TileOrigin
{
    Self = 0,

}

public class PlayerDisplayManager : MonoBehaviour, IHandDisplayCallbacks
{
    public TileDisplay tilePrefab;
    public HandDisplayManager handDisplay;
    public DiscardDisplayManager discardDisplay;

    void Awake()
    {
    }

    public void OnMeldMade(Player player, Meld meld, Tile input)
    {
        TileDisplay inputTile = handDisplay.tiles[handDisplay.tiles.Count - 1];
        inputTile.SetOwningPlayer(player);
    }

    public void OnTileAdded(Player player, Tile tile)
    {
        TileDisplay spawnedTile = Instantiate(tilePrefab);
        spawnedTile.SetTile(tile);
        spawnedTile.SetOwningPlayer(player);

        handDisplay.AddDrawnTile(spawnedTile);
    }

    public void OnTileRemoved(Player player, Tile tile)
    {
        TileDisplay discardedTile = handDisplay.RemoveTile(tile);
        discardedTile.SetOwningPlayer(null);
        discardDisplay.AddTile(discardedTile);
    }

    public void RefreshHand(Player player, List<Tile> tiles)
    {
        List<TileDisplay> displayTiles = handDisplay.tiles;

        int nullCount = displayTiles.Count - tiles.Count;
        for(int i = 0; i < tiles.Count; i++)
        {
            displayTiles[i].SetOwningPlayer(player);
            displayTiles[i].SetTile(tiles[i]);
            if(tiles[i] == null)
                Debug.Break();
        }
        for(int i = tiles.Count - 1; i < nullCount; i++)
        {
            displayTiles[i].SetTile(null);
        }
    }

    public void TransferTileIn(TileDisplay tile)
    {
        handDisplay.AddTile(tile);
    }

    public TileDisplay ExtractLastDiscardedTile()
    {
        return discardDisplay.ExtractLastDiscardedTile();
    }

    public void CreateHand()
    {
        handDisplay.ClearHand();
        discardDisplay.ClearPile();
        for(int i = 0; i < 13; i++)
        {
            handDisplay.AddTile(Instantiate(tilePrefab));
        }

    }
}
