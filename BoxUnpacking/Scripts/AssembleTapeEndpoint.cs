using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssembleTapeEndpoint : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{

    [SerializeField]
    private AssembleTapeEndpoint linkedPoint;

    private bool active = true;

    public AssembleTapeEndpoint GetLinkedPoint()
    {
        return linkedPoint;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!active)
            return;

        AssembleController.Instance.StartDragging(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!active)
            return;

        AssembleController.Instance.TrySnapping(this);
    }

    public void Deactivate()
    {
        active = false;

        AssembleController.Instance.ATEDeactivated();
    }
}
