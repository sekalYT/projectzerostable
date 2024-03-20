using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : MonoBehaviour
{
    public GameObject portal;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.transform.position = new Vector2(portal.transform.position.x, portal.transform.position.y);
            gameObject.SetActive(false);
        }
    }
}
