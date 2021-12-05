using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamsManager : MonoBehaviour
{
    public List<ImagenPerfilController> imagenesPerfil;
    public List<Sprite> spritesPerfil;
    public MainManager mainManager;
    public SerializeManager serializeManager;
    public Animator animator;

    public InputField nameInput;
    public InputField passwordInput;

    private void Start()
    {
        int chosenImage = Random.Range(0, 12);
        imagenesPerfil[chosenImage].SetImagePerfil();
    }

    public void OnButtonToEnterRoom()
    {
        if(nameInput.text == "")
        {
            Debug.Log("No name entered!");
        }
        else if(passwordInput.text != mainManager.serverPassword)
        {
            Debug.Log("Incorrect password");
        }
        else
        {
            mainManager.user.userName = nameInput.text;
            //serializeManager.SendData(1, false, mainManager.user);
            animator.SetTrigger("OutPanel");
        }
    }
}
