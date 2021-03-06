using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotFollowingCardUI : CardUI
{
    [Header("UI Elements")]
    public Image cardColor;
    public Image prohibitedImage;

    public override void SetCardUI(CardBase _card, int _playerNumber)
    {
        base.SetCardUI(_card, _playerNumber);
        cardBase = _card;

        switch (_card.cardType)
        {
            case (CardType.NotFollowingBlue):
                cardColor.color = blueColor;
                prohibitedImage.color = blueColor;
                break;
            case (CardType.NotFollowingYellow):
                cardColor.color = yellowColor;
                prohibitedImage.color = yellowColor;
                break;
            case (CardType.NotFollowingGreen):
                cardColor.color = greenColor;
                prohibitedImage.color = greenColor;
                break;
            case (CardType.NotFollowingRed):
                cardColor.color = redColor;
                prohibitedImage.color = redColor;
                break;
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
