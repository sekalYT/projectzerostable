using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeath : MonoBehaviour
{

    private float randomNumber;
    public GameObject DeathText1;
    public GameObject DeathText2;
    public GameObject DeathText3;
    public GameObject DeathText0;

    void Start()
    {
         randomNumber = Random.Range(0, 3);

        if(randomNumber == 0)
        {
            DeathText0.SetActive(true);
        }

        if (randomNumber == 1)
        {
            DeathText1.SetActive(true);
        }

        if (randomNumber == 2)
        {
            DeathText2.SetActive(true);
        }

        if (randomNumber == 3)
        {
            DeathText3.SetActive(true);
        }
    }
}
