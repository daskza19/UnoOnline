using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerServer : MonoBehaviour
{
    [Header("Properties")]
    public UserBase user;
    public Image userImage;
    public Text userName;
    public Text userStatus;
    public Text numberofCards;
    public Text otherInfo;
    public GameObject cardsPanel;
    public List<GameObject> cardsGOList = new List<GameObject>();

    [Header("Cards prefabs")]
    public GameObject basicCard;
    public GameObject basicCardSum;
    public GameObject blackCard;
    public GameObject blackCardSum;
    public GameObject notFollowingCard;

    public void InstantiateNewCard()
    {
        for(int j = 0; j < cardsGOList.Count; j++)
        {
            Destroy(cardsGOList[j]);
        }
        cardsGOList.Clear();

        for (int i = 0; i < user.cardList.Count; i++)
        {
            CardBase cardToInstantiate = user.cardList[i];

            if (cardToInstantiate.cardType == CardType.BlackColorCard)
            {
                GameObject newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, cardsPanel.transform);
                cardsGOList.Add(newCard);
                newCard.GetComponent<BasicBlackCardUI>().SetCardUI(cardToInstantiate, 0);
                newCard.GetComponent<Button>().interactable = false;
            }
            else if (cardToInstantiate.cardType == CardType.BlackSum4Card)
            {
                GameObject newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, cardsPanel.transform);
                cardsGOList.Add(newCard);
                newCard.GetComponent<SumBlackCardUI>().SetCardUI(cardToInstantiate, 0);
                newCard.GetComponent<Button>().interactable = false;
            }
            else if (cardToInstantiate.cardType == CardType.NotFollowingBlue ||
                cardToInstantiate.cardType == CardType.NotFollowingGreen ||
                cardToInstantiate.cardType == CardType.NotFollowingRed ||
                cardToInstantiate.cardType == CardType.NotFollowingYellow)
            {
                GameObject newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, cardsPanel.transform);
                cardsGOList.Add(newCard);
                newCard.GetComponent<NotFollowingCardUI>().SetCardUI(cardToInstantiate, 0);
                newCard.GetComponent<Button>().interactable = false;
            }
            else if (cardToInstantiate.cardType == CardType.SumBlue ||
                cardToInstantiate.cardType == CardType.SumGreen ||
                cardToInstantiate.cardType == CardType.SumRed ||
                cardToInstantiate.cardType == CardType.SumYellow)
            {
                GameObject newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, cardsPanel.transform);
                cardsGOList.Add(newCard);
                newCard.GetComponent<SumNormalCardUI>().SetCardUI(cardToInstantiate, 0);
                newCard.GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, cardsPanel.transform);
                cardsGOList.Add(newCard);
                newCard.GetComponent<NormalCardUI>().SetCardUI(cardToInstantiate, 0);
                newCard.GetComponent<Button>().interactable = false;
            }
        }
    }
    public void DeleteCardInIndex(int _index)
    {
        Destroy(cardsGOList[_index]); //Delete the gameObject
        cardsGOList.RemoveAt(_index); //Delete the gameObject from list
    }
}
