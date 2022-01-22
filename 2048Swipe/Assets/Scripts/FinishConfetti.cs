using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishConfetti : MonoBehaviour
{

    [SerializeField] private GameObject confetti;

    private void Start()
    {
        GameManager.instance.OnWin += Show;
    }

    private void Show()
    {
        confetti.SetActive(true);
    }


}
