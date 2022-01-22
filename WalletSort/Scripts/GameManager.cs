using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;
using GameAnalyticsSDK;
using UnityEngine.Networking;
using DG.Tweening;
using LionStudios.Suite.Analytics;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    public Action OnWin, OnLose;

    [HideInInspector]
    public GameStatus gameStatus;


    public int loopLastLevels;


    private void Awake()
    {
        instance = this;
        QualitySettings.vSyncCount = 0;
#if UNITY_ANDROID
        Application.targetFrameRate = 30;
#endif
#if UNITY_EDITOR
        Application.targetFrameRate = 120;
#endif
#if UNITY_IOS
        Application.targetFrameRate = 60;
#endif
    }

    private void Start()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        LionAnalytics.LevelStart(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));
    }

    public void RecordNextLevel()
    {
        Debug.Log("YE");
        Debug.Log("SceneManager.GetActiveScene().buildIndex " + SceneManager.GetActiveScene().buildIndex); 
        int newLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (newLevelIndex >= SceneManager.sceneCountInBuildSettings)
            newLevelIndex = 1;
        Debug.Log("newLevelIndex " + newLevelIndex);

        PlayerPrefs.SetInt("Level", newLevelIndex);

        PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);
    }


    public void Win()
    {
        gameStatus = GameStatus.Win;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        LionAnalytics.LevelComplete(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        Taptic.Heavy();


        OnWin?.Invoke();
    }

    public void Lose()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        LionAnalytics.LevelFail(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        gameStatus = GameStatus.Lose;

        //if(!challengeLevel)
            //StartCoroutine(ShowAdWithDelay(showAdAfterGameOverDelay));

        OnLose?.Invoke();
    }

    public void NextLevel()
    {
        Debug.Log("Level = " + PlayerPrefs.GetInt("Level"));
        PlayerPrefs.SetInt("AttempNum", 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    public void Restart()
    {
        LionAnalytics.LevelRestart(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        PlayerPrefs.SetInt("AttempNum", PlayerPrefs.GetInt("AttemptNum", 1) + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public enum GameStatus
{
    Playing,
    Win,
    Lose
}