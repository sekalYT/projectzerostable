using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menuscript : MonoBehaviour
{
     public GameObject Menu;
     public int gg = 1;

    void Update()
    {
        if(gg == 1)
        {
        if(Input.GetKey(KeyCode.Escape))
        {
          Menu.SetActive(true);
          Time.timeScale = 0f;
          gg = 0;
        }
        }
    }
}
