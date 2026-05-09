using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using YG;

public class YGPlugin : MonoBehaviour
{
    public static YGPlugin main;

    void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        YG2.SyncInitialization();

        if (GetPlayerLevel() == "")
        {
            SetPlayerLevel("2");
        }

        if (GetPlayerCoins() != "")
        {
            GameManager.main.AddCoins(int.Parse(GetPlayerCoins()));
        }
    }

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        YG2.onRewardAdv += OnReward;
    }

    void OnDisable()
    {
        YG2.onRewardAdv -= OnReward;
    }

    /// <summary>
    /// Показать рекламу за вознаграждение
    /// </summary>
    public void ShowRewarded(string rewardID)
    {
        YG2.RewardedAdvShow(rewardID, () =>
        {
            Time.timeScale = 1;
        });
    }

    void OnReward(string resourceName)
    {
        GameManager.main.AddCoins(500);        
        StartCoroutine(TimerTest());
    }

    IEnumerator TimerTest()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1;
    }

    public void SetPlayerLevel(string _level)
    {
        YG2.saves.level = _level;
        YG2.SaveProgress();
    }

    public void SetPlayerCoins(string _coins)
    {
        YG2.saves.coins = _coins;
        YG2.SaveProgress();
    }

    public string GetPlayerCoins()
    {
        return YG2.saves.coins;
    }

    public string GetPlayerLevel()
    {
        return YG2.saves.level;
    }
}

namespace YG
{
    public partial class SavesYG
    {
		public string coins = "";
		public string level = "";
    }
}