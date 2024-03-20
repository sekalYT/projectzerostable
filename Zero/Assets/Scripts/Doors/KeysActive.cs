using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysActive : MonoBehaviour
{
    public GameObject InputActive;

    void Start()
    {
        InputActive = GameObject.FindGameObjectWithTag("DoorInput");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            InputActive.gameObject.GetComponent<DoorsKeys>().g = true;
            gameObject.SetActive(false);
        }
    }
}
