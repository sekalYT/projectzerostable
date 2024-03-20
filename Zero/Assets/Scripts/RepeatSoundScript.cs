using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatSoundScript : MonoBehaviour
{
    public AudioSource Audio;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Audio.GetComponent<AudioSource>();
            Audio.Play();
        }
    }
}
