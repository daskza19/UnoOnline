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
    public GameObject uiManager;
    public int indexInCardList = 0;

    [Header("Card settings (global)")]
    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public Color yellowColor = Color.yellow;
    public Color greenColor = Color.green;
    public Color blackColor = Color.black;

    public virtual void SetCardUI(CardBase _card, int playerPosition)
    {
        Debug.Log("Card base set");
        middlePosition = GameObject.Find("PanelMiddle");
        uiManager = GameObject.Find("UIManager");
        if (uiManager != null)
        {
            indexInCardList = uiManager.GetComponent<UIManager>().playerUIs[playerPosition].cardGOList.Count;
        }
    }

    public void PutTheCardOnTheMiddle()
    {
        gameObject.transform.SetParent(middlePosition.transform);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(37.5f, -55);
        middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(gameObject);
    }

    public void SendPetitionToPutCardOnMiddle()
    {
        if(uiManager == null)
        {
            Debug.Log("UIManager is null in this card!");
            return;
        }

        uiManager.GetComponent<UIManager>().SendToServerPutCardInMiddle(indexInCardList);
    }
}