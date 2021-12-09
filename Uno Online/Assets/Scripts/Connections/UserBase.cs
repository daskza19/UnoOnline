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
    public int userID = 0;
    public UserBase(string _name, int _num, int _image, UserStatus _status = UserStatus.Disconnected)
    {
        userName = _name;
        userImage = _image;
        userNumber = _num;
        userStatus = _status;
        cardList = new List<CardBase>();
        Debug.Log("Created User with this id: " + userID);
    }

    public void RandomUserID()
    {
        userID = Random.Range(0, int.MaxValue);
        Debug.Log("Randomized User ID. User name: " + userName);
    }
}
