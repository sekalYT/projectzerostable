using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDontGiveUp : MonoBehaviour
{
    public GameObject TimerText;
    public GameObject Player;
    [SerializeField] private float ValueMinus = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void IDont()
    {
        TimerText.GetComponent<countdowntimer>().timeValue -= ValueMinus;
        Player.GetComponent<Health>().dead = false;
        Player.GetComponent<Health>().isImmortal = true;
        Player.GetComponent<Health>().RespawnPlayer();
    }
}
