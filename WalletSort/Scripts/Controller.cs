using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Controller : MonoBehaviour
{
    public static Controller instance { get; private set; }

    public System.Action OnCardPlaced, OnCardPickedUp;

    [SerializeField]
    private Transform flyToWhenSelected;

    [SerializeField]
    private float flyDuration;

    private Card selectedCard = null;

    private bool flyToPlaceSequencePlaying = false;

    private void Awake()
    {
        instance = this;
    }

    public void SelectCard(Card card, Vector3 intermediateLocalPoint, out bool success)
    {
        if (selectedCard != null)
        {
            success = false;
            return;
        }

        card.parentPlace.SetFill(false);

        success = true;

        selectedCard = card;

        flyToPlaceSequencePlaying = true;

        var targetRotation = card.GetSelectedRotation(); //card.transform.rotation.eulerAngles;

        //targetRotation.y = 0F;

        var selectSequence = DOTween.Sequence();

        selectSequence
            .Append(selectedCard.transform.DOLocalMove(intermediateLocalPoint, flyDuration / 2F))
            .Append(selectedCard.transform.DOMove(flyToWhenSelected.position, flyDuration / 2F).SetEase(Ease.OutSine))
            .Join(selectedCard.transform.DORotate(targetRotation, flyDuration / 2F).SetEase(Ease.OutSine))
            .OnComplete(() => flyToPlaceSequencePlaying = false);

        OnCardPickedUp?.Invoke();
    }

    public void PlaceCard(Place place, out bool success)
    {
        if (selectedCard == null || flyToPlaceSequencePlaying || place.filled || 
            (place.targetType != selectedCard.cardType && place.targetType != -2 && selectedCard.onlyCorrectPlacement))
        {
            success = false;
            return;
        }

        selectedCard.SetParentPlace(place);

        success = true;

        Card thisCard = selectedCard;

        var placeSequence = DOTween.Sequence();

        var intermediatePoint = place.transform.TransformDirection(place.GetIntermediateLocalPoint());

        var rotFromPlace = place.GetRotationOffset();

        var rotFromCard = thisCard.GetPlaceRotation();

        var posFromPlace = place.GetPositionOffset();

        var posFromCard = thisCard.GetPlacePosition();

        placeSequence
            .Append(selectedCard.transform.DOMove(place.transform.position + intermediatePoint, flyDuration / 2F))
           .Join(selectedCard.transform.DORotate(place.transform.rotation.eulerAngles + rotFromPlace + rotFromCard, flyDuration / 2F).SetEase(Ease.OutSine))
            .Append(selectedCard.transform.DOLocalMove(posFromPlace + posFromCard, flyDuration / 2F).SetEase(Ease.OutSine));

        selectedCard = null;

        OnCardPlaced?.Invoke();
    }

}
