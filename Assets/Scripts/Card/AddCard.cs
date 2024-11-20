using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCard : MonoBehaviour
{

    [SerializeField] private Transform field;
    [SerializeField] private GameObject card;

    private void Awake()
    {
        
        for (int i = 0; i < 10; i++)
        {
            GameObject button = Instantiate(card);
            button.name = "" + i;
            button.transform.SetParent(field, false);
        }

    }

}
