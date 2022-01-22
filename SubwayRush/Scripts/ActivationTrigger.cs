using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour
{

    public Stickman stickman;

    private bool activatedNext;


    private void OnTriggerStay(Collider other)
    {
        if (activatedNext || !stickman.activatedElevator)
            return;

        var otherStickman = other.GetComponent<Stickman>(); 

        if(otherStickman && !otherStickman.activatedElevator)
        {
            otherStickman.ActivateEscalatorAscension();
        }
    }

}
