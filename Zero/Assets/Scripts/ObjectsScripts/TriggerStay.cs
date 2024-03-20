using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStay : MonoBehaviour
{

    public GameObject Hint;
    public bool i;

    void Update()
    {
        if (i == false)
        {
            Hint.SetActive(false);
        }

        if (i == true)
        {
            Hint.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            i = true;
        }
    }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                i = false;
            }
        }
    }

