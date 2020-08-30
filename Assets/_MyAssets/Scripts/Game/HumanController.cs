using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.41

public class HumanController : PlayerController
{
    protected override void OnHandCallEnabled(HandCall handCalls)
    {
        //If chi hand call enabled, and there are multiple chi
        //configure button to open up the options to call chi on
    }

    protected override void OnHandCallRequested(HandCall handCall)
    {
        //Disable UI buttons
    }

    protected override void OnHandReset(List<Tile> tiles)
    {
        //Do nothing
    }

    protected override void OnTileAdded(Tile tile)
    {
        //Check for hand calls
        //If any hand calls are enabled, tell UI to enable buttons
    }

    protected override void OnTileDiscarded(DiscardedTile discardedTile)
    {
        //Do nothing
    }

    protected override void OnMeldCreated(Meld meld)
    {
        //Do nothing
    }

    public override void OnCanStealLateKan(DiscardedTile tile)
    {
        //Enable ron button
    }
}
