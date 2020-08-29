using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Version: 0.35
public class GameState : MonoBehaviour
{
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

    public void OnPlayerDiscarded(DiscardedTile discardedTile)
    {
        foreach(PlayerController player in m_players)
        {
            if(player.playerID == m_currentPlayerID)
                continue;

            //Check chi if the player's ID is +1 the currentPlayerID
            if(player.EnableHandCalls(discardedTile, (m_currentPlayerID + 1) % 4 == player.playerID))
            {
                m_handCallRequestCount++;
            }
        }

        if(m_handCallRequestCount == 0)
        {
            if(m_deck.IsEmpty())
            {
                //Check if deck is empty
                //If empty and current dealer is in tenpai,
                //count current dealer as the winner
            }
            else
            {
                StartNextTurn();
            }
        }
    }

    public void RequestHandCall(PlayerController player, Player.HandCall handCall, HandCallFunction function)
    {
        //If the current player requests the hand call, just call the function
        if(player.playerID == m_currentPlayerID)
        {
            Debug.Assert(handCall == Player.HandCall.Chi || handCall == Player.HandCall.Pon || handCall == Player.HandCall.Ron, "Player requested an invalid hand call");
            function();
            return;
        }

        //Check to see if this player has requested a hand call before
        HandCallRequest findRequest = m_handCallRequests.Find((HandCallRequest compare) => { return compare.player == player; });
        if(findRequest.function != null)
            return;

        m_handCallRequests.Add(new HandCallRequest(player, handCall, function));
        m_handCallRequestCount--;

        if(m_handCallRequestCount == 0)
        {
            HandCallRequest requestToCall = m_handCallRequests[0];
            Player.HandCall highestPriority = Player.HandCall.None;

            for(int i = 1; i < m_handCallRequests.Count; i++)
            {
                if(m_handCallRequests[i].handCall > highestPriority)
                {
                    highestPriority = m_handCallRequests[i].handCall;
                    requestToCall = m_handCallRequests[i];
                }
            }

            requestToCall.function();
            m_handCallRequests.Clear();
        }
    }

    public bool IsMyTurn(byte playerID)
    {
        return m_currentPlayerID == playerID;
    }

    private void StartNextTurn()
    {
        m_currentPlayerID = (byte)((m_currentPlayerID + 1) % 4);
        m_players[m_currentDealerID].AddTile(m_deck.DrawTile());
    }

    private void StartNextRound(int winningPlayerID)
    {
        if(ShouldRotateDealers(winningPlayerID))
        {
            m_currentDealerID = (byte)((m_currentDealerID + 1) % 4);
        }
    }

    private bool ShouldRotateDealers(int winningPlayerID)
    {
        return winningPlayerID != m_currentDealerID;
    }

    struct HandCallRequest
    {
        public PlayerController player;
        public Player.HandCall handCall;
        public HandCallFunction function;

        public HandCallRequest(PlayerController player, Player.HandCall handCall, HandCallFunction function)
        {
            this.player = player;
            this.handCall = handCall;
            this.function = function;
        }
    };
}
