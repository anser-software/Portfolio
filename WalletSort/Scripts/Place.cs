using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Place : MonoBehaviour, IPointerClickHandler
{

    public bool filled { get; private set; }

    [HideInInspector]
    public int targetType;

    [SerializeField]
    private Vector3 intermediateLocalPoint;

    [SerializeField]
    private Vector3 positionOffset, rotationOffset;

    private bool clickEnabled = true;
    public void DisableClick()
    {
        clickEnabled = false;

        Destroy(GetComponent<Collider>());
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (clickEnabled == false)
            return;

        TryFill();
    }

    public Vector3 GetRotationOffset()
    {
        return rotationOffset;
    }

    public Vector3 GetPositionOffset()
    {
        return positionOffset;
    }

    public bool TryFill()
    {
        bool succsess;

        Controller.instance.PlaceCard(this, out succsess);

        if (succsess)
            filled = true;

        return succsess;
    }

    public void SetFill(bool filled)
    {
        this.filled = filled;
    }

    private void Start()
    {
        if (transform.GetComponentInChildren<Card>())
            filled = true;
    }

    public Vector3 GetIntermediateLocalPoint()
    {
        return intermediateLocalPoint;
    }

}
