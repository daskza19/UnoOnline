using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public List<PlayerUI> playerUIs;
    public List<Sprite> fotosPerfil;

    // Start is called before the first frame update
    void Start()
    {
        playerUIs[0].imagenPerfil.sprite = fotosPerfil[2];
        playerUIs[1].imagenPerfil.sprite = fotosPerfil[3];
        playerUIs[2].imagenPerfil.sprite = fotosPerfil[4];
        playerUIs[3].imagenPerfil.sprite = fotosPerfil[14];
    }
}
