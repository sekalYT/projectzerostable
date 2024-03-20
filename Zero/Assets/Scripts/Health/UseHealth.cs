using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHealth : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private float healthValue;
    [SerializeField] private GameObject ItemDeactive;
    [SerializeField] private AudioSource SoundSystemHeal;

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            Player.GetComponent<Health>().AddHealth(healthValue);
            SoundSystemHeal.GetComponent<AudioSource>();
            SoundSystemHeal.Play();
            ItemDeactive.SetActive(false);
        }
    }
}
