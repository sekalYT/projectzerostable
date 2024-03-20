using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDisableOnTime1 : MonoBehaviour
{
    public float timeLeft;
    public float otkat; //default (на какое откатиться)
    public GameObject S;
    public float otkat2; // через сколько откатиться

// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay2D(Collider2D col)
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            S.SetActive(false);
        }
        if (timeLeft < otkat2)
        {
            S.SetActive(true);
            timeLeft = otkat;
        }
        }
    }
