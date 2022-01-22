using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;
using GameAnalyticsSDK;
using UnityEngine.Networking;
using DG.Tweening;
using LionStudios.Suite.Analytics;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    public Action OnWin, OnLose, OnWinTapesCoiledUp;

    [HideInInspector]
    public GameStatus gameStatus;


    public int loopLastLevels;

    public int[] firstAssembleScenes;

    public int[] secondAssembleScenes;

    public bool useSecondAssemble;

    private int totalWinTapes;

    private List<WinTape> winTapesCoiledUp = new List<WinTape>();


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

        totalWinTapes = FindObjectsOfType<WinTape>().Length;
    }

    public void RecordNextLevel()
    {
        int newLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (firstAssembleScenes.Contains(newLevelIndex) || secondAssembleScenes.Contains(newLevelIndex) || newLevelIndex >= SceneManager.sceneCountInBuildSettings)
            newLevelIndex = 1;

        PlayerPrefs.SetInt("Level", newLevelIndex);

        PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);
    }

    public void WinTapeCoiledUp(WinTape tape)
    {
        if (winTapesCoiledUp.Contains(tape))
            return;

        winTapesCoiledUp.Add(tape);

        if (winTapesCoiledUp.Count == totalWinTapes)
            OnWinTapesCoiledUp?.Invoke();
    }

    public void Win()
    {
        if (gameStatus == GameStatus.Win)
            return;

        gameStatus = GameStatus.Win;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        LionAnalytics.LevelComplete(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        Taptic.Heavy();


        OnWin?.Invoke();
    }

    public void LoadAssemble()
    {
        int assembleLevel;

        if (useSecondAssemble)
        {
            assembleLevel = PlayerPrefs.GetInt("AssembleLevel", secondAssembleScenes[0]);

            PlayerPrefs.SetInt("AssembleLevel", assembleLevel + 1);

            if (PlayerPrefs.GetInt("AssembleLevel") >= SceneManager.sceneCount || firstAssembleScenes.Contains(assembleLevel))
                PlayerPrefs.SetInt("AssembleLevel", secondAssembleScenes[0]);
        }
        else
        {
            assembleLevel = PlayerPrefs.GetInt("AssembleLevel", firstAssembleScenes[0]);

            PlayerPrefs.SetInt("AssembleLevel", assembleLevel + 1);

            if (PlayerPrefs.GetInt("AssembleLevel") >= SceneManager.sceneCount || secondAssembleScenes.Contains(assembleLevel))
                PlayerPrefs.SetInt("AssembleLevel", firstAssembleScenes[0]);
        }

        PlayerPrefs.SetInt("AttempNum", 1);
        SceneManager.LoadScene(assembleLevel);
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