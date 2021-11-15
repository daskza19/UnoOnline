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
    BlackSum4Card,
    BlackColorCard
}

[System.Serializable]
public class CardBase
{
    public CardType cardType = CardType.None;
    public int num = 0;

    public CardBase (CardType _type, int num = 0)
    {
        cardType = _type;
        num = 0;
    }

    public virtual void DoAction()
    {
        Debug.Log("Acton DONE");
    }
}
