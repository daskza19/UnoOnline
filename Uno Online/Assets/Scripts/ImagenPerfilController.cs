using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagenPerfilController : MonoBehaviour
{
    public GameObject selectedImage;
    public bool isSelected = false;
    public TeamsManager teamsManager;

    public void SetImagePerfil()
    {
        if (isSelected == true)
            return;

        for(int i = 0; i < teamsManager.imagenesPerfil.Count; i++)
        {
            if(teamsManager.imagenesPerfil[i].isSelected == true)
            {
                teamsManager.imagenesPerfil[i].DeselectImage();
            }
        }

        selectedImage.SetActive(true);
        isSelected = true;

        for (int i = 0; i < teamsManager.imagenesPerfil.Count; i++)
        {
            if (teamsManager.imagenesPerfil[i].isSelected == true)
            {
                teamsManager.mainManager.user.userImage = i;
            }
        }
    }

    public void DeselectImage()
    {
        if (isSelected == false)
            return;

        selectedImage.SetActive(false);
        isSelected = false;
    }
}
