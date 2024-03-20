using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D col)
    {
        gameObject.GetComponent<AudioSource>().enabled = true;
    }
}
