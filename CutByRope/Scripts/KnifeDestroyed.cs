using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeDestroyed : MonoBehaviour
{

    [SerializeField]
    private float force;

    [SerializeField]
    private float forceRadius;

    [SerializeField]
    private float upwardForce;

    private Rigidbody[] parts;

    public void ApplyForce(Vector3 breakPos)
    {
        parts = transform.GetComponentsInChildren<Rigidbody>();

        foreach (var part in parts)
        {
            part.AddExplosionForce(force, breakPos, forceRadius);
            part.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
        }
    }
}
