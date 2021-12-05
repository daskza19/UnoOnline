using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour
{
    [Header("Server Properties")]
    public int host = 1818;
    public string password = "1919";

    [Header("Players Properties")]
    public List<UserBase> userList;

    [Header("Game Properties")]
    public bool isInGame = false;
    public int gameTurn;
    public bool isClock = true;

    [Header("UI Things")]
    public List<Sprite> spritesPerfil;
    public List<PlayerServer> playerProperties; 

    private SerializeManager serializeManager;
    public Socket newSocket;
    public IPEndPoint ipep;
    public EndPoint sendEnp;
    public Thread mainThread;

    private bool wannaUpdateInfo = false;
    public int whatToDo = 1000;

    // Start is called before the first frame update
    void Start()
    {
        serializeManager = GetComponent<SerializeManager>();
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, host);
        sendEnp = (EndPoint)ipep;

        newSocket.Bind(ipep);

        for (int i = 0; i < playerProperties.Count; i++)
        {
            playerProperties[i].userStatus.text = "DISCONNECTED";
            playerProperties[i].userStatus.color = Color.red;
        }

        mainThread = new Thread(ReceiveUsersLoop);
        mainThread.Start();
    }

    private void ReceiveUsersLoop()
    {
        while (userList.Count < 4)
        {
            whatToDo = serializeManager.ReceiveData(false);
            wannaUpdateInfo = true;
        }
        isInGame = true;
        Debug.Log("All players DONE!");
        serializeManager.SendData(4, false); //Send to the other players that the match is going to start (to change their scenes)
    }

    private void Update()
    {
        if (wannaUpdateInfo)
        {
            switch (whatToDo)
            {
                case (1): //Received a single User
                    UpdateUserUI();
                    break;
                case (5): //Received a single User
                    UpdateUserUI();
                    break;
                case (6): //Received a single User
                    UpdateUserUI();
                    break;
            }
            wannaUpdateInfo = false;
        }
    }

    public void UpdateUserUI()
    {
        for(int i = 0; i < userList.Count; i++)
        {
            if (userList[i] == null)
            {
                PutOneUserDisconnected(i);
                return;
            }
            playerProperties[i].userName.text = userList[i].userName;
            playerProperties[i].userImage.sprite = spritesPerfil[userList[i].userImage];
            playerProperties[i].numberofCards.text = "Number of cards: " + userList[i].cardList.Count.ToString();
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

    public void PutOneUserDisconnected(int _index)
    {
        playerProperties[_index].userName.text = "NO USER";
        playerProperties[_index].userImage.sprite = spritesPerfil[12];
        playerProperties[_index].userStatus.text = "DISCONNECTED";
        playerProperties[_index].userStatus.color = Color.red;
        playerProperties[_index].numberofCards.text = "Number of cards: 0";
    }

    private void OnApplicationQuit()
    {
        Debug.Log("HOLAAAAAAAAAAAAAAAAAAAAAAA");
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
}
