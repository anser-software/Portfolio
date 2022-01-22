using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerButton : MonoBehaviour
{

    [SerializeField]
    private Buttons type;

    public void OnClick()
    {
        Controller.Instance.ButtonClicked(type);       
    }

}