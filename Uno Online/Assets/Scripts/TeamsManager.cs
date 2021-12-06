using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamsManager : MonoBehaviour
{
    [Header("Images to put")]
    public List<ImagenPerfilController> imagenesPerfil;
    public List<Sprite> spritesPerfil;
    public List<Sprite> spritesNumeros;

    [Header("Managers")]
    public MainManager mainManager;
    public SerializeManager serializeManager;
    public Animator animator;

    [Header("UI Things")]
    public InputField nameInput;
    public InputField passwordInput;
    public GameObject userPrefab;
    public GameObject blueTeamScroll;
    public GameObject redTeamScroll;

    private List<GameObject> userPanels = new List<GameObject>();

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
            mainManager.user.userStatus = UserStatus.Connected;
            serializeManager.SendData(1, true, mainManager.user);
            animator.SetTrigger("OutPanel");
        }
    }

    public void UpdateUsersTeams()
    {
        mainManager.user.userNumber = mainManager.userList.Count;
        Debug.Log("UpdateUsers");

        for (int j = 0; j < userPanels.Count; j++)
        {
            Destroy(userPanels[j]);
        }

        userPanels.Clear();

        for (int i = 0; i < mainManager.userList.Count; i++)
        {
            if (mainManager.userList[i].userName == "")
                return;

            GameObject newGO = new GameObject();
            if (mainManager.userList[i].userNumber==1 || mainManager.userList[i].userNumber == 3)
            {
                newGO = Instantiate(userPrefab, new Vector3(0, 0, 0), Quaternion.identity, blueTeamScroll.transform);
            }
            else if (mainManager.userList[i].userNumber == 2 || mainManager.userList[i].userNumber == 4)
            {
                newGO = Instantiate(userPrefab, new Vector3(0, 0, 0), Quaternion.identity, redTeamScroll.transform);
            }
            newGO.GetComponent<PlayerUI>().UserName.text = mainManager.userList[i].userName;
            newGO.GetComponent<PlayerUI>().imagenPerfil.sprite = spritesPerfil[mainManager.userList[i].userImage];
            if (mainManager.userList[i].userNumber == 1) newGO.GetComponent<PlayerUI>().PlayerNumber.sprite = spritesNumeros[0];
            if (mainManager.userList[i].userNumber == 2) newGO.GetComponent<PlayerUI>().PlayerNumber.sprite = spritesNumeros[1];
            if (mainManager.userList[i].userNumber == 3) newGO.GetComponent<PlayerUI>().PlayerNumber.sprite = spritesNumeros[2];
            if (mainManager.userList[i].userNumber == 4) newGO.GetComponent<PlayerUI>().PlayerNumber.sprite = spritesNumeros[3];

            userPanels.Add(newGO);
        }
    }
}
