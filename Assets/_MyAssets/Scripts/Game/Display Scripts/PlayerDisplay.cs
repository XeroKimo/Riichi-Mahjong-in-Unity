using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum TileOrigin
{
    Self = 0,

}

public class PlayerDisplay : MonoBehaviour//, IHandDisplayCallbacks
{
    public HandDisplayManager handDisplay;
    public DiscardDisplayManager discardDisplay;
    public MeldDisplayManager meldDisplay;

    public void AddTileToHand(Tile tile, IPlayerActions player)
    {
        handDisplay.AddDrawnTile(tile, player);
    }

    public void RemoveTileFromHand(Tile tile)
    {
        handDisplay.RemoveTile(tile);
    }

    public void RemoveTileFromDiscard()
    {
        discardDisplay.RemoveLastDiscardedTile();
    }

    public void CreateMeld(Meld meld, IPlayerActions player)
    {
        meldDisplay.DisplayMeld(meld, player);
    }

    public void ResetDisplay(Hand hand, IPlayerActions player)
    {
        handDisplay.Clear(hand, player);
        meldDisplay.Clear();
        discardDisplay.ClearPile();
    }

    public void SetBounds(PlayerDisplayBounds displayBounds)
    {
        handDisplay.transform.SetParent(displayBounds.handBounds);
        discardDisplay.transform.SetParent(displayBounds.discardBounds);
        meldDisplay.transform.SetParent(displayBounds.meldBounds);

        RectTransform handTransform = handDisplay.transform as RectTransform;
        RectTransform discardTransform = discardDisplay.transform as RectTransform;
        RectTransform meldTransform = meldDisplay.transform as RectTransform;

        handTransform.anchoredPosition = Vector2.zero;
        discardTransform.anchoredPosition = Vector2.zero;
        meldTransform.anchoredPosition = Vector2.zero;

        handTransform.rotation = Quaternion.identity;
        discardTransform.rotation = Quaternion.identity;
        meldTransform.rotation = Quaternion.identity;

        handTransform.localScale = Vector3.one;
    }

    //void Awake()
    //{
    //}

    //public void OnMeldMade(Player player, Meld meld)
    //{
    //    handDisplay.DisplayMeld(meld);
    //}

    //public void OnTileAdded(Player player, Tile tile)
    //{
    //    TileDisplay spawnedTile = Instantiate(tilePrefab);
    //    spawnedTile.SetTile(tile);
    //    spawnedTile.SetOwningPlayer(player);

    //    handDisplay.AddDrawnTile(spawnedTile);
    //}

    //public void OnTileRemoved(Player player, Tile tile)
    //{
    //    TileDisplay discardedTile = handDisplay.RemoveTile(tile);
    //    discardedTile.SetOwningPlayer(null);
    //    discardDisplay.AddTile(discardedTile);
    //}

    //public void RefreshHand(Player player, List<Tile> tiles)
    //{
    //    List<TileDisplay> displayTiles = handDisplay.tiles;

    //    int nullCount = displayTiles.Count - tiles.Count;
    //    for(int i = 0; i < tiles.Count; i++)
    //    {
    //        displayTiles[i].SetOwningPlayer(player);
    //        displayTiles[i].SetTile(tiles[i]);
    //    }
    //    for(int i = tiles.Count - 1; i < nullCount; i++)
    //    {
    //        displayTiles[i].SetTile(Tile.nullTile);
    //    }
    //}

    //public void TransferTileIn(TileDisplay tile)
    //{
    //    handDisplay.AddTile(tile);
    //}

    //public TileDisplay ExtractLastDiscardedTile()
    //{
    //    return discardDisplay.ExtractLastDiscardedTile();
    //}

    //public void CreateHand()
    //{
    //    handDisplay.ClearHand();
    //    discardDisplay.ClearPile();
    //    for(int i = 0; i < 13; i++)
    //    {
    //        handDisplay.AddTile(Instantiate(tilePrefab));
    //    }
    //}


}
