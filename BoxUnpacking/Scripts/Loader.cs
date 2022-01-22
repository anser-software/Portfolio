using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LionStudios.Suite.Analytics;
public class Loader : MonoBehaviour
{

    private void Awake()
    {
        LionAnalytics.GameStart();
        GameAnalyticsSDK.GameAnalytics.Initialize();
        int sceneIndex = PlayerPrefs.GetInt("Level", 1);
        SceneManager.LoadScene(sceneIndex);
    }

}
