using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour
{
    #region Variables
    [Header("Server Properties")]
    public int host = 1818;
    public string password = "1919";

    [Header("Players Properties")]
    public List<UserBase> userList;

    [Header("Game Properties")]
    public int lastPlayerWithOneCard = 0;
    public int actualColor = 4;
    public int actualNumber = 8;
    private float startTime;
    public bool isInGame = false;
    public int gameTurn = 0;
    public bool isClock = true;

    [Header("UI Things")]
    public List<Sprite> spritesPerfil;
    public List<PlayerServer> playerProperties;
    public List<Text> gameProperties;

    private SerializeManager serializeManager;
    public Socket newSocket;
    public IPEndPoint ipep;
    public EndPoint temporalEndPoint;
    public List<EndPoint> sendEnp;
    public List<EndPoint> recuperateList;
    public Thread mainThread;
    public Thread checkThread;

    public bool wannaUpdateInfo = false;
    private bool wannaStartTimer = false;
    public int whatToDo = 1000;

    private List<int> numberNewCard = new List<int>();
    private List<int> randomizerNewCard = new List<int>();
    private List<int> number2NewCard = new List<int>();
    private bool wannaRandomNumbers;
    #endregion

    #region UnityFunctions
    void Start()
    {
        wannaRandomNumbers = true;
        serializeManager = GetComponent<SerializeManager>();
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, host);
        temporalEndPoint = (EndPoint)ipep;
        sendEnp = new List<EndPoint>();

        newSocket.Bind(ipep);

        for (int i = 0; i < playerProperties.Count; i++)
        {
            playerProperties[i].userStatus.text = "DISCONNECTED";
            playerProperties[i].userStatus.color = Color.red;
        }

        mainThread = new Thread(ReceiveUsersLoop);
        mainThread.Start();
        checkThread = new Thread(CheckUsersLoop);
        checkThread.Start();
    }
    private void Update()
    {
        if (wannaRandomNumbers)
        {
            numberNewCard.Clear();
            randomizerNewCard.Clear();
            number2NewCard.Clear();

            for (int i = 0; i < 100; i++)
            {
                int num1 = UnityEngine.Random.Range(0, 10);
                int num2 = UnityEngine.Random.Range(1, 24);
                int num3 = UnityEngine.Random.Range(1, 3);
                numberNewCard.Add(num1);
                randomizerNewCard.Add(num2);
                number2NewCard.Add(num3);
            }

            wannaRandomNumbers = false;
        }

        if (isInGame)
        {
            if (wannaStartTimer)
            {
                startTime = Time.time;
                wannaStartTimer = false;
            }
            float t = Time.time - startTime;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");
            gameProperties[5].text = "Match Time: " + minutes + ":" + seconds;
        }

        if (wannaUpdateInfo)
        {
            UpdateUserUI();
            UpdateGameUI();
            wannaUpdateInfo = false;
        }
    }
    #endregion

    #region Threads
    private void ReceiveUsersLoop()
    {
        while (true)
        {
            while (userList.Count < 4 || !CheckAllUsersState(UserStatus.Connected))
            {
                whatToDo = serializeManager.ReceiveData(false);
                wannaUpdateInfo = true;
            }
            isInGame = true;
            Debug.Log("All players DONE!");
            Thread.Sleep(100);
            wannaStartTimer = true;

            serializeManager.SendData(4, false); //Send to the other players that the match is going to start (to change their scenes)

            Thread.Sleep(500);
            SumToAllPlayerNumberCards(5); //Put the first cards to all the players

            Thread.Sleep(250);
            WhoIsNext();
            serializeManager.SendData(14, false);

            while (isInGame) //Start the gameloop
            {
                if (CheckAllUsersState(UserStatus.Disconnected))
                {
                    Debug.Log("Game ended!!");
                    isInGame = false;
                    Thread.Sleep(100);
                    userList.Clear();
                    temporalEndPoint = (EndPoint)ipep;
                    sendEnp.Clear();
                    continue;
                }
                whatToDo = serializeManager.ReceiveData(false);
                wannaUpdateInfo = true;
            }
        }
    }
    private void CheckUsersLoop()
    {
        //while (true)
        //{
        //    serializeManager.SendData(7, false);
        //
        //    Thread.Sleep(5);
        //
        //    whatToDo = serializeManager.ReceiveData(false);
        //    wannaUpdateInfo = true;
        //}
    }
    #endregion

    #region Utilities
    public int WhoIsNext()
    {
        //Actual turn is a int that says the player number that is going to be able to do an action
        UserBase user = new UserBase("", 0, 15);
        while (user.userStatus==UserStatus.Disconnected)
        {
            if (isClock) gameTurn++;
            else if (!isClock) gameTurn--;

            if (gameTurn < 0) gameTurn = 3;
            else if (gameTurn >= 4) gameTurn = 0;

            user = userList[gameTurn];
        }

        for(int i = 0; i < 4; i++)
        {
            if(userList[i].userStatus != UserStatus.Disconnected)
            {
                userList[i].userStatus = UserStatus.Waiting;
            }
        }
        userList[gameTurn].userStatus = UserStatus.InTurn;
        wannaUpdateInfo = true;
        return gameTurn;
    }
    public bool CheckAllUsersState(UserStatus _status)
    {
        if (userList.Count != 4)
            return false;

        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (userList[i].userStatus == _status) count++;
        }

        if (count == 4) return true;
        return false;
    }
    public void CreateNewRandomCard(int playerNumber)
    {
        CardBase _newCard = new CardBase(CardType.None);

        //Not following Cards has a normal probability to appear
        if (randomizerNewCard[0] >= 1 && randomizerNewCard[0] <= 4)
        {
            if (randomizerNewCard[0] == 1)
                _newCard.cardType = CardType.NotFollowingRed;
            else if (randomizerNewCard[0] == 2)
                _newCard.cardType = CardType.NotFollowingBlue;
            else if (randomizerNewCard[0] == 3)
                _newCard.cardType = CardType.NotFollowingGreen;
            else if (randomizerNewCard[0] == 4)
                _newCard.cardType = CardType.NotFollowingYellow;
        }

        //Sum Basic Cards has a normal probability to appear
        else if (randomizerNewCard[0] >= 5 && randomizerNewCard[0] <= 8)
        {
            if (number2NewCard[0] == 1)
                number2NewCard[0] = 2;
            else
                number2NewCard[0] = 4;

            if (randomizerNewCard[0] == 5)
                _newCard.cardType = CardType.SumRed;
            else if (randomizerNewCard[0] == 6)
                _newCard.cardType = CardType.SumBlue;
            else if (randomizerNewCard[0] == 7)
                _newCard.cardType = CardType.SumGreen;
            else if (randomizerNewCard[0] == 8)
                _newCard.cardType = CardType.SumYellow;

            _newCard.num = number2NewCard[0];
        }

        //Black Cards has a normal probability to appear
        else if (randomizerNewCard[0] == 9 || randomizerNewCard[0] == 10)
        {
            _newCard.cardType = CardType.BlackColorCard;
        }
        else if (randomizerNewCard[0] == 11)
        {
            if (number2NewCard[0] == 1)
                number2NewCard[0] = 2;
            else
                number2NewCard[0] = 4;

            _newCard.cardType = CardType.BlackSum4Card;
            _newCard.num = number2NewCard[0];
        }

        //Basic Cards has a double probability to appear
        else
        {
            if (randomizerNewCard[0] == 12 || randomizerNewCard[0] == 13 || randomizerNewCard[0] == 14)
                _newCard.cardType = CardType.RedCard;
            else if (randomizerNewCard[0] == 15 || randomizerNewCard[0] == 16 || randomizerNewCard[0] == 17)
                _newCard.cardType = CardType.BlueCard;
            else if (randomizerNewCard[0] == 18 || randomizerNewCard[0] == 19 || randomizerNewCard[0] == 20)
                _newCard.cardType = CardType.GreenCard;
            else if (randomizerNewCard[0] == 21 || randomizerNewCard[0] == 22 || randomizerNewCard[0] == 23)
                _newCard.cardType = CardType.YellowCard;

            _newCard.num = numberNewCard[0];
        }

        Debug.Log("Created a new Card: " + _newCard.cardType.ToString() + " with this number: " + _newCard.num);

        userList[playerNumber-1].cardList.Add(_newCard);
        wannaRandomNumbers = true;
        wannaUpdateInfo = true;
    }
    public void SumToAllPlayerNumberCards(int numberCardsToSum)
    {
        int count = 0;
        for(int i = 0; i < userList.Count; i++)//Do the loop for all players
        {
            if (userList[i] == null || userList[i].userName == "") //If he player is null in the list or has been deleted not sum the cards
                continue;

            for(int j = 0; j < numberCardsToSum; j++) //Do the loop x times to get the new Cards
            {
                CardBase _newCard = new CardBase(CardType.None);

                //Not following Cards has a normal probability to appear
                if (randomizerNewCard[count] >= 1 && randomizerNewCard[count] <= 4)
                {
                    if (randomizerNewCard[count] == 1)
                        _newCard.cardType = CardType.NotFollowingRed;
                    else if (randomizerNewCard[count] == 2)
                        _newCard.cardType = CardType.NotFollowingBlue;
                    else if (randomizerNewCard[count] == 3)
                        _newCard.cardType = CardType.NotFollowingGreen;
                    else if (randomizerNewCard[count] == 4)
                        _newCard.cardType = CardType.NotFollowingYellow;
                }

                //Sum Basic Cards has a normal probability to appear
                else if (randomizerNewCard[count] >= 5 && randomizerNewCard[count] <= 8)
                {
                    if (number2NewCard[count] == 1)
                        number2NewCard[count] = 2;
                    else
                        number2NewCard[count] = 4;

                    if (randomizerNewCard[count] == 5)
                        _newCard.cardType = CardType.SumRed;
                    else if (randomizerNewCard[count] == 6)
                        _newCard.cardType = CardType.SumBlue;
                    else if (randomizerNewCard[count] == 7)
                        _newCard.cardType = CardType.SumGreen;
                    else if (randomizerNewCard[count] == 8)
                        _newCard.cardType = CardType.SumYellow;

                    _newCard.num = number2NewCard[count];
                }

                //Black Cards has a normal probability to appear
                else if (randomizerNewCard[count] == 9 || randomizerNewCard[count] == 10)
                {
                    _newCard.cardType = CardType.BlackColorCard;
                }
                else if (randomizerNewCard[count] == 11)
                {
                    if (number2NewCard[count] == 1)
                        number2NewCard[count] = 2;
                    else
                        number2NewCard[count] = 4;

                    _newCard.cardType = CardType.BlackSum4Card;
                    _newCard.num = number2NewCard[count];
                }

                //Basic Cards has a double probability to appear
                else
                {
                    if (randomizerNewCard[count] == 12 || randomizerNewCard[count] == 13 || randomizerNewCard[count] == 14)
                        _newCard.cardType = CardType.RedCard;
                    else if (randomizerNewCard[count] == 15 || randomizerNewCard[count] == 16 || randomizerNewCard[count] == 17)
                        _newCard.cardType = CardType.BlueCard;
                    else if (randomizerNewCard[count] == 18 || randomizerNewCard[count] == 19 || randomizerNewCard[count] == 20)
                        _newCard.cardType = CardType.GreenCard;
                    else if (randomizerNewCard[count] == 21 || randomizerNewCard[count] == 22 || randomizerNewCard[count] == 23)
                        _newCard.cardType = CardType.YellowCard;

                    _newCard.num = numberNewCard[count];
                }
                userList[i].cardList.Add(_newCard); //Add the new Card to the user
                count++;
            }
        }
        wannaUpdateInfo = true;
        wannaRandomNumbers = true;
        serializeManager.SendData(12, false); //Send all the new lists to all the players
    }

    public void SumToOnePlayerCards(int numberCardsToSum, int whichPlayer)
    {
        int count = 0;
        for (int j = 0; j < numberCardsToSum; j++) //Do the loop x times to get the new Cards
        {
            CardBase _newCard = new CardBase(CardType.None);

            //Not following Cards has a normal probability to appear
            if (randomizerNewCard[count] >= 1 && randomizerNewCard[count] <= 4)
            {
                if (randomizerNewCard[count] == 1)
                    _newCard.cardType = CardType.NotFollowingRed;
                else if (randomizerNewCard[count] == 2)
                    _newCard.cardType = CardType.NotFollowingBlue;
                else if (randomizerNewCard[count] == 3)
                    _newCard.cardType = CardType.NotFollowingGreen;
                else if (randomizerNewCard[count] == 4)
                    _newCard.cardType = CardType.NotFollowingYellow;
            }

            //Sum Basic Cards has a normal probability to appear
            else if (randomizerNewCard[count] >= 5 && randomizerNewCard[count] <= 8)
            {
                if (number2NewCard[count] == 1)
                    number2NewCard[count] = 2;
                else
                    number2NewCard[count] = 4;

                if (randomizerNewCard[count] == 5)
                    _newCard.cardType = CardType.SumRed;
                else if (randomizerNewCard[count] == 6)
                    _newCard.cardType = CardType.SumBlue;
                else if (randomizerNewCard[count] == 7)
                    _newCard.cardType = CardType.SumGreen;
                else if (randomizerNewCard[count] == 8)
                    _newCard.cardType = CardType.SumYellow;

                _newCard.num = number2NewCard[count];
            }

            //Black Cards has a normal probability to appear
            else if (randomizerNewCard[count] == 9 || randomizerNewCard[count] == 10)
            {
                _newCard.cardType = CardType.BlackColorCard;
            }
            else if (randomizerNewCard[count] == 11)
            {
                if (number2NewCard[count] == 1)
                    number2NewCard[count] = 2;
                else
                    number2NewCard[count] = 4;

                _newCard.cardType = CardType.BlackSum4Card;
                _newCard.num = number2NewCard[count];
            }

            //Basic Cards has a double probability to appear
            else
            {
                if (randomizerNewCard[count] == 12 || randomizerNewCard[count] == 13 || randomizerNewCard[count] == 14)
                    _newCard.cardType = CardType.RedCard;
                else if (randomizerNewCard[count] == 15 || randomizerNewCard[count] == 16 || randomizerNewCard[count] == 17)
                    _newCard.cardType = CardType.BlueCard;
                else if (randomizerNewCard[count] == 18 || randomizerNewCard[count] == 19 || randomizerNewCard[count] == 20)
                    _newCard.cardType = CardType.GreenCard;
                else if (randomizerNewCard[count] == 21 || randomizerNewCard[count] == 22 || randomizerNewCard[count] == 23)
                    _newCard.cardType = CardType.YellowCard;

                _newCard.num = numberNewCard[count];
            }
            userList[whichPlayer].cardList.Add(_newCard); //Add the new Card to the user
            count++;
        }
        wannaUpdateInfo = true;
        wannaRandomNumbers = true;
        serializeManager.SendData(12, false); //Send all the new lists to all the players
    }

    public void NextTurn()
    {
        WhoIsNext();
        serializeManager.SendData(14, false);
        wannaUpdateInfo = true;
    }
    #endregion

    #region UpdateUI
    public void UpdateUserUI()
    {
        for(int i = 0; i < userList.Count; i++)
        {
            if (userList[i] != null) playerProperties[i].user = userList[i];
            if (userList[i].userStatus == UserStatus.Disconnected)
            {
                PutOneUserDisconnected(i);
                continue;
            }
            playerProperties[i].userName.text = userList[i].userName;
            playerProperties[i].userImage.sprite = spritesPerfil[userList[i].userImage];
            playerProperties[i].numberofCards.text = "Number of cards: " + userList[i].cardList.Count.ToString();
            playerProperties[i].InstantiateNewCard();
            switch (userList[i].userStatus)
            {
                case (UserStatus.Connected):
                    playerProperties[i].userStatus.text = "CONNECTED";
                    playerProperties[i].userStatus.color = Color.green;
                    break;
                case (UserStatus.Ready):
                    playerProperties[i].userStatus.text = "READY";
                    playerProperties[i].userStatus.color = Color.yellow;
                    break;
                case (UserStatus.Waiting):
                    playerProperties[i].userStatus.text = "WAITING";
                    playerProperties[i].userStatus.color = Color.yellow;
                    break;
                case (UserStatus.InTurn):
                    playerProperties[i].userStatus.text = "IN TURN";
                    playerProperties[i].userStatus.color = Color.blue;
                    break;
                default:
                    playerProperties[i].userStatus.text = "DISCONNECTED";
                    playerProperties[i].userStatus.color = Color.green;
                    break;
            }
        }
    }
    public void UpdateGameUI()
    {
        string actualColorString = "None";
        if (actualColor == 1) actualColorString = "Red";
        if (actualColor == 2) actualColorString = "Blue";
        if (actualColor == 3) actualColorString = "Yellow";
        if (actualColor == 4) actualColorString = "Green";
        gameProperties[0].text = "Actual Color: " + actualColorString;
        gameProperties[1].text = "Actual Number: " + actualNumber.ToString();

        gameProperties[2].text = "Is in game: " + isInGame.ToString();
        gameProperties[3].text = "Player Turn: " + (gameTurn+1).ToString();
        gameProperties[4].text = "Is clockwise: " + isClock.ToString();
        gameProperties[6].text = "Last Player With One Card: " + lastPlayerWithOneCard.ToString();
    }
    public void PutOneUserDisconnected(int _index)
    {
        playerProperties[_index].userName.text = "NO USER";
        playerProperties[_index].userImage.sprite = spritesPerfil[12];
        playerProperties[_index].userStatus.text = "DISCONNECTED";
        playerProperties[_index].userStatus.color = Color.red;
        playerProperties[_index].numberofCards.text = "Number of cards: 0";

        for(int i = 0; i < playerProperties[_index].cardsGOList.Count; i++)
        {
            Destroy(playerProperties[_index].cardsGOList[i]);
        }
        playerProperties[_index].cardsGOList.Clear();
        playerProperties[_index].user.cardList.Clear();
    }
    #endregion

    #region AppQuit
    private void OnApplicationQuit()
    {
        Debug.Log("Server Disconnected, all the clients will quit the application");
    }
    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
    #endregion
}