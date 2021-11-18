using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public List<GameObject> cardListGO;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject GetCardWithID(int _id)
    {
        for(int i = 0; i < cardListGO.Count; i++)
        {
            if(cardListGO[i].GetComponent<CardUI>().cardBase.card_id == _id)
            {
                return cardListGO[i];
            }
        }
        Debug.Log("No card found with this id: " + _id.ToString());
        return null;
    }

}
