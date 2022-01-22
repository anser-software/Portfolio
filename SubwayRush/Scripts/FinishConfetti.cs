using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishConfetti : MonoBehaviour
{

    public static FinishConfetti instance { get; private set; }

    [SerializeField] private GameObject confetti;

    [SerializeField]
    private float delay = 2F;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager.instance.OnWin += Show;
    }

    private void Show()
    {
        //StartCoroutine(ShowConfetti());
    }

    private IEnumerator ShowConfetti()
    {
        yield return new WaitForSeconds(delay);
        confetti.SetActive(true);
    }

    public void Confetti()
    {
        confetti.SetActive(true);
    }
}
