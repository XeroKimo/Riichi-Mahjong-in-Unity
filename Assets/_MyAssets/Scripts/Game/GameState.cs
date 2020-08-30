using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.41
public class GameState : MonoBehaviour
{
    private enum WaitingState
    {
        Discard,    //Waiting for someone to discard
        Call        //Waiting for someone to call
    }

    public delegate void HandCallFunction();

    List<HandCallRequest> m_handCallRequests = new List<HandCallRequest>();
    //Count of people who has made requests
    int m_handCallRequestCount;

    List<PlayerController> m_players = new List<PlayerController>();
    byte m_currentPlayerID;
    byte m_currentDealerID;
    byte m_initialDealerID;
    Tile.Face m_currentRoundWind;
    Deck m_deck;

    Tile m_currentlyDrawnTile;
    HandCall m_lastHandCall;
    WaitingState m_waitingState;

    public void OnPlayerDiscarded(DiscardedTile discardedTile)
    {
        int handCallRequestCount = 0;
        foreach(PlayerController player in m_players)
        {
            if(player.playerID == m_currentPlayerID)
                continue;

            //Check chi if the player's ID is +1 the currentPlayerID
            if(player.EnableHandCalls(discardedTile, (m_currentPlayerID + 1) % 4 == player.playerID))
            {
                handCallRequestCount++;
            }
        }


        if(handCallRequestCount == 0)
        {
            if(m_deck.IsEmpty())
            {
                //TODO: 
                //Check if deck is empty
                //If empty and current dealer is in tenpai,
                //count current dealer as the winner
            }
            else
            {
                StartNextTurn();
            }
        }
        else
        {
            m_waitingState = WaitingState.Call;
            m_handCallRequestCount = handCallRequestCount;
        }
    }

    public void RequestHandCall(PlayerController player, HandCall handCall, HandCallFunction function)
    {
        //If the current player requests the hand call, just call the function
        if(player.playerID == m_currentPlayerID)
        {
            Debug.Assert(handCall == HandCall.Chi || handCall == HandCall.Pon || handCall == HandCall.Ron, "Player requested an invalid hand call");

            HandleHandCall(new HandCallRequest(player, handCall, function));
        }
        else
        {
            //Check to see if this player has requested a hand call before
            HandCallRequest findRequest = m_handCallRequests.Find((HandCallRequest compare) => { return compare.player == player; });
            if(findRequest.player == player)
                return;

            m_handCallRequests.Add(new HandCallRequest(player, handCall, function));
            m_handCallRequestCount--;

            if(m_handCallRequestCount == 0)
            {
                HandCallRequest requestToCall = m_handCallRequests[0];
                HandCall highestPriority = HandCall.None;

                for(int i = 1; i < m_handCallRequests.Count; i++)
                {
                    if(m_handCallRequests[i].handCall > highestPriority)
                    {
                        highestPriority = m_handCallRequests[i].handCall;
                        requestToCall = m_handCallRequests[i];
                    }
                }

                m_handCallRequests.Clear();

                HandleHandCall(requestToCall);
            }
        }
    }

    public bool CanPlayerDiscard(byte playerID)
    {
        return m_currentPlayerID == playerID && m_waitingState == WaitingState.Discard;
    }

    private void HandleHandCall(HandCallRequest handCall)
    {
        m_waitingState = WaitingState.Discard;
        HandCall previousHandCall = m_lastHandCall;
        m_lastHandCall = handCall.handCall;

        switch(handCall.handCall)  
        {
        case HandCall.None:
            if(previousHandCall == HandCall.LateKan)           //If no players want to steal the late kan
            {
                handCall.player.NotifyMeldCreated();                                  //Call meld created
                m_currentlyDrawnTile = m_deck.DrawDeadTile();                   //Set the currently drawn tile to a dead tile
                m_players[m_currentPlayerID].AddTile(m_currentlyDrawnTile);     //Add it to the callee
            }
            break;
        case HandCall.Chi:
        case HandCall.Pon:
            m_currentPlayerID = handCall.player.playerID;   //Set current player to the callerID
            handCall.function();
            handCall.player.NotifyMeldCreated();
            break;
        case HandCall.Kan:
            m_currentPlayerID = handCall.player.playerID;   //Set current player to the callerID
            handCall.function();                            //Call the hand call function

            handCall.player.NotifyMeldCreated();                  //Call meld created
            m_currentlyDrawnTile = m_deck.DrawDeadTile();   //Set the currently drawn tile to a dead tile
            handCall.player.AddTile(m_currentlyDrawnTile);  //Add it to the callee
            break;
        case HandCall.LateKan:
            handCall.function();//Call hand call function immediately

            DiscardedTile tileToSteal = new DiscardedTile(m_currentlyDrawnTile, handCall.player.playerID);
            List<PlayerController> canStealKan = new List<PlayerController>();
            foreach(PlayerController controller in m_players)   //Check to see if players can steal the tile
            {
                if(controller == handCall.player)
                    continue;

                if(controller.CanStealLateKan(tileToSteal))
                    canStealKan.Add(controller);
            }
            if(canStealKan.Count > 0)
            {
                m_waitingState = WaitingState.Call;                    //If any player can steal the tile, call on steal late kan
                m_handCallRequestCount = canStealKan.Count;            //to notify them that they can request to steal
                foreach(PlayerController controller in canStealKan)
                {
                    controller.OnCanStealLateKan(tileToSteal);
                }
            }
            else
            {
                handCall.player.NotifyMeldCreated();                  //Call meld created
                m_currentlyDrawnTile = m_deck.DrawDeadTile();   //If no players can steal, set the currently drawn tile to a dead tile
                handCall.player.AddTile(m_currentlyDrawnTile);  //Add it to the callee
            }
            break;

        case HandCall.Ron:
            //Calculate player hand
            break;
        case HandCall.Tsumo:
            //Calculate player hand
            break;
        default:
            break;
        }
    }

    private void StartNextTurn()
    {
        m_currentPlayerID = (byte)((m_currentPlayerID + 1) % 4);
        //Store the drawn tile for late kan check
        m_currentlyDrawnTile = m_deck.DrawTile();
        m_waitingState = WaitingState.Discard;

        foreach(PlayerController player in m_players)
        {
            player.PreTurnStart();
        }

        m_players[m_currentDealerID].AddTile(m_currentlyDrawnTile);
    }

    private void StartNextRound(int winningPlayerID)
    {
        if(ShouldRotateDealers(winningPlayerID))
        {
            m_currentDealerID = (byte)((m_currentDealerID + 1) % 4);
            if(m_currentDealerID == m_initialDealerID)
            {
                m_currentRoundWind = m_currentRoundWind + 1;
            }
        }

        //Resets the deck
        m_deck.ShuffleDeck();       
        m_deck.BreakDeck(m_currentDealerID, m_players.Count);

        ResetHands();

        //Make the current player's turn -1 the dealer's ID so when next turn is called, it is the dealer's turn
        m_currentPlayerID = (m_currentDealerID == 0) ? (byte)3 : (byte)(m_currentDealerID - 1); 

        StartNextTurn();
    }

    private bool ShouldRotateDealers(int winningPlayerID)
    {
        return winningPlayerID != m_currentDealerID;
    }

    private void ResetHands()
    {
        List<Tile>[] hands = new List<Tile>[4];

        const int tileBunch = 4;
        const int tileBunchCount = 3;


        //Each player takes turns drawing 4 tiles 3 times
        for(int i = 0; i < tileBunchCount; i++)
        {
            for(int j = 0; j < hands.Length; j++)       
            {
                for(int k = 0; k < tileBunch; k++)
                {
                    hands[j].Add(m_deck.DrawTile());
                }
            }
        }
        
        //Each player takes turns drawing their 13th tile
        for(int i = 0; i < tileBunchCount; i++)
        {
            for(int j = 0; j < hands.Length; j++)       
            {
                hands[j].Add(m_deck.DrawTile());
            }
        }


        //Reset each player's hand
        for(int i = 0; i < hands.Length; i++)
        {
            m_players[(m_currentDealerID + i) % hands.Length].ResetHand(hands[i]);
        }
    }


    struct HandCallRequest
    {
        public PlayerController player;
        public HandCall handCall;
        public HandCallFunction function;

        public HandCallRequest(PlayerController player, HandCall handCall, HandCallFunction function)
        {
            this.player = player;
            this.handCall = handCall;
            this.function = function;
        }
    };
}
