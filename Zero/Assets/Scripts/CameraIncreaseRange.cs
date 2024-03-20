using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIncreaseRange : MonoBehaviour
{
    public Camera mainCamera; // Renamed the variable to avoid conflicts with the class name
    public float newSize = 1f; // Renamed the variable to be more descriptive

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            mainCamera.orthographicSize = newSize;
        }
    }
}