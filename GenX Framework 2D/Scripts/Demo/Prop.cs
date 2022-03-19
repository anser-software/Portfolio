using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{

    void Start()
    {
        GetComponent<SpriteRenderer>().flipX = Random.Range(0, 2) == 0;
        Destroy(this);
    }

}

