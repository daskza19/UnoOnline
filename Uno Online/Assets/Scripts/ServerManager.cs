using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

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

    private SerializeManager serializeManager;
    public Socket newSocket;
    public IPEndPoint ipep;
    public EndPoint sendEnp;
    public Thread mainThread;

    // Start is called before the first frame update
    void Start()
    {
        serializeManager = GetComponent<SerializeManager>();
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Any, host);
        sendEnp = (EndPoint)ipep;

        newSocket.Bind(ipep);
        mainThread = new Thread(ReceiveUsersLoop);
        mainThread.Start();
    }

    private void ReceiveUsersLoop()
    {
        while (userList.Count < 5)
        {
            Debug.Log("Hola");
            serializeManager.ReceiveData(false);
        }
        isInGame = true;
    }

    private void OnDestroy()
    {
        mainThread.Abort();
        newSocket.Close();
    }
}
