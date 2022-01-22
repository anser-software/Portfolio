using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragableAssemblePart : MonoBehaviour, IPointerDownHandler
{

    public float initialHeight;

    public Transform targetPos;

    private bool dragable = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!dragable)
            return;

        AssembleSecondController.Instance.PickUp(transform, eventData.pointerCurrentRaycast.worldPosition);
    }

    public void Snap(float duration)
    {
        transform.DOMove(targetPos.position, duration);
        transform.DORotateQuaternion(targetPos.rotation, duration);

        dragable = false;
    }
}
