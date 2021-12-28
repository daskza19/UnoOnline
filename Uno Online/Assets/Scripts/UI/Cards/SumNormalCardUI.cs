using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumNormalCardUI : CardUI
{
    [Header("UI Elements")]
    public Image cardColor;
    public Text normalCard_numberText1;
    public Text normalCard_numberText2;
    public Image panelCard1;
    public Image panelCard2;

    public override void SetCardUI(CardBase _card, int _playerNumber)
    {
        base.SetCardUI(_card, _playerNumber);
        cardBase = _card;
        normalCard_numberText1.text = "+ " + _card.num.ToString();
        normalCard_numberText2.text = "+ " + _card.num.ToString();

        switch (_card.cardType)
        {
            case (CardType.SumBlue):
                cardColor.color = blueColor;
                panelCard1.color = blueColor;
                panelCard2.color = blueColor;
                break;
            case (CardType.SumYellow):
                cardColor.color = yellowColor;
                panelCard1.color = yellowColor;
                panelCard2.color = yellowColor;
                break;
            case (CardType.SumGreen):
                cardColor.color = greenColor;
                panelCard1.color = greenColor;
                panelCard2.color = greenColor;
                break;
            case (CardType.SumRed):
                cardColor.color = redColor;
                panelCard1.color = redColor;
                panelCard2.color = redColor;
                break;
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
