using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayManager : MonoBehaviour
{
    IPlayerActions m_localPlayer;
    [SerializeField]
    List<PlayerDisplay> m_playerDisplays;
    [SerializeField]
    List<PlayerDisplayBounds> m_playerDisplayBounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLocalPlayer(IPlayerActions localPlayer)
    {
        m_localPlayer = localPlayer;
        for(int i = 0; i < 4; i++)
        {
            m_playerDisplays[i + m_localPlayer.GetPlayerID() % 4].SetBounds(m_playerDisplayBounds[i]);
        }
    }

    public void AddTileToHand(Tile tile, IPlayerActions playerActions)
    {
        m_playerDisplays[playerActions.GetPlayerID()].AddTileToHand(tile, playerActions);
    }
    public void CreateMeld(Meld meld, IPlayerActions playerActions)
    {
        m_playerDisplays[playerActions.GetPlayerID()].CreateMeld(meld, playerActions);
    }
    public void RemoveTileFromHand(Tile tile, IPlayerActions playerActions)
    {
        m_playerDisplays[playerActions.GetPlayerID()].RemoveTileFromHand(tile);
    }
    public void RemoveTileFromDiscard(IPlayerActions playerActions)
    {
        m_playerDisplays[playerActions.GetPlayerID()].RemoveTileFromDiscard();
    }
    public void RefreshHand(Hand hand, IPlayerActions playerActions)
    {
        m_playerDisplays[playerActions.GetPlayerID()].ResetDisplay(hand, playerActions);
    }
}
