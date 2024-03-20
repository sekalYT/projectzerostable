using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    public float speed;
    public float jump;
    private Animator anim;
    private bool grounded;
    private int jumps = 2;
    public float Timer = 1f;
    public float Timerdefault = 1f;

    private void Awake()
    {
        // достаем компоненты VVV
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalinput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2((horizontalinput) * speed, body.velocity.y);
        // прыжок VVVV
        if (Input.GetKey(KeyCode.Space) && jumps > 0)
        {
            Jump();
        }
        //поворачивает игрока VVVV
        if (horizontalinput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalinput < -0.01f)

            transform.localScale = new Vector3(-1, 1, 1);

        anim.SetBool("run", horizontalinput != 0);
        anim.SetBool("grounded", grounded);

        if(Input.GetKey(KeyCode.Space) && jumps > 0)
        {
            Timer -= Time.deltaTime;

            if(Timer <= 0)
            {
                jumps = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0)
        {
            jumps -= 1;
        }

    }
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jump);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
            jumps = 2;
            Timer = Timerdefault;
         //   cooldowndown = cooldowndowndefault;
        }
    }
}
