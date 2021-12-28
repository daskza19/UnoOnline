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
    public ServerManager serverManager;
    public GameObject panelToChooseColors;
    public EndPanelContainer endPanel;
    public Button unoButton;

    [Header("Cards prefabs")]
    public GameObject basicCard;
    public GameObject basicCardSum;
    public GameObject blackCard;
    public GameObject blackCardSum;
    public GameObject notFollowingCard;

    [Header("Team Colors")]
    public Color firstTeam;
    public Color SecondTeam;

    [Header("Others")]
    public Text colorText;
    public Text numberText;
    public Text alreadyCardMiddle;

    public bool wannaUpdateStates = false;
    public bool wannaPutCardOnTheMiddle = false;
    private int indexCard = 0;
    public bool wannaUpdateCards = false;
    public bool wannaUpdateCardsOfOnePlayer = false;
    private int whichPlayer = 0;
    public List<int> positions = new List<int>();
    private int indexCardToSendMiddle = 0;
    public int whichPlayerHasOneCard = 0;


    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        SetUsersToBoard();
        unoButton.interactable = false;
    }

    private void Update()
    {
        if (mainManager.actualColor == 1) colorText.text = "Red";
        if (mainManager.actualColor == 2) colorText.text = "Blue";
        if (mainManager.actualColor == 3) colorText.text = "Yellow";
        if (mainManager.actualColor == 4) colorText.text = "Green";
        numberText.text = mainManager.actualNumber.ToString();
        alreadyCardMiddle.text = mainManager.alreadyPutCardInMiddle.ToString();

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
        for (int i = 0; i < mainManager.userList.Count; i++)
        {
            if (mainManager.userList[i].cardList.Count == 1)
            {
                unoButton.interactable = true;
                whichPlayerHasOneCard = i;
            }
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
    public void SendToServerUNOPressed()
    {
        Debug.Log("UNO!");
        mainManager.serializeManager.SendData(15, true, mainManager.user, whichPlayerHasOneCard);
        unoButton.interactable = false;
    }

    private bool IsCardValid(CardBase _card)
    {
        //If the card have the same number than the actual
        if (_card.num == mainManager.actualNumber) return true;
        switch (mainManager.actualColor)
        {
            case 1:
                if (_card.cardType == CardType.RedCard || _card.cardType == CardType.NotFollowingRed || _card.cardType == CardType.SumRed)
                {
                    return true;
                }
                break;
            case 2:
                if (_card.cardType == CardType.BlueCard || _card.cardType == CardType.NotFollowingBlue || _card.cardType == CardType.SumBlue)
                {
                    return true;
                }
                break;
            case 3:
                if (_card.cardType == CardType.YellowCard || _card.cardType == CardType.NotFollowingYellow || _card.cardType == CardType.SumYellow)
                {
                    return true;
                }
                break;
            case 4:
                if (_card.cardType == CardType.GreenCard || _card.cardType == CardType.NotFollowingGreen || _card.cardType == CardType.SumGreen)
                {
                    return true;
                }
                break;
            default:
                break;
        }

        return false;
    }

    public void CheckIfCardWithIndexIsValidAndSend(int indexCard)
    {
        if (mainManager.user.userStatus == UserStatus.InTurn && mainManager.alreadyPutCardInMiddle == false)
        {   
            indexCardToSendMiddle = indexCard;

            if (IsCardValid(playerUIs[0].user.cardList[indexCard]) == true)
            {
                Debug.Log("Send to server the petition to put one card in the middle");
                mainManager.alreadyPutCardInMiddle = true;
                ActualiceNumberAndColor(indexCard);
                SendToServerPutCardInMiddle();
            }
            else if (playerUIs[0].user.cardList[indexCard].cardType == CardType.BlackColorCard || playerUIs[0].user.cardList[indexCard].cardType == CardType.BlackSum4Card)
            {
                mainManager.alreadyPutCardInMiddle = true;
                ActualiceNumberAndColor(indexCard);
                panelToChooseColors.SetActive(true);
            }
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
        //First of all, we delete all the GameObjects of the cards
        for (int i = 0; i < 4; i++)
        {
            for(int a=0; a< playerUIs[i].cardGOList.Count; a++)
            {
                Destroy(playerUIs[i].cardGOList[a]);
            }
            playerUIs[i].cardGOList.Clear();
        }

        //If the current user view is not null, instantiate the cards
        if (playerUIs[0].user != null)
        {
            for (int j = 0; j < playerUIs[0].user.cardList.Count; j++)
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
        }

        //If the other team member of the player view is not null, instantiate the cards
        if (playerUIs[2].user.userStatus != UserStatus.Disconnected)
        {
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
        }

        //For all the players, change the text of the amount of cards
        for (int z = 0; z < 4; z++)
        {
            if(playerUIs[z].user.userStatus != UserStatus.Disconnected)
            {
                playerUIs[z].countofCards.text = playerUIs[z].user.cardList.Count.ToString();
            }
            else
            {
                playerUIs[z].countofCards.text = "0";
            }
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
        //IF the user is null, exit the function
        if (mainManager.userList[_playerNumber - 1] == null)
            return;

        //First we clean all the gameobjects from the UI
        for(int i=0;i < middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Count; i++)
        {
            Destroy(middlePosition.GetComponent<ListOfMiddleCards>().listOfCards[i]);
        }
        middlePosition.GetComponent<ListOfMiddleCards>().listOfCards.Clear();

        //After that, we instantiate the new card on the middle and put in the correct place
        GameObject newCard = InstantiateCard(playerUIs[GetPositionOfThePlayer(_playerNumber)].user.cardList[_indexCard], middlePosition);
        newCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        //After put the card on the middle, if the player is in a current team view, delete the gamobject from the list
        if (GetPositionOfThePlayer(_playerNumber) == 0 || GetPositionOfThePlayer(_playerNumber) == 2)
        {
            Destroy(playerUIs[GetPositionOfThePlayer(_playerNumber)].cardGOList[_indexCard]);
            playerUIs[GetPositionOfThePlayer(_playerNumber)].cardGOList.RemoveAt(_indexCard);
            ResetIndicesOfCards(_playerNumber);
        }

        //Remove the card from the intern card list of the player
        playerUIs[GetPositionOfThePlayer(_playerNumber)].user.cardList.RemoveAt(_indexCard);
        UpdateCardsOfPlayersUI();

        //Check if the user that put the card on the middle stay with 0 card. If it is, show the final panel and end the match
        if (playerUIs[GetPositionOfThePlayer(_playerNumber)].user.cardList.Count == 0)
        {
            for(int z = 0; z < mainManager.userList.Count; z++)
            {
                mainManager.userList[z].userStatus = UserStatus.Waiting;
            }
            turnAnimator.SetInteger("Turn", 0);
            if(_playerNumber == 1 || _playerNumber == 3)
            {
                endPanel.mainTitle.text = "The winner of this match is... The red team!";
                endPanel.firstUserImage.sprite = fotosPerfil[mainManager.userList[0].userImage];
                endPanel.firstUserName.text = mainManager.userList[0].userName;
                endPanel.secondUserImage.sprite = fotosPerfil[mainManager.userList[2].userImage];
                endPanel.secondUserName.text = mainManager.userList[2].userName;
            }
            else
            {
                endPanel.mainTitle.text = "The winner of this match is... The blue team!";
                endPanel.firstUserImage.sprite = fotosPerfil[mainManager.userList[1].userImage];
                endPanel.firstUserName.text = mainManager.userList[1].userName;
                endPanel.secondUserImage.sprite = fotosPerfil[mainManager.userList[3].userImage];
                endPanel.secondUserName.text = mainManager.userList[3].userName;
            }
            endPanel.animator.SetTrigger("EndMatch");
            StartCoroutine(EndMatch());
        }
    }

    private IEnumerator EndMatch()
    {
        yield return new WaitForSeconds(4);
        Application.Quit();
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
    public int GetPlayerWithOneCard()
    {
        return whichPlayerHasOneCard;
    }
    private void UpdateUIByUsersStates()
    {
        int whoIsTurn = 0;
        for(int z = 0; z < 4;z ++)
        {
            if (mainManager.userList[z] != null)
            {
                if (mainManager.userList[z].userStatus == UserStatus.InTurn)
                {
                    whoIsTurn = mainManager.userList[z].userNumber;
                }
                if (mainManager.userList[z].userStatus == UserStatus.Disconnected)
                {
                    int pos = GetPositionOfThePlayer(mainManager.userList[z].userNumber);
                    playerUIs[pos].imagenPerfil.sprite = fotosPerfil[12];
                    for (int j = 0; j < playerUIs[pos].cardGOList.Count; j++)
                    {
                        Destroy(playerUIs[pos].cardGOList[j]);
                    }
                    playerUIs[pos].cardGOList.Clear();
                    playerUIs[pos].countofCards.text = "0";
                }
            }
        }
        turnAnimator.SetInteger("Turn", GetPositionOfThePlayer(whoIsTurn)+1);
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
