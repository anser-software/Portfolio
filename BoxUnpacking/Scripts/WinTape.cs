using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tape))]
public class WinTape : MonoBehaviour
{

    //[SerializeField] [Range(0F, 1F)]
    //private float winCoiling;

    //[SerializeField]
    //private float finalCoilDuration;

    private Tape tape;

    private void Start()
    {
        tape = GetComponent<Tape>();

        tape.OnCoilUp += CoiledUp;
    }

    /*private void Update()
    {
        if (GameManager.instance.gameStatus != GameStatus.Playing)
            return;

        if(Mathf.InverseLerp(tape.InitialLatticeLocalY, tape.MaxLatticeLocalY, tape.GetLattice().localPosition.y) >= winCoiling)
        {
            if (tape.CoilUp(finalCoilDuration))
        }
    }*/

    private void CoiledUp()
    {
        GameManager.instance.WinTapeCoiledUp(this);
    }

}
