using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Booster : MonoBehaviour
{

    [SerializeField]
    private Vector3 pushOutSourcePos;

    [SerializeField]
    private float pushOutForce;

    [SerializeField]
    private float finalScale, duration;

    private Vector3 initScale;

    private void Start()
    {
        initScale = transform.localScale;

        DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.one * finalScale, duration);
    }


    private void OnTriggerStay(Collider other)
    {
        var rb = other.GetComponent<Rigidbody>();

        if(rb)
        {
            var currentPushOutForce = Mathf.Lerp(pushOutForce, 0F, Mathf.InverseLerp(initScale.x, finalScale, transform.localScale.x));

            rb.AddForce((transform.position - pushOutSourcePos).normalized * currentPushOutForce);
        }
    }

}
