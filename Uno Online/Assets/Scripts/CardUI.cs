using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CardUI : MonoBehaviour
{
    public Image cardColor;
    public Text numberText;

    public Color blueColor;
    public Color redColor;
    public Color yellowColor;
    public Color greenColor;
    public Color blackColor;

    public void SetCardUI(CardType _type, int _num = 0)
    {
        switch (_type)
        {
            case (CardType.BlueCard):

                break;
            case (CardType.YellowCard):

                break;
            case (CardType.GreenCard):

                break;
            case (CardType.RedCard):

                break;
            case (CardType.BlackColorCard):

                break;
            case (CardType.BlackSum4Card):

                break;
            default:
                Debug.Log("Card received has NONE type");
                break;
        }
    }
}
