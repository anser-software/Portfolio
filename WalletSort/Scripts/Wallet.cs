using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{

    [SerializeField]
    private GameObject[] placements;

    [SerializeField]
    private int[] targetTypeForEachPlacement;

    private void Start()
    {
        Controller.instance.OnCardPlaced += CheckPlaces;

        for (int i = 0; i < placements.Length; i++)
        {
            var places = placements[i].transform.GetComponentsInChildren<Place>();

            foreach (var p in places)
            {
                p.targetType = targetTypeForEachPlacement[i];
            }
        }
    }

    private void CheckPlaces()
    {
        bool success = true;

        for (int i = 0; i < placements.Length; i++)
        {
            var cards = placements[i].transform.GetComponentsInChildren<Card>();

            if (cards.Length > 0) {

                int targetType = targetTypeForEachPlacement[i] == -1 ? cards[0].cardType : targetTypeForEachPlacement[i];

                foreach (var card in cards)
                {
                    if(card.cardType != targetType)
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (success == false)
                break;
        }

        if (success)
            GameManager.instance.Win();
    }

}
