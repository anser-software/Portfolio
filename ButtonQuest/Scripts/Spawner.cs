using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    private GameObject objectToInstantiate;

    [SerializeField]
    private Transform place;

    public void Spawn()
    {
        var instance = Instantiate(objectToInstantiate, place.position, place.rotation);
    }

}
