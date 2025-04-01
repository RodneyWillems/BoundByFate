using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    #region Variables

    [HideInInspector]
    public static PlayerInfo Instance;

    private Statistics m_statsSol;
    private int m_hpSol;
    private int m_dpSol;

    private Statistics m_statsShawn;
    private int m_hpShawn;
    private int m_dpShawn;

    #endregion

    #region Setup

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // At the start both characters get their default stats
        m_statsSol = new()
        {
            hp = 15,
            dp = 13,
            attack = 5,
            level = 1
        };

        m_statsShawn = new()
        {
            hp = 10,
            dp = 13,
            attack = 7,
            level = 1
        };
    }

    #endregion

    #region Item usage

    public bool HPHeal(int hp)
    {
        // Heals both HP when either is possible
        if (m_hpSol < m_statsSol.hp || m_hpShawn < m_statsShawn.hp)
        {
            m_hpSol += hp;
            m_hpShawn += hp;

            if (m_hpSol > m_statsSol.hp)
            {
                m_hpSol = m_statsSol.hp;
            }

            if (m_hpShawn > m_statsShawn.hp)
            {
                m_hpShawn = m_statsShawn.hp;
            }
            return true;
        }
        return false;
    }

    public bool BPHeal(int dp)
    {
        // Heals both DP when either is possible
        if (m_dpSol < m_statsSol.dp || m_dpShawn < m_statsShawn.dp)
        {
            m_dpSol += dp;
            m_dpShawn += dp;

            if (m_dpSol > m_statsSol.dp)
            {
                m_dpSol = m_statsSol.dp;
            }

            if (m_dpShawn > m_statsShawn.dp)
            {
                m_dpShawn = m_statsShawn.dp;
            }
            return true;
        }
        return false;
    }

    #endregion

    public int GetStat(string name, string stat, bool current = false)
    {
        // Returns requested stat
        if (name == "Sol")
        {
            switch (stat)
            {
                case "HP":
                    if (current)
                        return m_hpSol;
                    return m_statsSol.hp;
                case "DP":
                    if (current)
                        return m_dpSol;
                    return m_statsSol.dp;
                case "Attack":
                    return m_statsSol.attack;
                default:
                    break;
            }
        }

        else if (name == "Shawn")
        {
            switch (stat)
            {
                case "HP":
                    if (current)
                        return m_hpShawn;
                    return m_statsShawn.hp;
                case "DP":
                    if (current)
                        return m_dpShawn;
                    return m_statsShawn.dp;
                case "Attack":
                    return m_statsShawn.attack;
                default:
                    break;
            }
        }

        return 0;
    }

}
