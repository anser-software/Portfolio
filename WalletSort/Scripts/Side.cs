using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Side : MonoBehaviour, IPointerClickHandler
{

    private Place[] places;

    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();

        places = transform.GetComponentsInChildren<Place>();

        foreach(Place p in places)
        {
            p.DisableClick();
        }

        col.enabled = false;

        Controller.instance.OnCardPickedUp += () => col.enabled = true;

        Controller.instance.OnCardPlaced += () => col.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (var p in places)
        {
            if(!p.filled)
            {
                var success = p.TryFill();

                if(success)
                 return;
            }
        }
    }
}
