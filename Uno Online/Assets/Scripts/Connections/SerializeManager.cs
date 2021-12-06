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
    public void SerializeStartMatch(bool _isVerify, UserBase _userToSend = null)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        if (_isVerify)
        {
            writer.Write(4);
            writer.Write(true);
            writer.Write(_userToSend.userNumber);
        }
        else
        {
            writer.Write(5);
            writer.Write(false);
        }
    }
    public void SerializeUserStatus(UserBase _user)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(5);
        writer.Write(_user.userNumber);
        writer.Write((int)_user.userStatus);
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
            case (6):
                
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
            }
            else
            {
                serverManager.userList[whichPlayer - 1] = new UserBase("", 0, 15);
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
            Debug.Log("Received DATA");
            newStream = new MemoryStream(buffer);
            whatis = Deserialize(_isClient);
        }

        return whatis;
    }
    public void SendData(int _what, bool _isClient, UserBase _userToSend = null)
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
                SerializeStartMatch(false);
                break;
            case (5):
                SerializeUserStatus(_userToSend);
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
