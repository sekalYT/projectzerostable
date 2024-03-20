using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timerrr : MonoBehaviour
{
    public float timeLeft;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Timer()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

