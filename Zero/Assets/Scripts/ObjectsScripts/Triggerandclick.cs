using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerandclick : MonoBehaviour
{
    public GameObject Disable;
    public GameObject Key;
    public GameObject Disable2;

    void OnTriggerStay2D(Collider2D col)
    {
        if(Input.GetKey(KeyCode.X))
        {
            Disable.SetActive(false);
            gameObject.SetActive(false);
            Key.SetActive(false);
            Disable2.SetActive(false);
        }
    }
}
