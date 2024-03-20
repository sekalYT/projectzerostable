using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInventory : MonoBehaviour
{

    [SerializeField] private GameObject ItemUI;
    [SerializeField] private GameObject Item;

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            ItemUI.SetActive(true);
            Item.SetActive(false);
        }    
    }
}

