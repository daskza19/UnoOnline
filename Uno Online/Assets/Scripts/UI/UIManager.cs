using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public List<PlayerUI> playerUIs;
    public List<Sprite> fotosPerfil;
    public GameObject playerDeck;

    [Header("Cards prefabs")]
    public GameObject basicCard;
    public GameObject basicCardSum;
    public GameObject blackCard;
    public GameObject blackCardSum;
    public GameObject notFollowingCard;


    // Start is called before the first frame update
    void Start()
    {
        playerUIs[0].imagenPerfil.sprite = fotosPerfil[2];
        playerUIs[1].imagenPerfil.sprite = fotosPerfil[3];
        playerUIs[2].imagenPerfil.sprite = fotosPerfil[4];
        playerUIs[3].imagenPerfil.sprite = fotosPerfil[14];
    }

    public void InstantiateNewCard()
    {
        int num = Random.Range(0, 10);
        CardType cardType = CardType.None;
        int randomizer = Random.Range(1, 24);

        //Not following Cards has a normal probability to appear
        if(randomizer>=1 && randomizer <= 4)
        {
            if (randomizer == 1)
                cardType = CardType.NotFollowingRed;
            else if (randomizer == 2)
                cardType = CardType.NotFollowingBlue;
            else if (randomizer == 3)
                cardType = CardType.NotFollowingGreen;
            else if (randomizer == 4)
                cardType = CardType.NotFollowingYellow;

            CardBase _newCard = new CardBase(cardType);
            GameObject newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, playerDeck.transform);
            newCard.GetComponent<NotFollowingCardUI>().SetCardUI(_newCard);
        }

        //Sum Basic Cards has a normal probability to appear
        else if (randomizer >= 5 && randomizer <= 8)
        {
            int num2 = Random.Range(1, 3);
            if (num2 == 1)
                num2 = 2;
            else
                num2 = 4;

            if (randomizer == 5)
                cardType = CardType.SumRed;
            else if (randomizer == 6)
                cardType = CardType.SumBlue;
            else if (randomizer == 7)
                cardType = CardType.SumGreen;
            else if (randomizer == 8)
                cardType = CardType.SumYellow;

            CardBase _newCard = new CardBase(cardType, num2);
            GameObject newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerDeck.transform);
            newCard.GetComponent<SumNormalCardUI>().SetCardUI(_newCard);
        }

        //Black Cards has a normal probability to appear
        else if (randomizer == 9 || randomizer == 10)
        {
            cardType = CardType.BlackColorCard;

            CardBase _newCard = new CardBase(cardType, num);
            GameObject newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, playerDeck.transform);
            newCard.GetComponent<BasicBlackCardUI>().SetCardUI(_newCard);
        }
            
        else if (randomizer == 11)
        {
            int num2 = Random.Range(1, 3);
            if (num2 == 1)
                num2 = 2;
            else
                num2 = 4;

            cardType = CardType.BlackSum4Card;

            CardBase _newCard = new CardBase(cardType, num2);
            GameObject newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerDeck.transform);
            newCard.GetComponent<SumBlackCardUI>().SetCardUI(_newCard);
        }

        //Basic Cards has a double probability to appear
        else
        {
            if (randomizer == 12 || randomizer == 13 || randomizer == 14)
                cardType = CardType.RedCard;
            else if (randomizer == 15 || randomizer == 16 || randomizer == 17)
                cardType = CardType.BlueCard;
            else if (randomizer == 18 || randomizer == 19 || randomizer == 20)
                cardType = CardType.GreenCard;
            else if (randomizer == 21 || randomizer == 22 || randomizer == 23)
                cardType = CardType.YellowCard;
            
            CardBase _newCard = new CardBase(cardType, num);
            GameObject newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, playerDeck.transform);
            newCard.GetComponent<NormalCardUI>().SetCardUI(_newCard);
        }
    }
}
