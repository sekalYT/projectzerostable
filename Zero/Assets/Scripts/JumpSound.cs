using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSound : MonoBehaviour
{
    public GameObject SoundSystem;
    public AudioSource Audio;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Audio.GetComponent<AudioSource>();
            Audio.Play();
        }

    }
}