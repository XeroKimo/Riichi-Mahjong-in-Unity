using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileDisplay : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    public Tile tile { get; private set; }
    IPlayerActions m_player;
    public Text text;

    public void Awake()
    {
        SetTile(Tile.nullTile);
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
        gameObject.SetActive(this.tile != Tile.nullTile);
        if(tile == Tile.nullTile)
            return;

        switch(tile.suit)
        {
        case Tile.Suit.Bamboo:
            text.text = "B";
            break;
        case Tile.Suit.Circle:
            text.text = "P";
            break;
        case Tile.Suit.Honor:
            text.text = "H";
            break;
        case Tile.Suit.Character:
            text.text = "C";
            break;
        default:
            break;
        }

        switch(tile.face)
        {
        case Tile.Face.One:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "E";
            else
                text.text += "1";
            break;
        case Tile.Face.Two:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "S";
            else
                text.text += "2";
            break;
        case Tile.Face.Three:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "W";
            else
                text.text += "3";
            break;
        case Tile.Face.Four:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "N";
            else
                text.text += "4";
            break;
        case Tile.Face.Five:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "G";
            else
                text.text += "5";
            break;
        case Tile.Face.Six:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "R";
            else
                text.text += "6";
            break;
        case Tile.Face.Seven:
            if(tile.suit == Tile.Suit.Honor)
                text.text += "Wh";
            else
                text.text += "7";
            break;
        case Tile.Face.Eight:
            text.text += "8";
            break;
        case Tile.Face.Nine:
            text.text += "9";
            break;
        default:
            break;
        }
    }

    public void SetOwningPlayer(IPlayerActions player)
    {
        m_player = player;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            RemoveTileFromHand();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        }
    }

    void RemoveTileFromHand()
    {
        if(m_player == null)
            return;

        m_player.RemoveTile(tile);
    }

    public void OnSelect(BaseEventData eventData)
    {

    }
}
