using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LionStudios.Ads;
using GameAnalyticsSDK;
public class ShopManager : MonoBehaviour
{

    public static ShopManager instance { get; private set; }

    [SerializeField]
    private Button[] tabButtons;

    [SerializeField]
    private GameObject[] tabContents;

    [SerializeField]
    private GameObject unlockRandomButton;

    [SerializeField]
    private float unlockRandomAnimWait;

    [SerializeField]
    private int unlockRandomAnimCount, unlockRandomCost;

    [SerializeField]
    private Text unlockRandomCostText;

    [SerializeField]
    private int earnRewardMoneyAdd;

    [SerializeField]
    private Text earnRewardMoneyText;

    [SerializeField]
    private RectTransform earnFreeButton;

    [SerializeField]
    private int[] unlockedPinsByDef, unlockedBgsByDef, unlockedKnivesByDef;

    private List<ShopItem>[] shopItems;

    private ShopItem[] selectedItemPerTab;

    private int currentTab = 0;

    private ShowAdRequest _ShowRewardedAdRequest;

    private bool unlockRandomAnimInProgress = false;

    private void Awake()
    {
        instance = this;
    }

    public int GetItemCount(int tab)
    {
        return shopItems[tab].Count;
    }

    public void Initialize()
    {
        instance = this;

        shopItems = new List<ShopItem>[tabContents.Length];

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i] = new List<ShopItem>(tabContents[i].transform.GetComponentsInChildren<ShopItem>());
        }

        for (int i = 0; i < shopItems.Length; i++)
        {
            for (int j = 0; j < shopItems[i].Count; j++)
            {
                shopItems[i][j].Initialize(j);
            }
        }

        selectedItemPerTab = new ShopItem[tabContents.Length];

        unlockRandomCostText.text = unlockRandomCost.ToString();

        earnRewardMoneyText.text = earnRewardMoneyAdd.ToString();

        UnlockDefaultItems();

        LoadSavedData();

        SetupRewardedCallbacks();
    }

    private void SetupRewardedCallbacks()
    {
        _ShowRewardedAdRequest = new ShowAdRequest();
        // Ad event callbacks
        _ShowRewardedAdRequest.OnDisplayed += adUnitId => Debug.Log("Displayed Rewarded Ad :: Ad Unit ID = " + adUnitId);
        _ShowRewardedAdRequest.OnClicked += adUnitId => Debug.Log("Clicked Rewarded Ad :: Ad Unit ID = " + adUnitId);
        _ShowRewardedAdRequest.OnHidden += adUnitId => Debug.Log("Closed Rewarded Ad :: Ad Unit ID = " + adUnitId);
        _ShowRewardedAdRequest.OnFailedToDisplay += (adUnitId, error) => Debug.LogError("Failed To Display Rewarded Ad :: Error = " + error + " :: Ad Unit ID = " + adUnitId);
        _ShowRewardedAdRequest.OnReceivedReward += RewardPlayer;

        _ShowRewardedAdRequest.SetPlacement("Earn_free");

        // Analytics settings and data
        _ShowRewardedAdRequest.sendAnalyticsEvents = true; // Defaults to true
        _ShowRewardedAdRequest.SetLevel(PlayerPrefs.GetInt("LevelCount", 1)); // This will be set prior to “showing” the ad as well.
    }

    private void RewardPlayer(string adUnitID, MaxSdkBase.Reward reward)
    {
        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "AppLovin", "Earn_free");
        UIManager.instance.AddMoneyAnimated(earnRewardMoneyAdd, earnFreeButton, false);
    }

    public ShopItem GetItem(int index, int tab)
    {
        return shopItems[tab][index];
    }

    private void UnlockDefaultItems()
    {
        if (PlayerPrefs.GetInt("DefaultItemsUnlocked", 0) == 1)
            return;

        foreach (var item in unlockedPinsByDef)
        {
            UnlockItem(0, item);
        }

        foreach (var item in unlockedBgsByDef)
        {
            UnlockItem(1, item);
        }

        foreach (var item in unlockedKnivesByDef)
        {
            UnlockItem(2, item);
        }

        SelectItem(0, 0);

        SelectItem(0, 1);

        SelectItem(0, 2);


        PlayerPrefs.SetInt("DefaultItemsUnlocked", 1);
    }

    private void LoadSavedData()
    {
        for (int i = 0; i < shopItems[0].Count; i++)
        {
            if (PlayerPrefs.GetInt("IsPinUnlocked_" + i, 0) == 1)
                shopItems[0][i].Unlock();
        }

        int selectedPin = PlayerPrefs.GetInt("SelectedPin", 0);

        selectedItemPerTab[0] = shopItems[0][selectedPin];

        selectedItemPerTab[0].Select();


        for (int i = 0; i < shopItems[1].Count; i++)
        {
            if (PlayerPrefs.GetInt("IsBgUnlocked_" + i, 0) == 1)
                shopItems[1][i].Unlock();
        }

        int selectedBg = PlayerPrefs.GetInt("SelectedBg", 0);

        selectedItemPerTab[1] = shopItems[1][selectedBg];

        selectedItemPerTab[1].Select();


        for (int i = 0; i < shopItems[2].Count; i++)
        {
            if (PlayerPrefs.GetInt("IsKnifeUnlocked_" + i, 0) == 1)
                shopItems[2][i].Unlock();
        }

        int selectedWire = PlayerPrefs.GetInt("SelectedKnife", 0);

        selectedItemPerTab[2] = shopItems[2][selectedWire];

        selectedItemPerTab[2].Select();
       /*

        for (int i = 0; i < shopItems[3].Count; i++)
        {
            if (PlayerPrefs.GetInt("IsKnifeUnlocked_" + i, 0) == 1)
                shopItems[3][i].Unlock();
        }

        int selectedKnife = PlayerPrefs.GetInt("SelectedKnife", 0);

        selectedItemPerTab[3] = shopItems[3][selectedWire];

        selectedItemPerTab[3].Select();*/
    }

    public void ShopHidden()
    {
        UIManager.instance.DisableShop();
    }

    public void OpenTab(int index)
    {
        tabButtons[index].interactable = false;
        tabContents[index].SetActive(true);

        currentTab = index;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i == index)
                continue;

            tabButtons[i].interactable = true;
            tabContents[i].SetActive(false);
        }

        if (index == 2)
            unlockRandomButton.SetActive(false);
        else
            unlockRandomButton.SetActive(true);
    }

    public void SelectItem(int index, int tab = -1)
    {
        if (tab == -1)
            tab = currentTab;

        if (selectedItemPerTab[tab])
            selectedItemPerTab[tab].UnSelect();

        if(tab == 0)
        {
            PlayerPrefs.SetInt("SelectedPin", index);
        } else if(tab == 1)
        {
            PlayerPrefs.SetInt("SelectedBg", index);
        }
        else if (tab == 2)
        {
            PlayerPrefs.SetInt("SelectedKnife", index);
        }
        else if (tab == 3)
        {
            PlayerPrefs.SetInt("SelectedKnife", index);
        }

        shopItems[tab][index].Select();

        selectedItemPerTab[tab] = shopItems[tab][index];

        UIManager.instance.OnShopSelectionChanged?.Invoke();
    }

    public void UnlockItem(int tab, int index)
    {
        if (tab == 0)
        {
            PlayerPrefs.SetInt("IsPinUnlocked_" + index, 1);
        }
        else if (tab == 1)
        {
            PlayerPrefs.SetInt("IsBgUnlocked_" + index, 1);
        } else if(tab == 2)
        {
            PlayerPrefs.SetInt("IsKnifeUnlocked_" + index, 1);
        }
        else if(tab == 3)
        {
            PlayerPrefs.SetInt("IsKnifeUnlocked_" + index, 1);
        }

        shopItems[tab][index].Unlock();
    }

    public void UnlockRandom()
    {
        if (unlockRandomAnimInProgress)
            return;

        if (GameManager.instance.SpendMoney(unlockRandomCost))
        {
            int lastIndex = 0, count = 0;

            for (int i = 0; i < shopItems[currentTab].Count; i++)
            {
                if (!shopItems[currentTab][i].isUnlocked)
                {
                    count++;
                    lastIndex = i;

                    if (count > 1)
                    {
                        unlockRandomAnimInProgress = true;

                        StartCoroutine(UnlockRandomSequence());
                        return;
                    }
                }
            }

            if (count == 0)
            {
                GameManager.instance.AddMoney(unlockRandomCost);
                return;
            }

            GameAnalytics.NewDesignEvent("random_skin");

            //LionStudios.Analytics.LogEvent("random_skin");

            UnlockItem(currentTab, lastIndex);

            SelectItem(lastIndex);
        }
    }

    public void EarnFree()
    {
        if(RewardedAd.IsAdReady)
            RewardedAd.Show(_ShowRewardedAdRequest);
        else
        {
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, "AppLovin", "Earn_free");
        }
    }

    private IEnumerator UnlockRandomSequence()
    {
        int selectedItem = 0;

        int previousSelection = 0;

        for (int i = 0; i < unlockRandomAnimCount; i++)
        {
            do
            {
                selectedItem = Random.Range(0, shopItems[currentTab].Count);
            } while (selectedItem == previousSelection || shopItems[currentTab][selectedItem].isUnlocked);

            shopItems[currentTab][selectedItem].Select();

            previousSelection = selectedItem;

            yield return new WaitForSeconds(unlockRandomAnimWait);

            shopItems[currentTab][selectedItem].UnSelect();
        }

        do
        {
            selectedItem = Random.Range(0, shopItems[currentTab].Count);
        } while (shopItems[currentTab][selectedItem].isSelected || shopItems[currentTab][selectedItem].isUnlocked);

        GameAnalytics.NewDesignEvent("random_skin");

        //LionStudios.Analytics.LogEvent("random_skin");

        UnlockItem(currentTab, selectedItem);

        SelectItem(selectedItem);

        unlockRandomAnimInProgress = false;
    }

}
