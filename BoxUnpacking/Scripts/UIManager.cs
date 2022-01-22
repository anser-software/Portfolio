using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [SerializeField]
    private float finishDelay;

    [SerializeField]
    private GameObject winScreen;

    [SerializeField]
    private GameObject loseScreen;

    [SerializeField]
    private Text levelCounter;

    [SerializeField]
    private Text reachX;

    [SerializeField]
    private GameObject tutorial;

    private bool showWin = false;

    private string levelCounterString;

    private void Awake()
    {
        instance = this;
        levelCounter.text = string.Format(levelCounter.text, PlayerPrefs.GetInt("LevelCount", 1));
        //reachX.text = string.Format(reachX.text, Mathf.Pow(2, BlockManager.instance.targetLevel + 1).ToString());
    }

    private void Start()
    {
        GameManager.instance.OnWin += ShowWinScreen;
        GameManager.instance.OnLose += ShowLoseScreen;
    }

    private void Update()
    {
        if (tutorial && Input.GetMouseButtonDown(0))
            tutorial.SetActive(false);
    }

    public void ShowWinScreen()
    {
        showWin = true;

        DOTween.Sequence().SetDelay(finishDelay).OnComplete(WinScreen);
    }

    private void WinScreen()
    {
        if (!showWin)
            return;

        winScreen.SetActive(true);

        GameManager.instance.RecordNextLevel();
    }


    public void ShowLoseScreen()
    {
        showWin = false;

        DOTween.Sequence().SetDelay(finishDelay).OnComplete(LoseScreen);
    }


    public void LoseScreen()
    {
        loseScreen.SetActive(true);
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
    }
    public void Restart()
    {
        GameManager.instance.Restart();
    }

    public void NextLevelCheat()
    {
        int newLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (newLevelIndex >= SceneManager.sceneCountInBuildSettings)
            newLevelIndex = 1;
        PlayerPrefs.SetInt("Level", newLevelIndex);
        Debug.Log("Custom NextLevelCheat");

        PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);

        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

}
