using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickmanSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject stickman;

    [SerializeField]
    private int count;

    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(stickman, transform).transform.localPosition = Vector3.zero 
                + Vector3.forward * Random.Range(-1F, 1F) 
                + Vector3.right * Random.Range(-1F, 1F);
        }
    }

}
