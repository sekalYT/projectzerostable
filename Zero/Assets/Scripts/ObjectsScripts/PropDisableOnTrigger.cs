using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDisableOnTrigger : MonoBehaviour
{
    public GameObject prop;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        gameObject.SetActive(false);
        prop.SetActive(false);
    }
}
