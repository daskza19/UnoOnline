using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    None,
    YellowCard,
    RedCard,
    BlueCard,
    GreenCard,
    BlackColorCard,
    BlackSum4Card,
    NotFollowingBlue,
    NotFollowingYellow,
    NotFollowingGreen,
    NotFollowingRed,
    SumBlue,
    SumYellow,
    SumGreen,
    SumRed
}

[System.Serializable]
public class CardBase
{
    public CardType cardType = CardType.None;
    public int num = 0;
    public int card_id = 0;

    public CardBase (CardType _type, int _num = 0)
    {
        cardType = _type;
        num = _num;
        card_id = Random.Range(0, int.MaxValue);
    }

    public virtual void DoAction()
    {
        Debug.Log("Acton DONE");
    }
}
