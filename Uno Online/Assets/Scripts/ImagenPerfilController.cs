using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagenPerfilController : MonoBehaviour
{
    public GameObject selectedImage;
    public List<Sprite> selectedSprites;
    public bool isSelected = false;
    public int playerSelected;
    public TeamsManager teamsManager;

    public void SetImagePerfil()
    {
        if (isSelected == true)
            return;

        for(int i = 0; i < teamsManager.imagenesPerfil.Count; i++)
        {
            if(teamsManager.imagenesPerfil[i].isSelected == true)
            {
                if(teamsManager.imagenesPerfil[i].playerSelected == teamsManager.actualUserNumber)
                {
                    teamsManager.imagenesPerfil[i].DeselectImage();
                }
            }
        }

        selectedImage.SetActive(true);
        selectedImage.GetComponent<Image>().sprite = selectedSprites[teamsManager.actualUserNumber];
        playerSelected = teamsManager.actualUserNumber;
        isSelected = true;

        for (int i = 0; i < teamsManager.imagenesPerfil.Count; i++)
        {
            if (teamsManager.imagenesPerfil[i].isSelected == true)
            {
                if (teamsManager.imagenesPerfil[i].playerSelected == teamsManager.actualUserNumber)
                {
                    teamsManager.indexImage = i;
                }
            }
        }
    }

    public void DeselectImage()
    {
        if (isSelected == false)
            return;

        selectedImage.SetActive(false);
        isSelected = false;
        playerSelected = 0;
    }
}
