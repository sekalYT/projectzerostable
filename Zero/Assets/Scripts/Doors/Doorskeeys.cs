using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorskeeys : MonoBehaviour
{
    public GameObject Panel;
    public bool g;
    public GameObject portal;
    private GameObject player;

    void Start()
    {
        g = false;
        player = GameObject.FindWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            if (g == true)
            {
                Panel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Panel.SetActive(false);
                    player.transform.position = new Vector2(portal.transform.position.x, portal.transform.position.y);
                }
            }
        }

       void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                Panel.SetActive(false);
            }
        }
    }
}
