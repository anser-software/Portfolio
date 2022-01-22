using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{

    private void Awake()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
        int sceneIndex = PlayerPrefs.GetInt("Level", 1);
        SceneManager.LoadScene(sceneIndex);
    }

}
