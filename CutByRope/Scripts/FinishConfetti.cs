using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FinishConfetti : MonoBehaviour
{

    [SerializeField] private float delay;

    [SerializeField] private GameObject confetti;

    private bool showConfetti = true;

    private void Start()
    {
        GameManager.instance.OnWin += Show;
        GameManager.instance.OnLose += DontShow;
    }

    private void Show()
    {
        //showConfetti = true;
        DOTween.Sequence().SetDelay(delay).OnComplete(Confetti);
    }

    private void DontShow()
    {
        showConfetti = false;
    }

    private void Confetti()
    {
        if (!showConfetti)
            return;

        confetti.SetActive(true);
    }

}
