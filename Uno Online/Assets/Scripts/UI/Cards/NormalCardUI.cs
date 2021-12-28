using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalCardUI : CardUI
{
    [Header("UI Elements")]
    public Image cardColor;
    public Text numberText;
    public Text smallnumberText1;
    public Text smallnumberText2;

    public override void SetCardUI(CardBase _card, int playerPosition)
    {
        base.SetCardUI(_card, playerPosition);
        cardBase = _card;
        numberText.text = _card.num.ToString();
        smallnumberText1.text = _card.num.ToString();
        smallnumberText2.text = _card.num.ToString();

        switch (_card.cardType)
        {
            case (CardType.BlueCard):
                cardColor.color = blueColor;
                numberText.color = blueColor;
                break;
            case (CardType.YellowCard):
                cardColor.color = yellowColor;
                numberText.color = yellowColor;
                break;
            case (CardType.GreenCard):
                cardColor.color = greenColor;
                numberText.color = greenColor;
                break;
            case (CardType.RedCard):
                cardColor.color = redColor;
                numberText.color = redColor;
                break;
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
