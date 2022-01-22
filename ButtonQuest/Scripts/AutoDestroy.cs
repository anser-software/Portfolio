using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AutoDestroy : MonoBehaviour
{

    [SerializeField]
    private float delay;

    void Start()
    {
        DOTween.Sequence().SetDelay(delay).OnComplete(Kill);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
