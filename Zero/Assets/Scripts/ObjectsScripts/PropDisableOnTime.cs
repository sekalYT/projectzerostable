using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDisableOnTime : MonoBehaviour
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
    void OnTriggerEnter2D(Collider2D col)
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
