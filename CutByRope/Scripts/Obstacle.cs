using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    private bool cut = false;

    [SerializeField]
    private GameObject hitEffect;

    [SerializeField]
    private bool hideAfterHit;

    private void OnTriggerEnter(Collider collider)
    {
        if (cut)
            return;

        if (collider.CompareTag("Rope"))
        {
            cut = true;

            Instantiate(hitEffect, transform.position, Quaternion.identity);

            Controller.instance.TearRope(transform.position);

            if (hideAfterHit)
                gameObject.SetActive(false);
        }
    }
}
