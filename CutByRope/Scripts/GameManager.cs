using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;
using GameAnalyticsSDK;
using LionStudios.Debugging;
using LionStudios.Ads;
using UnityEngine.Networking;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    public static int internetConnection = -1;

    public Action OnWin, OnLose, OnMoneyAmountChanged, OnInternetStatusSet;

    [HideInInspector]
    public GameStatus gameStatus;

    public bool challengeLevel;

    public int offerChallengeLevelFrequency;

    [SerializeField]
    private int moneyPerChallengeCoin;

    [SerializeField]
    private float collectCoinsInterval;

    [Range(0F, 1F)]
    public float coinDropChance;

    public int loopLastLevels;

    public int challengeLevelBeginIndex;

    [SerializeField]
    private int defaultTotalMoney;

    public int moneyForThisLevel;

    [SerializeField]
    private float showAdAfterGameOverDelay;

    [SerializeField]
    private GameObject winSFX;

    public int money { get; private set; }

    private ShowAdRequest gameOverAd;

    [HideInInspector]
    public bool rewardedAdShown;

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
        money = PlayerPrefs.GetInt("Money", defaultTotalMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (money - amount < 0)
            return false;

        money -= amount;

        PlayerPrefs.SetInt("Money", money);

        OnMoneyAmountChanged?.Invoke();

        return true;
    } 

    public void SetMoneyByStars(int starsCollected)
    {
        Debug.Log("SET MONEY BY STARS");
        moneyForThisLevel = starsCollected * 10;
    }

    public void AddMoney(int amount)
    {
        money += amount;

        PlayerPrefs.SetInt("Money", money);

        OnMoneyAmountChanged?.Invoke();
    }

    public IEnumerator internetConnectivity(System.Action<bool> action)
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com/");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    private void Start()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        //LionAnalytics.LevelStart(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        Debug.Log("internetConnection = " + internetConnection.ToString());

        LionStudios.Runtime.Sdks.AppLovin.WhenInitialized(() => Banner.Show());

        SetupInterstitialCallbacks();

        StartCoroutine(internetConnectivity((isConnected) => {
            if (internetConnection == -1) internetConnection = isConnected ? 1 : 0;
            Debug.Log("internetConnection set = " + internetConnection.ToString());
            OnInternetStatusSet?.Invoke();
        }));
        
    }

    public void RecordNextLevel()
    {
        if (!challengeLevel)
        {
            int newLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (newLevelIndex >= challengeLevelBeginIndex)
                newLevelIndex = challengeLevelBeginIndex - loopLastLevels;

            if (newLevelIndex >= challengeLevelBeginIndex)
            {
                Debug.LogError("Invalid scene");
                newLevelIndex = 1;
            }

            PlayerPrefs.SetInt("Level", newLevelIndex);

            PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);
        } else
        {
            int newChallengeLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (newChallengeLevelIndex >= SceneManager.sceneCountInBuildSettings)
                newChallengeLevelIndex = challengeLevelBeginIndex;

            PlayerPrefs.SetInt("ChallengeLevel", newChallengeLevelIndex);

            PlayerPrefs.SetInt("ChallengeCount", PlayerPrefs.GetInt("ChallengeCount", 1) + 1);
        }
    }

    void SetupInterstitialCallbacks()
    {
        gameOverAd = new ShowAdRequest();
        gameOverAd.SetPlacement("Game_over");
    }

    public void Win()
    {
        WinSFX();

        gameStatus = GameStatus.Win;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        //LionAnalytics.LevelComplete(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        Taptic.Heavy();

        if (challengeLevel)
            StartCoroutine(CollectCoins());
        else
        {
            PlayerPrefs.SetInt("ChallengeLevelWaitCounter", PlayerPrefs.GetInt("ChallengeLevelWaitCounter", 0) + 1);

            if (ABTestManager.instance.remoteVariables.nofail == 1 || ABTestManager.instance.simulateTestB)
                SetMoneyByStars(ABTestManager.instance.starsCollected);
        }

        OnWin?.Invoke();
    }

    private void WinSFX()
    {
        Instantiate(winSFX, transform.position, Quaternion.identity);
    }

    private IEnumerator CollectCoins()
    {
        foreach (var coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            yield return new WaitForSeconds(collectCoinsInterval);

            coin.transform.DOScale(0F, 0.5F);

            UIManager.instance.AddSingleCoin(moneyPerChallengeCoin);
        }
    }

    public void Lose()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level0" + PlayerPrefs.GetInt("LevelCount", 1));

        //LionAnalytics.LevelFail(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

        gameStatus = GameStatus.Lose;

        //if(!challengeLevel)
            //StartCoroutine(ShowAdWithDelay(showAdAfterGameOverDelay));

        OnLose?.Invoke();
    }

    private IEnumerator ShowAdWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

#if !UNITY_IOS
        if (Interstitial.IsAdReady)
        {
            LionStudios.Ads.Interstitial.Show(gameOverAd);
        }
#endif
    }

    public void NextLevel()
    {
        if(PlayerPrefs.GetInt("ChallengeLevelWaitCounter", 0) >= offerChallengeLevelFrequency)
            PlayerPrefs.SetInt("ChallengeLevelWaitCounter", 0);

        PlayerPrefs.SetInt("AttempNum", 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    public void Restart()
    {
        //LionAnalytics.LevelRestart(PlayerPrefs.GetInt("LevelCount", 1), PlayerPrefs.GetInt("AttemptNum", 1));

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