using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsKeys : MonoBehaviour

{
    public GameObject Door;
    public GameObject Panel;
    public bool g;

    void Start()
    {
        g = false;
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
                }
            }
        }
    }
}
