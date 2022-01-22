using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutChecker : MonoBehaviour
{

    [SerializeField]
    private Cuttable target;

    private bool cut = false;


    private void OnTriggerEnter(Collider collider)
    {
        if (cut)
            return;

        if (collider.CompareTag("Rope"))
        {
            cut = true;

            target.CheckerPassed(transform.position);

            //Taptic.Light();
        }
    }

    public void PassChecker()
    {
        if (cut)
            return;
        cut = true;

        target.CheckerPassed(transform.position);
    }

    public void WinWave(float height, float duration)
    {
        target.WinWave(height, duration);
    }
}
