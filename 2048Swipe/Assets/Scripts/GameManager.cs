using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;
using GameAnalyticsSDK;
public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    public Action OnWin, OnLose;

    public int loopLastLevels;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level0" + PlayerPrefs.GetInt("LevelCount", 1));
    }

    public void RecordNextLevel()
    {
        int newLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (newLevelIndex >= SceneManager.sceneCountInBuildSettings)
            newLevelIndex = SceneManager.sceneCountInBuildSettings - loopLastLevels;

        if(newLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Invalid scene");
            newLevelIndex = 1;
        }

        PlayerPrefs.SetInt("Level", newLevelIndex);

        PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);
    }

    public void Win()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        Taptic.Heavy();

        OnWin?.Invoke();
    }

    public void Lose()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        OnLose?.Invoke();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    public void Restart()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }
}
