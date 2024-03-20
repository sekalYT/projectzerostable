using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UPEnemy_Random : MonoBehaviour
{
    [SerializeField] private float verticalDistance;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float damage;

    private float initialY;
    private float speedY;

    private void Awake()
    {
        initialY = transform.position.y;
    }

    private void Start()
    {
        // Generate a random speed in the Y direction
        speedY = Random.Range(minSpeed, maxSpeed) * (Random.value > 0.5f ? 1f : -1f);
    }

    private void Update()
    {
        // Move the enemy up and down using the generated speedY
        float newY = transform.position.y + speedY * Time.deltaTime;
        if (newY > initialY + verticalDistance || newY < initialY - verticalDistance)
        {
            // Reverse the direction if we reach the upper or lower limit
            speedY *= -1f;
        }
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}