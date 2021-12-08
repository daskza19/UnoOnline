using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumBlackCardUI : CardUI
{
    [Header("UI Elements")]
    public Text numberText;

    public override void SetCardUI(CardBase _card, int _playerNumber)
    {
        base.SetCardUI(_card, _playerNumber);
        cardBase = _card;
        numberText.text = "+ " + _card.num.ToString();
    }
}
