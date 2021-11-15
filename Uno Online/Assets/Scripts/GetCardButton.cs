using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCardButton : MonoBehaviour
{
    public GameObject playerDeck;
    public GameObject cardPrefab;

    public void InstantiateNewCard()
    {
        int num = Random.Range(0, 10);
        int whatis = Random.Range(1, 6);
        CardBase _newCard = new CardBase((CardType)whatis, num);

        GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, playerDeck.transform);
        newCard.GetComponent<CardUI>().SetCardUI(_newCard);
    }
}
