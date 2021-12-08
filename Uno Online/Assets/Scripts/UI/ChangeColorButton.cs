using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorButton : MonoBehaviour
{
    public MainManager manager;
    public GameObject panel;
    public void ChangeColor(int whichColor)
    {
        switch (whichColor)
        {
            case (1):
                manager.actualColor = 1;
                break;
            case (2):
                manager.actualColor = 2;
                break;
            case (3):
                manager.actualColor = 3;
                break;
            case (4):
                manager.actualColor = 4;
                break;
            default:
                manager.actualColor = 1;
                break;
        }
        manager.uiManager.SendToServerPutCardInMiddle();
        panel.SetActive(false);
    }

    private void Update()
    {
        if (manager == null)
        {
            manager = GameObject.Find("MainManager").GetComponent<MainManager>();
        }
    }
}
