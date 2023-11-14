using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
    private uint CoinsCollected = 0;

    private Dictionary<AchievementID, Achievement> Achievements;

    private enum AchievementID
    {
        CoinCollector
    }

    void Start()
    {
        Coin.OnCoinCollected += CoinCollectedHandler;
    }

    private void InitializeAchievements()
    {
        Achievements = new Dictionary<AchievementID, Achievement>();
        Achievements.Add(AchievementID.CoinCollector, new Achievement() { Condition = () => { return CoinsCollected == 5; } });
    }

    private void CoinCollectedHandler()
    {
        CoinsCollected++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct Achievement
{
    public Func<bool> Condition;
    public bool IsCompleted;
}