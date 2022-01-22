using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{

    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = Controller.instance.currentBlock.transform.position + offset;
    }
}
