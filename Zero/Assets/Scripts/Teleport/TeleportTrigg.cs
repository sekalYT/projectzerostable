using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigg : MonoBehaviour
{
    public GameObject portal;
    private GameObject player;
    public GameObject panel;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            panel.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                panel.SetActive(false);
            }

        }
    }

