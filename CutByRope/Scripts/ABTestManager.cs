using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class ABTestManager : MonoBehaviour
{

    public static ABTestManager instance { get; private set; }

    [SerializeField]
    private GameObject parentA, parentB;

    public bool simulateTestA, simulateTestB;

    public int starsCollected { get; private set; }
    
    public RemoteVariables remoteVariables { get; private set; } = new RemoteVariables();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (simulateTestB)
        {
            parentA.SetActive(false);
            parentB.SetActive(true);
        }
        else if (simulateTestA)
        {
            parentA.SetActive(true);
            parentB.SetActive(false);
        }
        else
            LionStudios.Runtime.Sdks.AppLovin.WhenInitialized(() => UpdateConfig());
    }

    public void StarCollected()
    {
        starsCollected++;

        GameManager.instance.SetMoneyByStars(starsCollected);

        if(UIReward.instance)
            UIReward.instance.UpdateMoneyForLevelText();
    }

    private void UpdateConfig()
    {
        LionStudios.Runtime.Sdks.AppLovin.LoadRemoteData(remoteVariables);

        parentA.SetActive(remoteVariables.nofail == 0);
        parentB.SetActive(remoteVariables.nofail == 1);

        Debug.Log("[A/B Test] nofail = " + remoteVariables.nofail);

        if(remoteVariables.nofail == 0)
        GameAnalytics.SetCustomDimension01("fail-bomb");
         else
        GameAnalytics.SetCustomDimension01("nofail-stars");
    }

}

public class RemoteVariables
{
    public int nofail = 0;
}