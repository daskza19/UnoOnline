using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public List<PlayerUI> playerUIs;
    public List<Sprite> fotosPerfil;
    public List<Sprite> numbersPlayers;
    public GameObject playerDeck;
    public MainManager mainManager;

    [Header("Cards prefabs")]
    public GameObject basicCard;
    public GameObject basicCardSum;
    public GameObject blackCard;
    public GameObject blackCardSum;
    public GameObject notFollowingCard;

    [Header("Team Colors")]
    public Color firstTeam;
    public Color SecondTeam;

    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        SetUsersToBoard();
    }

    private void SetUsersToBoard()
    {
        int numberPlayer = mainManager.user.userNumber;
        for (int i = 0; i < 4; i++)
        {
            playerUIs[i].user = mainManager.userList[numberPlayer - 1];
            playerUIs[i].imagenPerfil.sprite = fotosPerfil[playerUIs[i].user.userImage];
            if (playerUIs[i].user.userNumber == 1 || playerUIs[i].user.userNumber == 3)
            {
                playerUIs[i].colorUsuario.color = firstTeam;
            }
            else
            {
                playerUIs[i].colorUsuario.color = SecondTeam;
            }
            if (playerUIs[i].user.userNumber == 1) playerUIs[i].PlayerNumber.sprite = numbersPlayers[0];
            else if (playerUIs[i].user.userNumber == 2) playerUIs[i].PlayerNumber.sprite = numbersPlayers[1];
            else if (playerUIs[i].user.userNumber == 3) playerUIs[i].PlayerNumber.sprite = numbersPlayers[2];
            else playerUIs[i].PlayerNumber.sprite = numbersPlayers[3];

            numberPlayer++;
            if (numberPlayer > 4) numberPlayer = 1;
        }
    }

    public void SendToServerGetCard()
    {
        Debug.Log("Send to server the petition to get a new card!");
        mainManager.serializeManager.SendData(10, true, mainManager.user);
    }

    public void InstantiateNewCard(CardBase _card, int _playerNumber)
    {
        //If the card added is the current player viewport or the team member one, instantiate a gameobject, if not only will update the list
        if(_playerNumber == mainManager.user.userNumber ||
            _playerNumber == mainManager.user.userNumber + 2 ||
            _playerNumber == mainManager.user.userNumber - 2)
        {
            if(_card.cardType == CardType.BlackColorCard)
            {
                GameObject newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<BasicBlackCardUI>().SetCardUI(_card);
            }
            else if (_card.cardType == CardType.BlackSum4Card)
            {
                GameObject newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<SumBlackCardUI>().SetCardUI(_card);
            }
            else if (_card.cardType == CardType.NotFollowingBlue ||
                _card.cardType == CardType.NotFollowingGreen ||
                _card.cardType == CardType.NotFollowingRed ||
                _card.cardType == CardType.NotFollowingYellow)
            {
                GameObject newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<NotFollowingCardUI>().SetCardUI(_card);
            }
            else if (_card.cardType == CardType.SumBlue ||
                _card.cardType == CardType.SumGreen ||
                _card.cardType == CardType.SumRed ||
                _card.cardType == CardType.SumYellow)
            {
                GameObject newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<SumNormalCardUI>().SetCardUI(_card);
            }
            else 
            {
                GameObject newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<NormalCardUI>().SetCardUI(_card);
            }
        }
    }
}
