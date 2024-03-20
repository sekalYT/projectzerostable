using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger2 : MonoBehaviour
{
    [SerializeField] private GameObject SoundSystemType;

    void OnTriggerEnter2D(Collider2D col)
    {
        SoundSystemType.GetComponent<AudioSource>().enabled = true;
    }
}
