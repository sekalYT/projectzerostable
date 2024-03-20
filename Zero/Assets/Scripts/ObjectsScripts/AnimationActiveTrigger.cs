using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationActiveTrigger : MonoBehaviour
{
    public Animator Anim;

    void Start()
    {
        Anim = GetComponent<Animator> ();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
            {
                Anim.SetTrigger ("1");
            }
    }
}
