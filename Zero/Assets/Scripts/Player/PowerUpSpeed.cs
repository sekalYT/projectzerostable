using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : MonoBehaviour
{
    public GameObject Pl;
    public float time;
    public float speedaddend;
    public bool one;
    public GameObject Sprite;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (!one) 
            {
                Pl.GetComponent<PlayerMovement>().speed += speedaddend;
                speedaddend = 0; 
                one = true;
                Sprite.SetActive(false);
            }
        }
    }

    public void Update()
    {
        if (one == true)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            if (time < 0)
            {
                Pl.GetComponent<PlayerMovement>().speed = 6;
                one = false;
            }
        }
    }
}