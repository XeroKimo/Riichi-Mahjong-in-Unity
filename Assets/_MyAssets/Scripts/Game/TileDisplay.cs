using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileDisplay : MonoBehaviour, IPointerClickHandler
{
    public Tile tile { get; private set; }
    Player m_player;

    public void Awake()
    {
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
        gameObject.SetActive(this.tile != null);
    }

    public void SetOwningPlayer(Player player)
    {
        m_player = player;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(EventSystem.current.currentSelectedGameObject == this)
        {
            RemoveTileFromHand();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    void RemoveTileFromHand()
    {
        if(!m_player)
            return;

        m_player.RemoveTileFromHand(tile);
    }
}
