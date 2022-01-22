using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using LionStudios.Ads;
using GameAnalyticsSDK;
public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    public System.Action OnShopSelectionChanged;

    [SerializeField]
    private float winScreenDelay;

    [SerializeField]
    private GameObject winScreen, loseScreen, loseChallengeScreen, getNewBladeScreen, nextButton, challengeOfferScreen, starsScreen, loseVidPic;

    [SerializeField]
    private GameObject[] stars;

    [SerializeField]
    private ShopManager shop;

    [SerializeField]
    private Text levelCounter, reachX, moneyCounter, newBladePercentageCounter;

    [SerializeField]
    private float loseChallengeButtonFadeItDuration, loseChallengeButtonFadeItDelay;

    [SerializeField]
    private Button loseChallengeButton;

    [SerializeField]
    private Text loseChallengeText;

    [SerializeField]
    private Button declineChallengeButton;

    [SerializeField]
    private Text declineChallengeText;

    [SerializeField]
    private Button loseNoThanksButton;

    [SerializeField]
    private Text loseNoThanksText;

    [SerializeField]
    private float loseNoThanksButtonFadeInDelay;


    [SerializeField]
    private GameObject tutorial;

    [SerializeField]
    private GameObject newBladesProgressScreen;

    [SerializeField]
    private Slider newBladesSlider;

    [SerializeField]
    private Image bladePic;

    [SerializeField] [Range(0F, 1F)]
    private float bladeValuePerLevel;

    [SerializeField]
    private float newBladeSliderFillDuration, newBladeSliderFillDelay;

    [SerializeField]
    private UIReward reward;

    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private int coinNum;

    [SerializeField]
    private float coinDelay, coinMoveDuration;

    [SerializeField]
    private RectTransform moneyForThisLevel;

    [SerializeField]
    private RectTransform targetCoinPos;

    private Animator shopAnimator;

    private bool showWin, showInterstitial;

    private string levelCounterString;

    private bool showChallengeOffer, shopShown, nextLevelAfterAnim, fadeInLoseChallengeButton, declineChallengeFadeIn, loseNoThanksFadeIn;

    private float fader = 0F;

    private ShowAdRequest bladeAd, challengeAd, tryAgainChallengeAd, loseChallengeAd, noThanksAd, tryAgainAd, skipLevelAd;
    
    private void Awake()
    {
        instance = this;
        //reachX.text = string.Format(reachX.text, Mathf.Pow(2, BlockManager.instance.targetLevel + 1).ToString());
    }

    private int currentCoinProgress;

    private int amountAdded;

    public void AddMoneyAnimated(int amount, RectTransform initialPos, bool nextLevelOnComplete)
    {
        if(nextLevelOnComplete)
            nextLevelAfterAnim = true;

        currentCoinProgress = 0;

        amountAdded = amount;

        GameManager.instance.AddMoney(amount);

        SetMoneyTextValue(GameManager.instance.money - amountAdded);

        StartCoroutine(MoneyAnim(initialPos, nextLevelOnComplete));
    }


    private IEnumerator MoneyAnim(RectTransform initialPos, bool nextLevelOnComplete)
    {
        for (int i = 0; i < coinNum; i++)
        {
            var coinInstance = Instantiate(coinPrefab, transform);

            coinInstance.transform.position = initialPos.position;

            coinInstance.transform.DOMove(targetCoinPos.position, coinMoveDuration).OnComplete(() => CompleteCoinPath(coinInstance));

            yield return new WaitForSeconds(coinDelay);
        }

        if (nextLevelOnComplete)
            GameManager.instance.NextLevel();
    }

    public void AddSingleCoin(int moneyToAdd)
    {
        var coinInstance = Instantiate(coinPrefab, transform);

        coinInstance.transform.DOMove(targetCoinPos.position, coinMoveDuration).OnComplete(() => CompleteSingleCoin(coinInstance, moneyToAdd));
    }

    private void CompleteSingleCoin(GameObject coin, int money)
    {
        Destroy(coin);

        GameManager.instance.AddMoney(money);
    }

    private void CompleteCoinPath(GameObject coin)
    {
        Destroy(coin);

        currentCoinProgress++;

        int currentMoney = Mathf.CeilToInt(Mathf.Lerp(GameManager.instance.money - amountAdded,
            GameManager.instance.money, Mathf.InverseLerp(0, coinNum, currentCoinProgress)));

        SetMoneyTextValue(currentMoney);

    }

    private void Start()
    {
        GameManager.instance.OnWin += ShowWinScreen;
        GameManager.instance.OnLose += ShowLoseScreen;
        GameManager.instance.OnMoneyAmountChanged += UpdateMoneyText;
        GameManager.instance.OnInternetStatusSet += SetInternetDependentUI;

        SetAds();

        shopAnimator = shop.GetComponent<Animator>();

        shop.Initialize();

        if(!GameManager.instance.challengeLevel)
            levelCounter.text = string.Format(levelCounter.text, PlayerPrefs.GetInt("LevelCount", 1));
        else
            levelCounter.text = string.Format("Challenge {0}", PlayerPrefs.GetInt("ChallengeCount", 1));

        UpdateMoneyText();

        if (GameManager.internetConnection == 0)
        {
            reward.DisableReward();
            nextButton.SetActive(true);
        }
    }

    private void SetAds()
    {
        bladeAd = new ShowAdRequest();

        bladeAd.OnReceivedReward += BladeAdFinished;

        bladeAd.SetPlacement("Get_new_blade");

        challengeAd = new ShowAdRequest();

        challengeAd.OnReceivedReward += ChallengeAdFinished;

        challengeAd.SetPlacement("Play_challenge");

        tryAgainChallengeAd = new ShowAdRequest();

        tryAgainChallengeAd.OnReceivedReward += TryAgainChallengePostAd;

        tryAgainChallengeAd.SetPlacement("Replay_challenge");

        loseChallengeAd = new ShowAdRequest();

        loseChallengeAd.SetPlacement("Lose_challenge");

        noThanksAd = new ShowAdRequest();

        noThanksAd.SetPlacement("No_thanks");

        tryAgainAd = new ShowAdRequest();

        tryAgainAd.SetPlacement("Try_again");

        tryAgainAd.OnReceivedReward += TryAgainAdReward;

        skipLevelAd = new ShowAdRequest();

        skipLevelAd.SetPlacement("Skip_level");
    }

    private void TryAgainChallengePostAd(string adUnitId, MaxSdkBase.Reward _reward)
    {
        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "AppLovin", "Replay_challenge");

        SceneManager.LoadScene(PlayerPrefs.GetInt("ChallengeLevel", GameManager.instance.challengeLevelBeginIndex));
    }

    private void ChallengeAdFinished(string adUnitId, MaxSdkBase.Reward _reward)
    {
        if (PlayerPrefs.GetInt("ChallengeLevelWaitCounter", 0) >= GameManager.instance.offerChallengeLevelFrequency)
            PlayerPrefs.SetInt("ChallengeLevelWaitCounter", 0);

        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "AppLovin", "Play_challenge");

        SceneManager.LoadScene(PlayerPrefs.GetInt("ChallengeLevel", GameManager.instance.challengeLevelBeginIndex));
    }

    private void SetInternetDependentUI()
    {
        if (GameManager.internetConnection == 0)
        {
            reward.DisableReward();
            nextButton.SetActive(true);
        }
    }

    public void NoThanksButton()
    {
        if (Interstitial.IsAdReady)
        {
            Interstitial.Show(noThanksAd);
        }

        AddMoneyAnimated(GameManager.instance.moneyForThisLevel, moneyForThisLevel, true);
        //GameManager.instance.NextLevel();
    }

    private void UpdateMoneyText()
    {
        moneyCounter.text = GameManager.instance.money.ToString();
    }

    private void Update()
    {
        if (tutorial && Input.GetMouseButtonDown(0))
            tutorial.SetActive(false);

        if (fadeInLoseChallengeButton)
        {
            Color c = loseChallengeButton.image.color;
            c.a = Mathf.Lerp(0F, .75F, fader);
            loseChallengeButton.image.color = c;

            c = loseChallengeText.color;
            c.a = Mathf.Lerp(0F, 1F, fader);
            loseChallengeText.color = c;

            fader += Time.deltaTime / loseChallengeButtonFadeItDuration;

            if (fader >= 1F)
                fadeInLoseChallengeButton = false;
        } 

        if(declineChallengeFadeIn)
        {
            Color c = declineChallengeButton.image.color;
            c.a = Mathf.Lerp(0F, .75F, fader);
            declineChallengeButton.image.color = c;

            c = declineChallengeText.color;
            c.a = Mathf.Lerp(0F, 1F, fader);
            declineChallengeText.color = c;

            fader += Time.deltaTime / loseChallengeButtonFadeItDuration;

            if (fader >= 1F)
                declineChallengeFadeIn = false;
        }

        if (loseNoThanksFadeIn)
        {
            Color c = loseNoThanksButton.image.color;
            c.a = Mathf.Lerp(0F, .75F, fader);
            loseNoThanksButton.image.color = c;

            c = loseNoThanksText.color;
            c.a = Mathf.Lerp(0F, 1F, fader);
            loseNoThanksText.color = c;

            fader += Time.deltaTime / loseChallengeButtonFadeItDuration;

            if (fader >= 1F)
                loseNoThanksFadeIn = false;
        }
    }

    public void DeclineChallenge()
    {
        reward.UnholdDecrement();

        challengeOfferScreen.SetActive(false);
    }

    private bool won = false;

    public void ShowWinScreen()
    {
        showWin = true;
        won = true;
        DOTween.Sequence().SetDelay(winScreenDelay).OnComplete(WinScreen);
    }

    private float lastNewBladeValue;

    private void AddBladeValue()
    {
        lastNewBladeValue = PlayerPrefs.GetFloat("BladeValue", 0F);

        newBladesSlider.value = lastNewBladeValue;

        PlayerPrefs.SetFloat("BladeValue", Mathf.Clamp01(PlayerPrefs.GetFloat("BladeValue", 0F) + bladeValuePerLevel));

        if (PlayerPrefs.GetFloat("BladeValue", 0F) >= 1F)
        {
            reward.HoldDecrement();
            CheckIfUnlockedNewBlade();
        }
        else
        {
            DOTween.Sequence().SetDelay(newBladeSliderFillDelay).Append(DOTween.To(() => newBladesSlider.value, x => newBladesSlider.value = x,
                PlayerPrefs.GetFloat("BladeValue", 0F), newBladeSliderFillDuration)).OnComplete(CheckIfUnlockedNewBlade);

            DOTween.Sequence().SetDelay(newBladeSliderFillDelay).OnComplete(() => StartCoroutine(AnimateNewBladePercentageText()));
        }
    }

    private IEnumerator AnimateNewBladePercentageText()
    {
        newBladePercentageCounter.text = Mathf.CeilToInt(lastNewBladeValue).ToString();

        float x = 0F;

        do
        {
            x += Time.deltaTime / newBladeSliderFillDuration;

            newBladePercentageCounter.text = Mathf.CeilToInt(Mathf.Lerp(lastNewBladeValue, PlayerPrefs.GetFloat("BladeValue", 0F), x) * 100f).ToString() + "%";
            yield return null;
        } while (x < 1F);
    }

    private void CheckIfUnlockedNewBlade()
    {
        reward.EnableButtons();

        if (PlayerPrefs.GetFloat("BladeValue", 0F) >= 1F)
        {
            UnlockedNewBlade();
        }
    }

    private ShopItem currentUnlockedBlade;

    private void UnlockedNewBlade()
    {
        getNewBladeScreen.SetActive(true);

        currentUnlockedBlade = ShopManager.instance.GetItem(PlayerPrefs.GetInt("KnifeToUnlock", 1) ,2);

        bladePic.sprite = currentUnlockedBlade.icon.sprite;

        PlayerPrefs.SetFloat("BladeValue", 0F);
    }

    public void GetNewBlade()
    {
        if(RewardedAd.IsAdReady) 
            RewardedAd.Show(bladeAd);
        else
        {
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "AppLovin", "Get_new_blade");
        }

        //SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    private void BladeAdFinished(string adUnitId, MaxSdkBase.Reward _reward)
    {
        getNewBladeScreen.SetActive(false);
        newBladesProgressScreen.SetActive(false);

        PlayerPrefs.SetInt("KnifeToUnlock", PlayerPrefs.GetInt("KnifeToUnlock", 1) + 1);

        ShopManager.instance.UnlockItem(2, currentUnlockedBlade.index);

        ShopManager.instance.SelectItem(currentUnlockedBlade.index, 2);

        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "AppLovin", "Get_new_blade");

        showInterstitial = false;
        reward.UnholdDecrement();
    }

    public void LooseNewBlade()
    {
        if(!showChallengeOffer)
            reward.UnholdDecrement();

        getNewBladeScreen.SetActive(false);
        newBladesProgressScreen.SetActive(false);
    }

    public void SetMoneyTextValue(int amount)
    {
        moneyCounter.text = amount.ToString();
    }

    private void WinScreen()
    {
        if (!showWin)
            return;

        if(GameManager.internetConnection != 1 || !LionStudios.Ads.RewardedAd.IsAdReady)
        {
            reward.DisableReward();
            nextButton.SetActive(true);
        }

        if (PlayerPrefs.GetInt("KnifeToUnlock", 1) >= shop.GetItemCount(2))
        {
            reward.EnableButtons();

            newBladesProgressScreen.SetActive(false);
        }
        else if(GameManager.internetConnection == 1 && RewardedAd.IsAdReady)
            AddBladeValue();
        else
        {
            newBladesProgressScreen.SetActive(false);
        }

        if (PlayerPrefs.GetInt("ChallengeLevelWaitCounter", 0) >= GameManager.instance.offerChallengeLevelFrequency)
        {
            showChallengeOffer = true;
            declineChallengeFadeIn = true;
            challengeOfferScreen.SetActive(true);
            reward.HoldDecrement();
        }

        winScreen.SetActive(true);

        if(ABTestManager.instance.remoteVariables.nofail == 1 || ABTestManager.instance.simulateTestB)
        {
            starsScreen.SetActive(true);

            for (int i = 0; i < stars.Length; i++)
            {
                if (i < ABTestManager.instance.starsCollected)
                    stars[i].SetActive(true);
                else
                    stars[i].SetActive(false);
            }
        }

        GameManager.instance.RecordNextLevel();
    }

    public void ShowLoseScreen()
    {
        showWin = false;
        if (GameManager.instance.challengeLevel)
        {
            loseChallengeScreen.SetActive(true);
            DOTween.Sequence().SetDelay(loseChallengeButtonFadeItDelay).OnComplete(FadeItLoseChallengeButton);
        }
        else
        {
            if(GameManager.internetConnection == 1)
                DOTween.Sequence().SetDelay(loseNoThanksButtonFadeInDelay).OnComplete(() => loseNoThanksFadeIn = true);
            else
            {
                loseNoThanksButton.gameObject.SetActive(false);
                loseVidPic.SetActive(false);
            }

            loseScreen.SetActive(true);
        }
    }

    private void FadeItLoseChallengeButton()
    {
         fadeInLoseChallengeButton = true;
    }

    public void TryAgainButtonChallenge()
    {
        if(RewardedAd.IsAdReady)
            RewardedAd.Show(tryAgainChallengeAd);
        else
        {
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "AppLovin", "Replay_challenge");
        }
    }

    public void LoseButtonChallenge()
    {
        if (Interstitial.IsAdReady)
        {
            Interstitial.Show(loseChallengeAd);
        }

        PlayerPrefs.SetInt("AttempNum", 1);

        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    public void ShowShop()
    {
        if (shopShown)
            return;

        shop.gameObject.SetActive(true);

        shopShown = true;
    }

    public void HideShop()
    {
        shopAnimator.SetTrigger("Hide");
    }

    public void DisableShop()
    {
        shop.gameObject.SetActive(false);

        shopShown = false;
    }

    public void NextLevel()
    {
        if (nextLevelAfterAnim)
            return;

        AddMoneyAnimated(GameManager.instance.moneyForThisLevel, moneyForThisLevel, true);
    }

    public void Restart()
    {
        if (RewardedAd.IsAdReady)
        {
            RewardedAd.Show(tryAgainAd);
        }
        else
        {
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "AppLovin", "Try_again");
        }
    }

    private void TryAgainAdReward(string adUnitId, MaxSdkBase.Reward _reward)
    {
        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "AppLovin", "Try_again");

        GameManager.instance.Restart();
    }

    public void LoseScreenNoThanks()
    {
        Interstitial.Show(skipLevelAd);

        GameManager.instance.RecordNextLevel();

        GameManager.instance.NextLevel();
    }

    public void NextLevelCheat()
    {
        int newLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (newLevelIndex >= GameManager.instance.challengeLevelBeginIndex)
            newLevelIndex = 1;
        PlayerPrefs.SetInt("Level", newLevelIndex);

        if(!won)
            PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);

        if (PlayerPrefs.GetInt("ChallengeLevelWaitCounter", 0) >= GameManager.instance.offerChallengeLevelFrequency)
            PlayerPrefs.SetInt("ChallengeLevelWaitCounter", 0);

        PlayerPrefs.SetInt("AttempNum", 1);

        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    public void ChallengeLevel()
    {
        if (RewardedAd.IsAdReady)
        {
            RewardedAd.Show(challengeAd);
        } else
        {
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "AppLovin", "Play_challenge");
        }
    }

}
