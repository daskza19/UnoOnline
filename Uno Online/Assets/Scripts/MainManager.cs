using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public UIManager uiManager;
    public bool isInGame = false;
    public int gameTurn;
    public bool isClock = true;
    public List<GameObject> cardListGO;

    private static MainManager managerInstance;
    public SerializeManager serializeManager;
    public Socket newSocket;
    public IPEndPoint ipep;
    public EndPoint sendEnp;
    private Thread receiveThread;

    private bool wannaUpdateInfo = false;
    public int whatToDo = 1000;

    private void Awake()
    {
        user = new UserBase("Default Name", 1, 0);
        DontDestroyOnLoad(this.gameObject);
        if (managerInstance == null)
        {
            managerInstance = this;
        }
        else
        {
            Object.Destroy(gameObject);
        }
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

    public void DevtoolPassScene()
    {
        userList.Clear();
        UserBase _user1 = new UserBase("Manolo", 1, 2, UserStatus.Connected);
        userList.Add(_user1);
        UserBase _user2 = new UserBase("Silvino", 2, 4, UserStatus.Connected);
        userList.Add(_user2);
        UserBase _user3 = new UserBase("Javi", 3, 6, UserStatus.Connected);
        userList.Add(_user3);
        UserBase _user4 = new UserBase("Charlie", 4, 7, UserStatus.Connected);
        userList.Add(_user4);
        user = _user4;

        SceneManager.LoadScene("SampleScene");
    }

    public void ReceiveLoop()
    {
        while (true)
        {
            whatToDo = serializeManager.ReceiveData(true);
            wannaUpdateInfo = true;
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
                    else
                    {

                    }
                    break;
                case (4): //The server sent the action to load the match scene
                    SceneManager.LoadScene("SampleScene");
                    break;
            }
            wannaUpdateInfo = false;
        }
        if (uiManager == null)
        {
            GameObject uiman = GameObject.Find("UIManager");
            if (uiman != null)
            {
                uiManager = uiman.GetComponent<UIManager>();
            }
        }
    }

    public GameObject GetCardWithID(int _index)
    {
        if (cardListGO[_index] != null)
        {
            return cardListGO[_index];
        }
        return null;
    }

    #region GameplayFunctions
    public void SendToServerGetCard()
    {
        Debug.Log("Send to server the petition to get a new card!");
        serializeManager.SendData(10, true, user);
    }
    public void SendToServerPutCardInMiddle()
    {

    }
    #endregion


    #region ExitApplication
    private void OnApplicationQuit()
    {
        user.userStatus = UserStatus.Disconnected;
        serializeManager.SendData(5, true, user);
        Debug.Log("Client Desconnected");
    }
    private void OnDestroy()
    {
        if(receiveThread!=null) receiveThread.Abort();
    }
    #endregion
}
