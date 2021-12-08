using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class SerializeManager : MonoBehaviour
{
    public MemoryStream newStream;
    private ServerManager serverManager;
    private MainManager clientManager;

    private void Start()
    {
        serverManager = GetComponent<ServerManager>();
        clientManager = GetComponent<MainManager>();
    }

    #region Serialize
    public void SerializeConnectToServer()
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(0);
        writer.Write(true);
    }
    public void SerializeSingleUser(UserBase _user)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(1);
        writer.Write(_user.userName);
        writer.Write(_user.userImage);
        writer.Write(_user.userNumber);
        writer.Write((int)_user.userStatus);
    }
    public void SerializeListOfUsers(List<UserBase> _listUsers)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(2);
        writer.Write(_listUsers.Count);

        for(int i = 0; i < _listUsers.Count; i++)
        {
            if(_listUsers[i] == null)
            {
                writer.Write("");
                writer.Write(100);
                writer.Write(100);
                writer.Write(0);
            }
            else
            {
                writer.Write(_listUsers[i].userName);
                writer.Write(_listUsers[i].userImage);
                writer.Write(_listUsers[i].userNumber);
                writer.Write((int)_listUsers[i].userStatus);
            }
        }
    }
    public void SerializePassword(string _password)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(3);
        writer.Write(_password);
    }
    public void SerializeStartMatch()
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(4);
    }
    public void SerializeUserStatus(UserBase _user)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(5);
        writer.Write(_user.userNumber);
        writer.Write((int)_user.userStatus);
    }
    public void SerializeUserAskForCard(int _userNumber)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(10);
        writer.Write(_userNumber);
    }
    public void SerializeCardListFromOneUser(UserBase _user)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(11);
        writer.Write(_user.userNumber); //First we write which player is going to restart the list
        writer.Write(_user.cardList.Count);

        //The server serialize all the cards from that user
        for(int i = 0; i < _user.cardList.Count; i++)
        {
            writer.Write((int)_user.cardList[i].cardType);
            writer.Write(_user.cardList[i].num);
        }
    }
    public void SerializeCardListFromAllUsers()
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(12);

        for(int i = 0; i < serverManager.userList.Count; i++)
        {
            writer.Write(serverManager.userList[i].cardList.Count);

            //The server serialize all the cards from that user
            for (int j = 0; j < serverManager.userList[i].cardList.Count; j++)
            {
                writer.Write((int)serverManager.userList[i].cardList[j].cardType);
                writer.Write(serverManager.userList[i].cardList[j].num);
            }
        }
    }
    public void SerializeIndexCardToPutInMiddle(UserBase _user, int _indexCard, bool isClient)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(13);
        writer.Write(_user.userNumber); //First we write which player is going to restart the list
        writer.Write(_indexCard);
        if (isClient)
        {
            writer.Write(clientManager.actualColor);
            writer.Write(clientManager.actualNumber);
        }
        else
        {
            writer.Write(serverManager.actualColor);
            writer.Write(serverManager.actualNumber);
        }
    }
    public void SerializeAllPlayerStatus()
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(14);
        for(int i = 0; i < 4; i++)
        {
            writer.Write((int)serverManager.userList[i].userStatus);
        }
    }
    #endregion

    #region Deserialize
    public int Deserialize(bool isClient = true)
    {
        BinaryReader reader = new BinaryReader(newStream);
        newStream.Seek(0, SeekOrigin.Begin);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);

        int whatis = reader.ReadInt32();

        switch (whatis)
        {
            case (0):
                //This is sent when a client is tring to start the app (before they sent the user)
                DeserializeNewUserOpenApp(isClient);
                break;
            case (1): 
                //A single User (used when a new User connects with the server)
                DeserializeSingleUser(reader, isClient);
                break;
            case (2):
                //A list of Users (used when the server sends all the user list to the client)
                DeserializeListOfUsers(reader, isClient);
                break;
            case (3):
                //Password (used at first when the app starts, the server send the password, after the client verifies that password when the user types)
                DeserializePassword(reader, isClient);
                break;
            case (4):
                //The server sent to all the users that are ready and they have to change the scene
                DeserializeGoingToStartMatch(reader);
                break;
            case (5):
                //The client sent that his status changed (sent their player number and the new status)
                DeserializePlayerStatus(reader, isClient);
                break;
            case (10):
                //The server gets the petition to send a random card to a user
                DeserializePlayerAskForCard(reader, isClient);
                break;
            case (11):
                //The client gets the new card (when a player have to get a card of the middle)
                DeserializeOneCardListOfPlayer(reader, isClient);
                break;
            case (12):
                //The client gets all card list from all users
                DeserializeAllCardListsFromAllPlayers(reader, isClient);
                break;
            case (13):
                //The client /server gets the petition to put one card to the middle (from and index in cardlist from that user)
                DeserializePlayerWantsPutCardOnMiddle(reader, isClient);
                break;
            case (14):
                //The server sent all players status, in this status we will know which user is in turn
                DeserializeAllPlayersStatus(reader, isClient);
                break;
        }
        return whatis;
    }
    private void DeserializeNewUserOpenApp(bool _isClient)
    {
        //Open the server and put the function to send the server password
        if (_isClient == false)
        {
            SendData(3, false);
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializeSingleUser(BinaryReader _reader, bool _isClient)
    {
        UserBase _newUser = new UserBase("Default", 1, 0);

        _newUser.userName = _reader.ReadString();
        _newUser.userImage = _reader.ReadInt32();
        _newUser.userNumber = _reader.ReadInt32();
        _newUser.userStatus = (UserStatus)_reader.ReadInt32();

        newStream.Flush();
        newStream.Close();

        if(_isClient == false)
        {
            int position = CheckForTheFirstUserNull();
            if (position == 5) //That's the first user connected
            {
                _newUser.userNumber = serverManager.userList.Count + 1;
                serverManager.userList.Add(_newUser);
            }
            else
            {
                _newUser.userNumber = position + 1;
                serverManager.userList[position] = _newUser;
            }
            Debug.Log("ReceivedNewUser, To Send the list");
            SendData(2, false);
        }
    }
    private void DeserializeListOfUsers(BinaryReader _reader, bool _isClient)
    {
        int listCount = _reader.ReadInt32();
        List<UserBase> _userList = new List<UserBase>();

        for(int i = 0; i < listCount; i++)
        {
            UserBase _newUser = new UserBase("Default", 1, 0);

            _newUser.userName = _reader.ReadString();
            _newUser.userImage = _reader.ReadInt32();
            _newUser.userNumber = _reader.ReadInt32();
            _newUser.userStatus = (UserStatus)_reader.ReadInt32();

            _userList.Add(_newUser);
        }

        //Update the client user lists
        if (_isClient)
        {
            clientManager.userList.Clear();
            for (int j = 0; j < _userList.Count; j++)
            {
                clientManager.userList.Add(_userList[j]);
            }
        }
        else
        {
            serverManager.userList.Clear();
            for (int j = 0; j < _userList.Count; j++)
            {
                serverManager.userList.Add(_userList[j]);
            }
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializePassword(BinaryReader _reader, bool _isClient)
    {
        string password = _reader.ReadString();

        if (_isClient)
        {
            clientManager.serverPassword = password;
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializeGoingToStartMatch(BinaryReader _reader)
    {
        //TODO: Do the code to change the scene, at the start of the other scene, all the clients will send a verification that they are ready
        Debug.Log("Change the scene to start the match!");

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializePlayerStatus(BinaryReader _reader, bool _isClient)
    {
        int whichPlayer = _reader.ReadInt32();
        UserStatus _status = (UserStatus)_reader.ReadInt32();

        if (_status == UserStatus.Disconnected)
        {
            if (_isClient)
            {
                clientManager.userList[whichPlayer - 1] = new UserBase("", 0, 15);
                clientManager.userList[whichPlayer - 1].userStatus = UserStatus.Disconnected;
            }
            else
            {
                serverManager.userList[whichPlayer - 1] = new UserBase("", 0, 15);
                serverManager.userList[whichPlayer - 1].userStatus = UserStatus.Disconnected;
                SendData(2, false);
            }
        }
        else
        {
            if (_isClient)
            {
                clientManager.userList[whichPlayer - 1].userStatus = _status;
            }
            else
            {
                serverManager.userList[whichPlayer - 1].userStatus = _status;
            }
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializePlayerAskForCard(BinaryReader _reader, bool _isClient)
    {
        int whichPlayer = _reader.ReadInt32();

        serverManager.CreateNewRandomCard(whichPlayer);
        SendData(11, false, serverManager.userList[whichPlayer-1]); //Send the new card to the users

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializeOneCardListOfPlayer(BinaryReader _reader, bool _isClient)
    {
        int whichPlayer = _reader.ReadInt32();
        int countCards = _reader.ReadInt32();

        if (_isClient)
        {
            clientManager.userList[whichPlayer - 1].cardList.Clear();

            for(int i = 0; i < countCards; i++)
            {
                CardBase _newCard = new CardBase(CardType.None, 0);
                _newCard.cardType = (CardType)_reader.ReadInt32();
                _newCard.num = _reader.ReadInt32();
                clientManager.userList[whichPlayer - 1].cardList.Add(_newCard);
            }

            clientManager.uiManager.WannaUpdateCardsOfAllPlayers();
        }
        else
        {
            Debug.Log("The server received a user card list, in theory thats not possible :o");
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializeAllCardListsFromAllPlayers(BinaryReader _reader, bool _isClient)
    {
        if (_isClient)
        {
            for (int i = 0; i < clientManager.userList.Count; i++)
            {
                int countCards = _reader.ReadInt32();
                clientManager.userList[i].cardList.Clear();

                //The server serialize all the cards from that user
                for (int j = 0; j < countCards; j++)
                {
                    CardBase _newCard = new CardBase(CardType.None, 0);
                    _newCard.cardType = (CardType)_reader.ReadInt32();
                    _newCard.num = _reader.ReadInt32();

                    clientManager.userList[i].cardList.Add(_newCard);
                }
            }
            clientManager.uiManager.WannaUpdateCardsOfAllPlayers();
        }
        else
        {
            Debug.Log("The server received a user card list, in theory thats not possible :o");
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializePlayerWantsPutCardOnMiddle(BinaryReader _reader, bool _isClient)
    {
        int whichPlayer = _reader.ReadInt32();
        int cardIndex = _reader.ReadInt32();

        if (_isClient)
        {
            clientManager.uiManager.WannaPutCardOnTheMiddle(whichPlayer, cardIndex);
            clientManager.actualColor = _reader.ReadInt32();
            clientManager.actualNumber = _reader.ReadInt32();
        }
        else
        {
            serverManager.userList[whichPlayer - 1].cardList.RemoveAt(cardIndex);
            serverManager.actualColor = _reader.ReadInt32();
            serverManager.actualNumber = _reader.ReadInt32();
            serverManager.wannaUpdateInfo = true;
            SendData(13, false, serverManager.userList[whichPlayer - 1], cardIndex); //After the server actualice the list, send the action to the other clients
        }

        newStream.Flush();
        newStream.Close();
    }
    private void DeserializeAllPlayersStatus(BinaryReader _reader, bool _isClient)
    {
        if (_isClient)
        {
            for (int i = 0; i < 4; i++)
            {
                clientManager.userList[i].userStatus = (UserStatus)_reader.ReadInt32();
                clientManager.user = clientManager.userList[clientManager.uiManager.GetPositionOfThePlayer(clientManager.user.userNumber)];
                clientManager.uiManager.WannaUpdateUIFromUserStates();
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                serverManager.userList[i].userStatus = (UserStatus)_reader.ReadInt32();
            }
        }

        newStream.Flush();
        newStream.Close();
    }
    #endregion

    #region SendAndReceive
    public int ReceiveData(bool _isClient)
    {
        int whatis = 1000;
        if (_isClient)
        {
            byte[] buffer = new byte[2048];
            int recv = clientManager.newSocket.ReceiveFrom(buffer, ref clientManager.sendEnp);
            if (recv == 0)
            {
                Application.Quit();
            }
            Debug.Log("Received DATA");
            newStream = new MemoryStream(buffer);
            whatis = Deserialize(_isClient);
        }
        else
        {
            byte[] buffer = new byte[2048];
            int recv = serverManager.newSocket.ReceiveFrom(buffer, ref serverManager.sendEnp);
            if (recv == 0)
            {
                whatis = 1000;
                return whatis;
            }
            Debug.Log("Received DATA");
            newStream = new MemoryStream(buffer);
            whatis = Deserialize(_isClient);
        }

        return whatis;
    }
    public void SendData(int _what, bool _isClient, UserBase _userToSend = null, int _indexCard = 0)
    {
        switch (_what)
        {
            case (0):
                SerializeConnectToServer();
                break;
            case (1):
                SerializeSingleUser(_userToSend);
                break;
            case (2):
                if(_isClient) SerializeListOfUsers(clientManager.userList);
                else SerializeListOfUsers(serverManager.userList);
                break;
            case (3):
                SerializePassword(serverManager.password);
                break;
            case (4):
                SerializeStartMatch();
                break;
            case (5):
                SerializeUserStatus(_userToSend);
                break;
            case (10):
                SerializeUserAskForCard(_userToSend.userNumber);
                break;
            case (11):
                SerializeCardListFromOneUser(_userToSend);
                break;
            case (12):
                SerializeCardListFromAllUsers();
                break;
            case (13):
                SerializeIndexCardToPutInMiddle(_userToSend, _indexCard, _isClient);
                break;
            case (14):
                SerializeAllPlayerStatus();
                break;
        }

        if (_isClient)
        {
            clientManager.newSocket.SendTo(newStream.ToArray(), newStream.ToArray().Length, SocketFlags.None, clientManager.sendEnp);
        }
        else
        {
            serverManager.newSocket.SendTo(newStream.ToArray(), newStream.ToArray().Length, SocketFlags.None, serverManager.sendEnp);
        }
        Debug.Log("Sent DATA");
    }
    #endregion

    private int CheckForTheFirstUserNull()
    {
        if (serverManager.userList.Count == 0)
            return 5;
        for(int i = 0; i < serverManager.userList.Count; i++)
        {
            if (serverManager.userList[i].userStatus == UserStatus.Disconnected)
            {
                return i;
            }
        }
        return 5;
    }
}
