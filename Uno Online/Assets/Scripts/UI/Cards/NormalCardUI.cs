using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalCardUI : CardUI
{
    [Header("UI Elements")]
    public Image cardColor;
    public Text numberText;

    public override void SetCardUI(CardBase _card, int playerPosition)
    {
        base.SetCardUI(_card, playerPosition);
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
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
