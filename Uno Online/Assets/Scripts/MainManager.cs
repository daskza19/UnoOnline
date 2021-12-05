using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [Header("Client Properties")]
    public int host = 1818;
    public string serverPassword = "0";

    [Header("Players Properties")]
    public TeamsManager teamsManager;
    public UserBase user;
    public List<UserBase> userList;

    [Header("Game Properties")]
    public bool isInGame = false;
    public int gameTurn;
    public bool isClock = true;
    public List<GameObject> cardListGO;

    private SerializeManager serializeManager;
    public Socket newSocket;
    public IPEndPoint ipep;
    public EndPoint sendEnp;
    private Thread receiveThread;

    private bool wannaUpdateInfo = false;
    public int whatToDo = 1000;

    private void Awake()
    {
        user = new UserBase("Default Name", 1, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        serializeManager = GetComponent<SerializeManager>();

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1818);
        sendEnp = (EndPoint)ipep;

        serializeManager.SendData(0, true);

        receiveThread = new Thread(ReceiveLoop);
        receiveThread.Start();
    }

    public void ReceiveLoop()
    {
        while (true)
        {
            whatToDo = serializeManager.ReceiveData(true);
            wannaUpdateInfo = true;
            Debug.Log("RECEIVED NEW INFOOOOOOOOOOOOOOOOO");
        }
    }

    private void Update()
    {
        if (wannaUpdateInfo)
        {
            switch (whatToDo)
            {
                case (2): //The server sent a list of users
                    if (teamsManager != null) teamsManager.UpdateUsersTeams();
                    break;
            }
            wannaUpdateInfo = false;
        }
    }

    public GameObject GetCardWithID(int _id)
    {
        for(int i = 0; i < cardListGO.Count; i++)
        {
            if(cardListGO[i].GetComponent<CardUI>().cardBase.card_id == _id)
            {
                return cardListGO[i];
            }
        }
        Debug.Log("No card found with this id: " + _id.ToString());
        return null;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Client Desconnected");
        serializeManager.SendData(6, true);
    }

    private void OnDestroy()
    {
        receiveThread.Abort();
    }
}
