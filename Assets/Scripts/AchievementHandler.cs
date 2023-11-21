using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
    private uint CoinsCollected = 0;
    private uint EnemiesKilled = 0;

    private Dictionary<AchievementID, Achievement> Achievements;

    private uint AchievementNotifFlags = 0;
    
    [Flags]
    private enum AchievementID : uint
    {
        None = 0,
        CoinCollector = 1 << 0,
        Terminator = 1 << 1
    }

    void Start()
    {
        InitializeAchievements();
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected += CoinCollectedHandler;
        Enemy.OnEnemyKilled += EnemyKilledHandler;
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected -= CoinCollectedHandler;
        Enemy.OnEnemyKilled -= EnemyKilledHandler;
    }

    private void InitializeAchievements()
    {
        Achievements = new Dictionary<AchievementID, Achievement>
        {
            { AchievementID.CoinCollector, new Achievement { Condition = () => CoinsCollected == 5 } },
            { AchievementID.Terminator, new Achievement { Condition = () => EnemiesKilled == 10 } }
        };
    }

    private void EnemyKilledHandler()
    {
        EnemiesKilled++;
        if (Achievements[AchievementID.Terminator].IsCompleted &&
            (AchievementNotifFlags & (uint)AchievementID.Terminator) == 0)
        {
            Debug.Log("Terminator Unlocked!");
            AchievementNotifFlags |= (uint)AchievementID.Terminator;
        }
    }

    private void CoinCollectedHandler()
    {
        CoinsCollected++;
        if (Achievements[AchievementID.CoinCollector].IsCompleted &&
            (AchievementNotifFlags & (uint)AchievementID.CoinCollector) == 0)
        {
            Debug.Log("Coin Collector Unlocked!");
            AchievementNotifFlags |= (uint)AchievementID.CoinCollector;
        }
    }
}

public struct Achievement
{
    public Func<bool> Condition;
    public bool IsCompleted => Condition();
}