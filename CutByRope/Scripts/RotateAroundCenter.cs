using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{

    [SerializeField]
    private Vector3 center;

    private Pin[] pins;

    [SerializeField]
    private float scale;

    private void Start()
    {
        pins = FindObjectsOfType<Pin>();
    }

    private void Update()
    {
        var avgPos = (pins[0].transform.position + pins[1].transform.position) / 2F;

        var rot = transform.rotation.eulerAngles;

        rot.y = (avgPos.x - center.x) * scale;

        transform.rotation = Quaternion.Euler(rot);
    }

}
