using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSwap : MonoBehaviour
{
    public int Scene;

    void OnTriggerEnter2D()
    {
        SceneManager.LoadScene(Scene);
    }
}
