using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinWaveTrigger : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float height, duration;

    private void Start()
    {
        GameManager.instance.OnWin += Activate;
    }

    private void Activate()
    {
        animator.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var checker = other.GetComponent<CutChecker>();

        if(checker)
        {
            checker.WinWave(height, duration);
        }
    }

}
