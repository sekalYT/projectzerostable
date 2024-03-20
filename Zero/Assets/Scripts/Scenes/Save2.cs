using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save2 : MonoBehaviour
{
    public string string1;

    void SaveGame()
    {
        PlayerPrefs.SetString("SavedString", string1);
    }

    void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedString"))
        {
            string1 = PlayerPrefs.GetString("SavedString");
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }
}
