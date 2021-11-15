using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image cardColor;
    public Text numberText;

    [Header("Other")]
    public CardBase cardBase;

    [Header("Card settings (global)")]
    public Color blueColor;
    public Color redColor;
    public Color yellowColor;
    public Color greenColor;
    public Color blackColor;

    public void SetCardUI(CardBase _card)
    {
        cardBase = _card;
        numberText.text = _card.num.ToString();

        switch (_card.cardType)
        {
            case (CardType.BlueCard):
                cardColor.color = blueColor;
                break;
            case (CardType.YellowCard):
                cardColor.color = yellowColor;
                break;
            case (CardType.GreenCard):
                cardColor.color = greenColor;
                break;
            case (CardType.RedCard):
                cardColor.color = redColor;
                break;
            case (CardType.BlackColorCard):
                cardColor.color = blackColor;
                break;
            case (CardType.BlackSum4Card):
                cardColor.color = blackColor;
                break;
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
