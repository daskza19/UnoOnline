using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CardUI : MonoBehaviour
{
    [Header("Other")]
    public CardBase cardBase;
    public GameObject middlePosition;


    [Header("Card settings (global)")]
    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public Color yellowColor = Color.yellow;
    public Color greenColor = Color.green;
    public Color blackColor = Color.black;

    public virtual void SetCardUI(CardBase _card)
    {
        Debug.Log("Card base set");
        middlePosition = GameObject.Find("PanelMiddle");
    }

    public void PutNewCardOnMiddle(int _card_id)
    {
        gameObject.transform.SetParent(middlePosition.transform);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(37.5f, -55);
        middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(gameObject);
    }
}