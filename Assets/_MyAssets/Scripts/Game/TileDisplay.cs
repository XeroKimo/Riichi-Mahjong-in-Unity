using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileDisplay : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    public Tile tile { get; private set; }
    Player m_player;
    public Text text;

    public void Awake()
    {
        SetTile(Tile.EmptyTile);
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
        gameObject.SetActive(this.tile != Tile.EmptyTile);
        if(tile == Tile.EmptyTile)
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
            text.text += "1";
            break;
        case Tile.Face.Two:
            text.text += "2";
            break;
        case Tile.Face.Three:
            text.text += "3";
            break;
        case Tile.Face.Four:
            text.text += "4";
            break;
        case Tile.Face.Five:
            text.text += "5";
            break;
        case Tile.Face.Six:
            text.text += "6";
            break;
        case Tile.Face.Seven:
            text.text += "7";
            break;
        case Tile.Face.Eight:
            text.text += "8";
            break;
        case Tile.Face.Nine:
            text.text += "9";
            break;
        case Tile.Face.East:
            text.text += "E";
            break;
        case Tile.Face.South:
            text.text += "S";
            break;
        case Tile.Face.West:
            text.text += "W";
            break;
        case Tile.Face.North:
            text.text += "N";
            break;
        case Tile.Face.Red:
            text.text += "R";
            break;
        case Tile.Face.White:
            text.text += "W";
            break;
        case Tile.Face.Green:
            text.text += "G";
            break;
        default:
            break;
        }
    }

    public void SetOwningPlayer(Player player)
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
        if(!m_player)
            return;

        m_player.RemoveTileFromHand(tile);
    }

    public void OnSelect(BaseEventData eventData)
    {

    }
}
