using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

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

    [SerializeField]
    private float winScreenDelay;

    private string levelCounterString;

    private void Awake()
    {
        instance = this;
        levelCounter.text = string.Format(levelCounter.text, PlayerPrefs.GetInt("LevelCount", 1));
    }

    private void Start()
    {
        GameManager.instance.OnWin += Win;
        GameManager.instance.OnLose += ShowLoseScreen;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnWin -= Win;
        GameManager.instance.OnLose -= ShowLoseScreen;
    }

    private void Update()
    {
        if (tutorial && Input.GetMouseButtonDown(0))
            tutorial.SetActive(false);
    }

    public void Win()
    {
        GameManager.instance.RecordNextLevel();

        //if(Controller.instance.finishBehavior == FinishBehavior.FillRoom)
            StartCoroutine(ShowWinScreen());
    }

    public void ShowWin()
    {
        winScreen.SetActive(true);
        FinishConfetti.instance.Confetti();
    }

    private IEnumerator ShowWinScreen()
    {
        yield return new WaitForSeconds(winScreenDelay);

        winScreen.SetActive(true);
    }

    public void ShowLoseScreen()
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

        PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 1) + 1);

        SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

}
