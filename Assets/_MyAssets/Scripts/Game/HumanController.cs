﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : PlayerController
{
    protected override void OnHandCallEnabled(Player.HandCall handCalls)
    {
        //If chi hand call enabled, and there are multiple chi
        //configure button to open up the options to call chi on
    }

    protected override void OnHandCallRequested(Player.HandCall handCall)
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

}