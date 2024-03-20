using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillzonaScript : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, ignoreImmortality: true);
            }
        }
    }
}