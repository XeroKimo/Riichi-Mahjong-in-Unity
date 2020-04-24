using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayManager : MonoBehaviour, IHandDisplayCallbacks
{
    public TileDisplay tilePrefab;
    public HandDisplayManager handDisplay;
    public DiscardDisplayManager discardDisplay;

    public void OnHandCallMade(Player player, List<Tile> tile, HandCalls handCall)
    {

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

    public void RefreshHand(Player player, List<Tile> tile)
    {
        List<TileDisplay> tiles = handDisplay.tiles;

        int nullCount = tiles.Count - tile.Count;
        for(int i = 0; i < tile.Count; i++)
        {
            tiles[i].SetOwningPlayer(player);
            tiles[i].SetTile(tile[i]);
        }
        for(int i = tile.Count - 1; i < nullCount; i++)
        {
            tiles[i].SetTile(null);
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
}
