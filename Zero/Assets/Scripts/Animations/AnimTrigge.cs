using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTrigge : MonoBehaviour
{

     void OnTriggerEnter2D (Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            gameObject.GetComponent<Animator>().SetTrigger("1");
        }
    }
}
