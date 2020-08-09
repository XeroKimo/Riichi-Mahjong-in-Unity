using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MahjongController : MonoBehaviour, IPlayerEventCallbacks
{
    IPlayerActions player;
    PlayerDisplayManager playerDisplay;
    virtual public void OnHandCallAvailable(HandCall callFlags, List<Meld> cachedChi)
    {
    }

    virtual public void OnHandReset(Hand hand)
    {
        playerDisplay.RefreshHand(hand, player);
    }

    public void OnMeldCreated(Meld meld)
    {
    }

    public void OnPlayerCreated(IPlayerActions player)
    {
        this.player = player;
    }

    virtual public void OnTileAdded(Tile tile)
    {
        playerDisplay.AddTileToHand(tile, player);
    }

    virtual public void OnTileRemoved(Tile tile)
    {
        playerDisplay.RemoveTileFromHand(tile, player);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
