using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Animator turnAnimator;
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

    public bool wannaPutCardOnTheMiddle = false;
    private int indexCard = 0;
    public bool wannaUpdateCards = false;
    public bool wannaUpdateCardsOfOnePlayer = false;
    private int whichPlayer = 0;
    public List<int> positions = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        SetUsersToBoard();
        StartCoroutine(StartTheMatch());
    }

    private IEnumerator StartTheMatch()
    {
        yield return new WaitForSeconds(2);
        turnAnimator.SetInteger("Turn", GetPositionOfThePlayer(1) + 1);
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
    public void SendToServerPutCardInMiddle(int indexCard)
    {
        //TODO: Check if the card is valid card
        if (mainManager.user.userStatus == UserStatus.InTurn)
        {
            Debug.Log("Send to server the petition to put one card in the middle");
            mainManager.serializeManager.SendData(13, true, mainManager.user, indexCard);
        }
    }
    #endregion


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

    #region Utilities
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
        if(GetPositionOfThePlayer(_playerNumber) == 0 || GetPositionOfThePlayer(_playerNumber) == 2)
        {
            playerUIs[GetPositionOfThePlayer(_playerNumber)].cardGOList[_indexCard].GetComponent<CardUI>().PutTheCardOnTheMiddle();
            playerUIs[GetPositionOfThePlayer(_playerNumber)].cardGOList.RemoveAt(_indexCard);
            ResetIndicesOfCards(_playerNumber);
        }
        else
        {
            Debug.Log("The app will not instantiate this card because the card doesn't in the team");
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
    private int GetPositionOfThePlayer(int playerNumber)
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
    #endregion
}
