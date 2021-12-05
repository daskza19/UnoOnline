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
    }
    public void SerializeListOfUsers(List<UserBase> _listUsers)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(2);
        writer.Write(_listUsers.Count);

        for(int i = 0; i < _listUsers.Count; i++)
        {
            writer.Write(_listUsers[i].userName);
            writer.Write(_listUsers[i].userImage);
            writer.Write(_listUsers[i].userNumber);
        }
    }
    public void SerializePassword(string _password)
    {
        newStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newStream);

        writer.Write(3);
        writer.Write(_password);
    }
    #endregion

    #region Deserialize
    public void Deserialize(bool isClient = true)
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
        }
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

        newStream.Flush();
        newStream.Close();

        if(_isClient == false)
        {
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

            _userList.Add(_newUser);
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
    #endregion

    #region SendAndReceive
    public void ReceiveData(bool _isClient)
    {
        if (_isClient)
        {
            byte[] buffer = new byte[2048];
            int recv = clientManager.newSocket.ReceiveFrom(buffer, ref clientManager.sendEnp);
            if (recv == 0)
            {
                Application.Quit();
            }
            newStream = new MemoryStream(buffer);
            Deserialize(_isClient);
        }
        else
        {
            byte[] buffer = new byte[2048];
            int recv = serverManager.newSocket.ReceiveFrom(buffer, ref serverManager.sendEnp);
            newStream = new MemoryStream(buffer);
            Deserialize(_isClient);
        }
        Debug.Log("Received DATA");
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
}
