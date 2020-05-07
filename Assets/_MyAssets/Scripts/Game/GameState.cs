using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MahjongFlags
{
    public enum Flag
    {
        HP_Riichi = 0x0001,
        HP_DoubleRiichi = 0x0002,
        HP_LateKan = 0x0004,
        HP_Ippatsu = 0x0008,
        HP_RinshanKaihou = 0x0010,
        HP_NagashiMangan = 0x0020,
        HP_KokushiMusou = 0x0040,
        HP_Ryanpeikou = 0x0080,

        FU_SingleTileWait = 0x0100,

        DP_FirstTile = 0x0200,
        DP_LastTile = 0x0400,
    }

    private int m_flag;

    public MahjongFlags()
    {
        m_flag = 0;
    }

    public MahjongFlags(MahjongFlags other)
    {
        m_flag = other.m_flag;
    }

    public void SetFlag(Flag flag)
    {
        m_flag = m_flag | (int)flag;
    }

    public void ResetFlag(Flag flag)
    {
        if (IsFlagSet(flag))
            m_flag = m_flag ^ (int)flag;
    }

    public bool IsFlagSet(Flag flag)
    {
        return ((m_flag & (int)flag) == (int)flag);
    }

    public void SetFlag(MahjongFlags other)
    {
        m_flag = m_flag & other.m_flag;
    }
}

public class GameState : MonoBehaviour
{
    private List<PlayerData> m_players = null;

    int m_currentPlayerTurn;

    //[SerializeField]
    Deck m_deck;
    //DebugDeck m_deck;
    DiscardPile m_discardPile;
    public Tile tilePrefab;

    PlayerData m_dealer = null;

    Hand hand;
    private void Awake()
    {
        m_deck = new Deck();
        m_deck.ShuffleDeck();
        m_deck.BreakDeck(Random.Range(0, 4));
        m_discardPile = new DiscardPile();

        hand = new Hand();

        Debug.Log(HandPatterns.CalculateHandValue(25, 4, false, HandCall.Tsumo));
    }


    // Start is called before the first frame update
    void Start()
    {
        //while(
        //SetUpDeck();
        //DrawPlayerHands();
        //while(!deck.Empty())
        //{
        //currentPlayer draw tile
        //currentPlayer discard tile
        //for(int i = 0; i < player.count; i++)
        //previousPlayer = players[i - 1];
        //check for hand calls

        //if hand call available
        //wait for input

        //currentPlayer++
        //}


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Shuffle()
    {
        m_deck.ShuffleDeck();
        m_deck.BreakDeck(m_players.FindIndex((PlayerData other) => { return m_dealer == other; }));
    }
    void EndGame()
    {
    }
}
