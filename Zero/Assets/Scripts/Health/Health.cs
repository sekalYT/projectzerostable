using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    public bool dead;
    public GameObject DeathScreen;
    public GameObject buttonoff;

    public bool isImmortal = false;
    public float immortalTime = 3.0f;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float _damage, bool ignoreImmortality = false)
    {
        if (!isImmortal || ignoreImmortality)
        {
            currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

            if (currentHealth > 0)
            {
                anim.SetTrigger("hurt");
                DeathScreen.SetActive(false);
                StartCoroutine(EnableImmortality());
            }
            else
            {
                if (!dead)
                {
                    Time.timeScale = 0.0f;
                    dead = true;
                    DeathScreen.SetActive(true);
                }
            }
        }
    }

    private bool CheckIfAnyCheckpointExists()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        return checkpoints.Length > 0;
    }

    private IEnumerator EnableImmortality()
    {
        isImmortal = true;
        anim.SetBool("isImmortal", true);
        yield return new WaitForSeconds(immortalTime);
        isImmortal = false;
        anim.SetBool("isImmortal", false);
    }

    private void TeleportToNearestCheckpoint()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Transform closestCheckpoint = null;
        float closestDistance = Mathf.Infinity;
        Transform playerTransform = transform;

        foreach (GameObject checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector3.Distance(playerTransform.position, checkpoint.transform.position);
            if (distanceToCheckpoint < closestDistance)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint.transform;
            }
        }

        if (closestCheckpoint != null)
        {
            playerTransform.position = closestCheckpoint.position;
        }
        else
        {
            Debug.LogWarning("No Checkpoints found!");
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    public void RespawnPlayer()
    {
        currentHealth = startingHealth;
        DeathScreen.SetActive(false);
        isImmortal = false;
        Time.timeScale = 1.0f;
        StartCoroutine(TeleportWithDelay());
        if (!CheckIfAnyCheckpointExists())
        {
            buttonoff.SetActive(false);
        }
    }

    private IEnumerator TeleportWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        TeleportToNearestCheckpoint();
    }
}