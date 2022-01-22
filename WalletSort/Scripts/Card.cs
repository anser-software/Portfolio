using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Card : MonoBehaviour, IPointerClickHandler
{

    public Place parentPlace { get; private set; }

    [SerializeField]
    private int type;

    public bool onlyCorrectPlacement;

    [SerializeField]
    private Vector3 selectedRotation;

    [SerializeField]
    private Vector3 placeRotation, placePosition;


    public int cardType { get; private set; }

    private void Awake()
    {
        cardType = type;
    }

    private void Start()
    {
        parentPlace = transform.parent.GetComponent<Place>();
    }

    public void SetParentPlace(Place parentPlace)
    {
        this.parentPlace = parentPlace;
        transform.parent = parentPlace.transform;
    }

    public Vector3 GetSelectedRotation()
    {
        return selectedRotation;
    }

    public Vector3 GetPlaceRotation()
    {
        return placeRotation;
    }

    public Vector3 GetPlacePosition()
    {
        return placePosition;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        bool success;

        Controller.instance.SelectCard(this, parentPlace == null ? Vector3.up * 4F : parentPlace.GetIntermediateLocalPoint(), out success);

        if (success)
            parentPlace = null;
    }

}
