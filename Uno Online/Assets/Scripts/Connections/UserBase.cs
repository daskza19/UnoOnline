using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserBase
{
    public string userName = "Default";
    public int userNumber = 1;
    public int userImage = 0;
    public List<CardBase> cardList;

    public UserBase(string _name, int _num, int _image)
    {
        userName = _name;
        userImage = _image;
        userNumber = _num;
    }
}
