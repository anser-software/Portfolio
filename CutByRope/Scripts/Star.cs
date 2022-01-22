using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{

    [SerializeField]
    private GameObject collectEffect;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Rope"))
        {
            Taptic.Light();

            Instantiate(collectEffect, transform.position, Quaternion.identity);

            ABTestManager.instance.StarCollected();

            Destroy(gameObject);
        }
    }

}
