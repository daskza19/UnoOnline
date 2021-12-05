using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserStatus
{
    Disconnected,
    Connected,
    Waiting,
    Ready,
    InTurn
}

[System.Serializable]
public class UserBase
{
    public string userName = "Default";
    public int userNumber = 1;
    public int userImage = 0;
    public List<CardBase> cardList;
    public UserStatus userStatus;
    public UserBase(string _name, int _num, int _image)
    {
        userName = _name;
        userImage = _image;
        userNumber = _num;
        userStatus = UserStatus.Disconnected;
        cardList = new List<CardBase>();
    }
}