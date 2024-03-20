using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwaper : MonoBehaviour
{
    public int Scene;

    public void Swap()
    {
        SceneManager.LoadScene(Scene);
    }
}