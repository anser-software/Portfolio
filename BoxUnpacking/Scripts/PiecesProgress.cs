using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecesProgress : MonoBehaviour
{

    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private Text collectedText;

    [SerializeField]
    private Image progressbar;

    private string collectedString;

    private void Start()
    {
        collectedString = collectedText.text;

        GameManager.instance.OnWin += Win;
    }

    private void Win()
    {
        int currentPart = PlayerPrefs.GetInt("CurrentPart", 0);

        if (currentPart > 0)
        {
            collectedText.text = string.Format(collectedString, currentPart.ToString(), 5);

            progressbar.fillAmount = currentPart / 5F;

            parent.SetActive(true);
        }
    }

}
