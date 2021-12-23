using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject middlePosition;
    public Animator turnAnimator;
    public List<PlayerUI> playerUIs;
    public List<Sprite> fotosPerfil;
    public List<Sprite> numbersPlayers;
    public MainManager mainManager;
    public GameObject panelToChooseColors;

    [Header("Cards prefabs")]
    public GameObject basicCard;
    public GameObject basicCardSum;
    public GameObject blackCard;
    public GameObject blackCardSum;
    public GameObject notFollowingCard;

    [Header("Team Colors")]
    public Color firstTeam;
    public Color SecondTeam;

    public bool wannaUpdateStates = false;
    public bool wannaPutCardOnTheMiddle = false;
    private int indexCard = 0;
    public bool wannaUpdateCards = false;
    public bool wannaUpdateCardsOfOnePlayer = false;
    private int whichPlayer = 0;
    public List<int> positions = new List<int>();
    private int indexCardToSendMiddle = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        SetUsersToBoard();
    }

    private void Update()
    {
        if (wannaUpdateCards)
        {
            UpdateCardsOfPlayersUI();
            wannaUpdateCards = false;
        }
        if (wannaUpdateCardsOfOnePlayer)
        {
            UpdateCardsOfOnePlayerUI(whichPlayer);
            wannaUpdateCardsOfOnePlayer = false;
        }
        if (wannaPutCardOnTheMiddle)
        {
            PutOneCardOnTheMiddle(indexCard, whichPlayer);
            wannaPutCardOnTheMiddle = false;
        }
        if (wannaUpdateStates)
        {
            UpdateUIByUsersStates();
            wannaUpdateStates = false;
        }
    }

    #region PetitionsToServer
    public void SendToServerGetCard()
    {
        if(mainManager.user.userStatus == UserStatus.InTurn)
        {
            Debug.Log("Send to server the petition to get a new card!");
            mainManager.serializeManager.SendData(10, true, mainManager.user);
        }
    }
    public void CheckIfCardWithIndexIsValidAndSend(int indexCard)
    {
        //TODO: Check if the card is valid card
        if (mainManager.user.userStatus == UserStatus.InTurn)
        {   
            indexCardToSendMiddle = indexCard;
            if (playerUIs[0].user.cardList[indexCard].num == mainManager.actualNumber)
            {
                Debug.Log("Send to server the petition to put one card in the middle");
                ActualiceNumberAndColor(indexCard);
                SendToServerPutCardInMiddle();
            }

            switch (mainManager.actualColor)
            {
                case 1:
                    if (playerUIs[0].user.cardList[indexCard].cardType == CardType.RedCard || playerUIs[0].user.cardList[indexCard].cardType == CardType.NotFollowingRed || playerUIs[0].user.cardList[indexCard].cardType == CardType.SumRed)
                    {
                        Debug.Log("Send to server the petition to put one card in the middle");
                        ActualiceNumberAndColor(indexCard);
                        SendToServerPutCardInMiddle();
                    }
                    break;
                case 2:
                    if (playerUIs[0].user.cardList[indexCard].cardType == CardType.BlueCard || playerUIs[0].user.cardList[indexCard].cardType == CardType.NotFollowingBlue || playerUIs[0].user.cardList[indexCard].cardType == CardType.BlueCard)
                    {
                        Debug.Log("Send to server the petition to put one card in the middle");
                        ActualiceNumberAndColor(indexCard);
                        SendToServerPutCardInMiddle();
                    }
                    break;
                case 3:
                    if (playerUIs[0].user.cardList[indexCard].cardType == CardType.YellowCard || playerUIs[0].user.cardList[indexCard].cardType == CardType.NotFollowingYellow || playerUIs[0].user.cardList[indexCard].cardType == CardType.SumYellow)
                    {
                        Debug.Log("Send to server the petition to put one card in the middle");
                        ActualiceNumberAndColor(indexCard);
                        SendToServerPutCardInMiddle();
                    }
                    break;
                case 4:
                    if (playerUIs[0].user.cardList[indexCard].cardType == CardType.GreenCard || playerUIs[0].user.cardList[indexCard].cardType == CardType.NotFollowingGreen || playerUIs[0].user.cardList[indexCard].cardType == CardType.SumGreen)
                    {
                        Debug.Log("Send to server the petition to put one card in the middle");
                        ActualiceNumberAndColor(indexCard);
                        SendToServerPutCardInMiddle();
                    }
                    break;
                default:
                    break;
            }
            //if (playerUIs[0].user.cardList[indexCard].cardType != CardType.BlackColorCard && playerUIs[0].user.cardList[indexCard].cardType != CardType.BlackSum4Card)
            //{
            //    Debug.Log("Send to server the petition to put one card in the middle");
            //    ActualiceNumberAndColor(indexCard);
            //    SendToServerPutCardInMiddle();
            //}
            //else
            //{
            //    panelToChooseColors.SetActive(true);
            //}
        }
    }
    #endregion

    #region ThreadPetitions
    public void WannaUpdateCardsOfAllPlayers()
    {
        wannaUpdateCards = true;
    }
    public void WannaUpdateCardsOfOneSinglePlayer(int _whichplayer)
    {
        whichPlayer = _whichplayer;
        wannaUpdateCardsOfOnePlayer = true;
    }
    public void WannaPutCardOnTheMiddle(int _whichplayer, int _index)
    {
        indexCard = _index;
        whichPlayer = _whichplayer;
        wannaPutCardOnTheMiddle = true;
    }
    public void WannaUpdateUIFromUserStates()
    {
        wannaUpdateStates = true;
    }
    #endregion

    #region Utilities
    private GameObject InstantiateCard(CardBase _card, GameObject where)
    {
        GameObject newCard;

        if (_card.cardType == CardType.BlackColorCard)
        {
            newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, where.transform);
            middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(newCard);
            newCard.GetComponent<BasicBlackCardUI>().SetCardUI(_card, 0);
            newCard.GetComponent<Button>().interactable = false;
        }
        else if (_card.cardType == CardType.BlackSum4Card)
        {
            newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, where.transform);
            middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(newCard);
            newCard.GetComponent<SumBlackCardUI>().SetCardUI(_card, 0);
            newCard.GetComponent<Button>().interactable = false;
        }
        else if (_card.cardType == CardType.NotFollowingBlue ||
            _card.cardType == CardType.NotFollowingGreen ||
            _card.cardType == CardType.NotFollowingRed ||
            _card.cardType == CardType.NotFollowingYellow)
        {
            newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, where.transform);
            middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(newCard);
            newCard.GetComponent<NotFollowingCardUI>().SetCardUI(_card, 0);
            newCard.GetComponent<Button>().interactable = false;
        }
        else if (_card.cardType == CardType.SumBlue ||
            _card.cardType == CardType.SumGreen ||
            _card.cardType == CardType.SumRed ||
            _card.cardType == CardType.SumYellow)
        {
            newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, where.transform);
            middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(newCard);
            newCard.GetComponent<SumNormalCardUI>().SetCardUI(_card, 0);
            newCard.GetComponent<Button>().interactable = false;
        }
        else
        {
            newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, where.transform);
            middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Add(newCard);
            newCard.GetComponent<NormalCardUI>().SetCardUI(_card, 0);
            newCard.GetComponent<Button>().interactable = false;
        }

        return newCard;
    }
    public void SendToServerPutCardInMiddle()
    {
        mainManager.serializeManager.SendData(13, true, mainManager.user, indexCardToSendMiddle);
    }
    private void SetUsersToBoard()
    {
        int numberPlayer = mainManager.user.userNumber;
        for (int i = 0; i < 4; i++)
        {
            playerUIs[i].user = mainManager.userList[numberPlayer - 1];
            positions.Add(playerUIs[i].user.userNumber);
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
    private void UpdateCardsOfPlayersUI()
    {
        for(int i = 0; i < 4; i++)
        {
            //First of all, we delete all the GameObjects of the cards
            for(int a=0; a< playerUIs[i].cardGOList.Count; a++)
            {
                Destroy(playerUIs[i].cardGOList[a]);
            }
            playerUIs[i].cardGOList.Clear();
        }

        for(int j = 0; j < playerUIs[0].user.cardList.Count; j++)
        {
            if (playerUIs[0].user.cardList[j].cardType == CardType.BlackColorCard)
            {
                GameObject newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<BasicBlackCardUI>().SetCardUI(playerUIs[0].user.cardList[j], 0);
                playerUIs[0].cardGOList.Add(newCard);
            }
            else if (playerUIs[0].user.cardList[j].cardType == CardType.BlackSum4Card)
            {
                GameObject newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<SumBlackCardUI>().SetCardUI(playerUIs[0].user.cardList[j], 0);
                playerUIs[0].cardGOList.Add(newCard);
            }
            else if (playerUIs[0].user.cardList[j].cardType == CardType.NotFollowingBlue ||
                playerUIs[0].user.cardList[j].cardType == CardType.NotFollowingGreen ||
                playerUIs[0].user.cardList[j].cardType == CardType.NotFollowingRed ||
                playerUIs[0].user.cardList[j].cardType == CardType.NotFollowingYellow)
            {
                GameObject newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<NotFollowingCardUI>().SetCardUI(playerUIs[0].user.cardList[j], 0);
                playerUIs[0].cardGOList.Add(newCard);
            }
            else if (playerUIs[0].user.cardList[j].cardType == CardType.SumBlue ||
                playerUIs[0].user.cardList[j].cardType == CardType.SumGreen ||
                playerUIs[0].user.cardList[j].cardType == CardType.SumRed ||
                playerUIs[0].user.cardList[j].cardType == CardType.SumYellow)
            {
                GameObject newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<SumNormalCardUI>().SetCardUI(playerUIs[0].user.cardList[j], 0);
                playerUIs[0].cardGOList.Add(newCard);
            }
            else
            {
                GameObject newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                newCard.GetComponent<NormalCardUI>().SetCardUI(playerUIs[0].user.cardList[j], 0);
                playerUIs[0].cardGOList.Add(newCard);
            }
        }
        for (int k = 0; k < playerUIs[2].user.cardList.Count; k++)
        {
            if (playerUIs[2].user.cardList[k].cardType == CardType.BlackColorCard)
            {
                GameObject newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[2].cardScroll.transform);
                newCard.GetComponent<BasicBlackCardUI>().SetCardUI(playerUIs[2].user.cardList[k], 2);
                playerUIs[2].cardGOList.Add(newCard);
                newCard.GetComponent<Button>().interactable = false;
            }
            else if (playerUIs[2].user.cardList[k].cardType == CardType.BlackSum4Card)
            {
                GameObject newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[2].cardScroll.transform);
                newCard.GetComponent<SumBlackCardUI>().SetCardUI(playerUIs[2].user.cardList[k], 2);
                playerUIs[2].cardGOList.Add(newCard);
                newCard.GetComponent<Button>().interactable = false;
            }
            else if (playerUIs[2].user.cardList[k].cardType == CardType.NotFollowingBlue ||
                playerUIs[2].user.cardList[k].cardType == CardType.NotFollowingGreen ||
                playerUIs[2].user.cardList[k].cardType == CardType.NotFollowingRed ||
                playerUIs[2].user.cardList[k].cardType == CardType.NotFollowingYellow)
            {
                GameObject newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[2].cardScroll.transform);
                newCard.GetComponent<NotFollowingCardUI>().SetCardUI(playerUIs[2].user.cardList[k], 2);
                playerUIs[2].cardGOList.Add(newCard);
                newCard.GetComponent<Button>().interactable = false;
            }
            else if (playerUIs[2].user.cardList[k].cardType == CardType.SumBlue ||
                playerUIs[2].user.cardList[k].cardType == CardType.SumGreen ||
                playerUIs[2].user.cardList[k].cardType == CardType.SumRed ||
                playerUIs[2].user.cardList[k].cardType == CardType.SumYellow)
            {
                GameObject newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[2].cardScroll.transform);
                newCard.GetComponent<SumNormalCardUI>().SetCardUI(playerUIs[2].user.cardList[k], 2);
                playerUIs[2].cardGOList.Add(newCard);
                newCard.GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[2].cardScroll.transform);
                newCard.GetComponent<NormalCardUI>().SetCardUI(playerUIs[2].user.cardList[k], 2);
                playerUIs[2].cardGOList.Add(newCard);
                newCard.GetComponent<Button>().interactable = false;
            }
        }
        for (int z = 0; z < 4; z++)
        {
            playerUIs[z].countofCards.text = mainManager.userList[positions[z] - 1].cardList.Count.ToString();
        }
    }
    private void UpdateCardsOfOnePlayerUI(int whichPlayer)
    {
        if(whichPlayer == positions[0] ||whichPlayer == positions[2])
        {
            //First of all, we delete all the GameObjects of the cards
            for (int k = 0; k < playerUIs[GetPositionOfThePlayer(whichPlayer)].cardGOList.Count; k++)
            {
                Destroy(playerUIs[GetPositionOfThePlayer(whichPlayer)].cardGOList[k]);
            }
            playerUIs[GetPositionOfThePlayer(whichPlayer)].cardGOList.Clear();

            for (int i = 0; i < mainManager.userList[whichPlayer - 1].cardList.Count; i++)
            {
                if (mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.BlackColorCard)
                {
                    GameObject newCard = Instantiate(blackCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                    newCard.GetComponent<BasicBlackCardUI>().SetCardUI(mainManager.userList[whichPlayer - 1].cardList[i], GetPositionOfThePlayer(whichPlayer)+1);
                    playerUIs[0].cardGOList.Add(newCard);
                }
                else if (mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.BlackSum4Card)
                {
                    GameObject newCard = Instantiate(blackCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                    newCard.GetComponent<SumBlackCardUI>().SetCardUI(mainManager.userList[whichPlayer - 1].cardList[i], GetPositionOfThePlayer(whichPlayer)+1);
                    playerUIs[0].cardGOList.Add(newCard);
                }
                else if (mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.NotFollowingBlue ||
                    mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.NotFollowingGreen ||
                    mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.NotFollowingRed ||
                    mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.NotFollowingYellow)
                {
                    GameObject newCard = Instantiate(notFollowingCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                    newCard.GetComponent<NotFollowingCardUI>().SetCardUI(mainManager.userList[whichPlayer - 1].cardList[i], GetPositionOfThePlayer(whichPlayer)+1);
                    playerUIs[0].cardGOList.Add(newCard);
                }
                else if (mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.SumBlue ||
                    mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.SumGreen ||
                    mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.SumRed ||
                    mainManager.userList[whichPlayer - 1].cardList[i].cardType == CardType.SumYellow)
                {
                    GameObject newCard = Instantiate(basicCardSum, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                    newCard.GetComponent<SumNormalCardUI>().SetCardUI(mainManager.userList[whichPlayer - 1].cardList[i], GetPositionOfThePlayer(whichPlayer)+1);
                    playerUIs[0].cardGOList.Add(newCard);
                }
                else
                {
                    GameObject newCard = Instantiate(basicCard, new Vector3(0, 0, 0), Quaternion.identity, playerUIs[0].cardScroll.transform);
                    newCard.GetComponent<NormalCardUI>().SetCardUI(mainManager.userList[whichPlayer - 1].cardList[i], GetPositionOfThePlayer(whichPlayer)+1);
                    playerUIs[0].cardGOList.Add(newCard);
                }
            }
        }
        playerUIs[GetPositionOfThePlayer(whichPlayer)].countofCards.text = mainManager.userList[whichPlayer - 1].cardList.Count.ToString();
    }
    private void PutOneCardOnTheMiddle(int _indexCard, int _playerNumber)
    {
        for(int i=0;i < middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Count; i++)
        {
            Destroy(middlePosition.GetComponent<ListOfMiddleCards>().listOfCards[i]);
        }
        middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Clear();

        GameObject newCard = InstantiateCard(playerUIs[GetPositionOfThePlayer(_playerNumber)].user.cardList[_indexCard], middlePosition);
        newCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        if (GetPositionOfThePlayer(_playerNumber) == 0 || GetPositionOfThePlayer(_playerNumber) == 2)
        {
            Destroy(playerUIs[GetPositionOfThePlayer(_playerNumber)].cardGOList[_indexCard]);
            playerUIs[GetPositionOfThePlayer(_playerNumber)].cardGOList.RemoveAt(_indexCard);
            ResetIndicesOfCards(_playerNumber);
        }

        playerUIs[GetPositionOfThePlayer(_playerNumber)].user.cardList.RemoveAt(_indexCard);
        UpdateCardsOfPlayersUI();
    }
    private void ResetIndicesOfCards(int playerNumber)
    {
        for(int i = 0; i < playerUIs[GetPositionOfThePlayer(playerNumber)].cardGOList.Count; i++)
        {
            playerUIs[GetPositionOfThePlayer(playerNumber)].cardGOList[i].GetComponent<CardUI>().indexInCardList = i;
        }
    }
    public int GetPositionOfThePlayer(int playerNumber)
    {
        for(int i = 0; i < 4; i++)
        {
            if(positions[i] == playerNumber)
            {
                return i;
            }
        }
        return 0;
    }
    private void UpdateUIByUsersStates()
    {
        turnAnimator.SetInteger("Turn", GetPositionOfThePlayer(GetUserInListByStatus(UserStatus.InTurn)+1)+1);
        wannaUpdateCards = true;
    }
    private int GetUserInListByStatus(UserStatus _status)
    {
        for(int i = 0; i < mainManager.userList.Count; i++)
        {
            if(mainManager.userList[i].userStatus == _status)
            {
                return i;
            }
        }
        return 0;
    }
    private void ActualiceNumberAndColor(int index)
    {
        CardBase card = playerUIs[0].user.cardList[index];

        if (card.cardType == CardType.RedCard || card.cardType == CardType.SumRed || card.cardType == CardType.NotFollowingRed)
        {
            mainManager.actualColor = 1;
        }
        else if (card.cardType == CardType.BlueCard || card.cardType == CardType.SumBlue || card.cardType == CardType.NotFollowingBlue)
        {
            mainManager.actualColor = 2;
        }
        else if (card.cardType == CardType.YellowCard || card.cardType == CardType.SumYellow || card.cardType == CardType.NotFollowingYellow)
        {
            mainManager.actualColor = 3;
        }
        else if (card.cardType == CardType.GreenCard || card.cardType == CardType.SumGreen || card.cardType == CardType.NotFollowingGreen)
        {
            mainManager.actualColor = 4;
        }

        //Change the actual number to check if the next card is valid
        mainManager.actualNumber = card.num;
    }
    #endregion
}
