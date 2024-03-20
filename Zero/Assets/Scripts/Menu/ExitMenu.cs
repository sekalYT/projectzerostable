using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenu : MonoBehaviour
{
    public GameObject Menu;
    public GameObject menuscript;

    public void Exit()
    {
        Time.timeScale = 1f;
        Menu.SetActive(false);
        menuscript.GetComponent<Menuscript> ().gg += 1;
    }
}
