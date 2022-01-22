using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using LionStudios.Ads;
using GameAnalyticsSDK;
public class UIReward : MonoBehaviour
{

    public static UIReward instance { get; private set; }

    [SerializeField]
    private Text moneyForLevel;

    [SerializeField]
    private GameObject x3Button;

    [SerializeField]
    private Text x3ButtonText;

    [SerializeField]
    private Button noButton;

    [SerializeField]
    private Text noButtonText;

    [SerializeField]
    private float showNoButtonDelay, noButtonFadeItDuration;

    [SerializeField]
    private float delayAfterAd;

    [SerializeField]
    private float multiplierDecrementDelay, multiplierDecrementInterval;

    [SerializeField]
    private int startMultiplier, endMultiplier;

    private string multiplierString;

    private int multiplier = 9;

    private ShowAdRequest showRewardedAdRequest;

    private bool fadeInNoButton = false;

    private float fader = 0F;

    private bool adClicked = false;

    private bool rewardEnabled = true;

    private bool holdDecrement = false;

    private bool decrementing = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        moneyForLevel.text = GameManager.instance.moneyForThisLevel.ToString();

        showRewardedAdRequest = new ShowAdRequest();

        showRewardedAdRequest.SetPlacement("Multiplier");

        multiplier = startMultiplier;

        multiplierString = x3ButtonText.text;

        x3ButtonText.text = string.Format(multiplierString, multiplier.ToString());

        showRewardedAdRequest.OnReceivedReward += ApplyX3AD;

        DOTween.Sequence().SetDelay(showNoButtonDelay).OnComplete(ShowNoButton);

        DOTween.Sequence().SetDelay(multiplierDecrementDelay).OnComplete(() => StartDecrement());
    }
    
    public void UpdateMoneyForLevelText()
    {
        moneyForLevel.text = GameManager.instance.moneyForThisLevel.ToString();
    }

    public void HoldDecrement()
    {
        holdDecrement = true;
    }

    public void UnholdDecrement()
    {
        holdDecrement = false;

        StartDecrement();
    }

    public void StartDecrement()
    {
        if (PlayerPrefs.GetInt("LevelCount", 1) <= 2)
            return;

        if (holdDecrement || decrementing)
            return;

        decrementing = true;

        StopAllCoroutines();

        StartCoroutine(Decrement());
    }

    public void EnableButtons()
    {
        noButton.interactable = true;
        x3Button.GetComponent<Button>().interactable = true;
    }

    private IEnumerator Decrement()
    {
        do
        {
            yield return new WaitForSeconds(multiplierDecrementInterval);

            if (adClicked)
                yield break;

            multiplier--;

            x3ButtonText.text = string.Format(multiplierString, multiplier.ToString());
        } while (multiplier > endMultiplier);
    }

    private void ShowNoButton()
    {
        if (adClicked || !rewardEnabled)
            return;

        noButton.gameObject.SetActive(true);

        fadeInNoButton = true;
    }

    public void DisableReward()
    {
        rewardEnabled = false;
        x3Button.SetActive(false);
    }

    private void Update()
    {
        if(fadeInNoButton)
        {
            Color c = noButton.image.color;
            c.a = Mathf.Lerp(0F, .75F, fader);
            noButton.image.color = c;

            c = noButtonText.color;
            c.a = Mathf.Lerp(0F, 1F, fader);
            noButtonText.color = c;

            fader += Time.deltaTime / noButtonFadeItDuration;

            if (fader >= 1F)
                fadeInNoButton = false;
        }
    }

    public void MultiplyBy3()
    {
        adClicked = true;

        if (PlayerPrefs.GetInt("LevelCount", 1) > 2)
        {
            if (RewardedAd.IsAdReady)
            {
                RewardedAd.Show(showRewardedAdRequest);
            }
            else
            {
                GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "AppLovin", "Multiplier");
            }
        }
        else
            ApplyX3();
    }

    private void ApplyX3AD(string adUnitID, MaxSdkBase.Reward reward)
    {
        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "AppLovin", "Multiplier");

        noButton.gameObject.SetActive(false);

        ApplyX3();
    }

    private void ApplyX3()
    {
        noButton.gameObject.SetActive(false);

        GameManager.instance.rewardedAdShown = true;

        x3Button.SetActive(false);

        GameManager.instance.moneyForThisLevel *= multiplier;

        moneyForLevel.text = GameManager.instance.moneyForThisLevel.ToString();

        DOTween.Sequence().SetDelay(delayAfterAd).OnComplete(() => UIManager.instance.AddMoneyAnimated(GameManager.instance.moneyForThisLevel, moneyForLevel.rectTransform, true));

    }

}
