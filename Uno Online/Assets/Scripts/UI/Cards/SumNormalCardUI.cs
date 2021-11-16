using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumNormalCardUI : CardUI
{
    [Header("UI Elements")]
    public Image cardColor;
    public Text numberText;

    public override void SetCardUI(CardBase _card)
    {
        base.SetCardUI(_card);
        cardBase = _card;
        numberText.text = "+ " + _card.num.ToString();

        switch (_card.cardType)
        {
            case (CardType.SumBlue):
                cardColor.color = blueColor;
                break;
            case (CardType.SumYellow):
                cardColor.color = yellowColor;
                break;
            case (CardType.SumGreen):
                cardColor.color = greenColor;
                break;
            case (CardType.SumRed):
                cardColor.color = redColor;
                break;
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
