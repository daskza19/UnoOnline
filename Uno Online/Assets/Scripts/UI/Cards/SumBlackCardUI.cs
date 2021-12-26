using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumBlackCardUI : CardUI
{
    [Header("UI Elements")]
    public Text blackCard_numberText1;
    public Text blackCard_numberText2;

    public override void SetCardUI(CardBase _card, int _playerNumber)
    {
        base.SetCardUI(_card, _playerNumber);
        cardBase = _card;
        blackCard_numberText1.text = "+ " + 4;
        blackCard_numberText2.text = "+ " + 4;
    }
}
