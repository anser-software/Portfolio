using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escalator : MonoBehaviour
{

    public static Escalator Instance { get; private set; }

    [SerializeField]
    private Transform[] checkPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3[] GetCheckPoints()
    {
        Vector3[] points = new Vector3[checkPoints.Length];

        for (int i = 0; i < checkPoints.Length; i++)
        {
            points[i] = checkPoints[i].position;
        }

        return points;
    }


    private void OnTriggerEnter(Collider other)
    {
        var otherStickman = other.GetComponent<Stickman>();

        if (otherStickman)
        {
            StartCoroutine(otherStickman.ActivateEscalatorAscension());
        }
    }
}