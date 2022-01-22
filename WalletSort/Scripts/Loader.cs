using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LionStudios.Suite.Analytics;
public class Loader : MonoBehaviour
{

    private void Awake()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
    // AppLovin SDK is initialized, start loading ads
        };

        MaxSdk.SetSdkKey("FPsy8BTEL2p_J-N_h9tFRBlWkpN_4--KWa9zJdmMdiL9yEFSG4dUN6Un2xDAK5OcuVeC_yrf2MhNVWwTVNDr7q");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

        LionAnalytics.GameStart();
        GameAnalyticsSDK.GameAnalytics.Initialize();
        int sceneIndex = PlayerPrefs.GetInt("Level", 1);
        SceneManager.LoadScene(sceneIndex);
    }

}
