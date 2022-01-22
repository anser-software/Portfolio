using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Confetti : MonoBehaviour
{

    [SerializeField]
    private GameObject confetti;

    [SerializeField]
    private float delay;

    private void Start()
    {
        GameManager.instance.OnWin += Fire;
    }

    private void Fire()
    {
        DOTween.Sequence().SetDelay(delay).OnComplete(() => confetti.SetActive(true));
    }

}
