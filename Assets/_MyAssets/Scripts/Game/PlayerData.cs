using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandCalls
{
    public enum Flag
    {
        None = 0,
        Chi = 1,
        Pon = 2,
        Kan = 4,
        Ron = 8,
        Tsumo = 16
    }

    int m_flags;

    public static HandCalls Chi { get => new HandCalls(Flag.Chi); }
    public static HandCalls Pon { get => new HandCalls(Flag.Pon); }
    public static HandCalls Kan { get => new HandCalls(Flag.Kan); }
    public static HandCalls Ron { get => new HandCalls(Flag.Ron); }
    public static HandCalls Tsumo { get => new HandCalls(Flag.Tsumo); }

    public HandCalls()
    {
        m_flags = 0;
    }

    public HandCalls(HandCalls other)
    {
        m_flags = other.m_flags;
    }

    public HandCalls(Flag flag)
    {
        m_flags = (int)flag;
    }

    public void SetFlag(Flag flag)
    {
        m_flags = m_flags | (int)flag;
    }

    public bool IsSet(Flag flag)
    {
        return (m_flags & (int)flag) == (int)flag;
    }

    public void ResetFlag(Flag flag)
    {
        if(IsSet(flag))
            m_flags = m_flags ^ (int)flag;
    }
}

public class PlayerData
{
    public Hand hand;
    public int points;

    public PlayerData()
    {
        hand = new Hand();
        points = 0;
    }
}
