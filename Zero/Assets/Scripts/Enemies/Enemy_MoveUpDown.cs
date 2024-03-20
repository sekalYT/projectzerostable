using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MoveUpDown : MonoBehaviour
{
    [SerializeField] private float verticalDistance; // Distance to move up and down
    [SerializeField] private float speed; // Speed of movement
    [SerializeField] private float damage; // Damage to apply to the player if they collide with the enemy

    private bool movingUp; // Flag to check if the enemy is moving up
    private float upEdge; // The upper limit for vertical movement
    private float downEdge; // The lower limit for vertical movement

    private void Awake()
    {
        upEdge = transform.position.y + verticalDistance;
        downEdge = transform.position.y - verticalDistance;
    }

    private void Update()
    {
        if (movingUp)
        {
            if (transform.position.y < upEdge)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                movingUp = false;
            }
        }
        else
        {
            if (transform.position.y > downEdge)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                movingUp = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}