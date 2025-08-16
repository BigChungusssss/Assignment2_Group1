using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats stats;   
    public float enemyDamage = 50f; 
    public float parryableDamage = 30f;

    private PlayerController controller;
    private Collider2D playerCollider;
    public float invincibilityTime = 1f; 

    private void Awake()
    {
        if (stats == null && PlayerStats.Instance != null)
            stats = PlayerStats.Instance;

        controller = GetComponent<PlayerController>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerCollider == null || !playerCollider.enabled) return; // Skip damage if collider disabled

        if (collision.CompareTag("Enemy") || collision.CompareTag("IndestructibleEnemy"))
        {
            TakeDamage(enemyDamage);
        }
        else if (collision.CompareTag("Parry"))
        {
            TakeDamage(parryableDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (stats == null) return;

        stats.Health -= amount;
        stats.Health = Mathf.Max(stats.Health, 0f);

        Debug.Log("Player hit! Current health: " + stats.Health);

        if (stats.Health <= 0f)
        {
            Die();
        }
        else
        {
            StartCoroutine(TemporaryInvincibility());
        }
    }

    private IEnumerator TemporaryInvincibility()
    {
        if (playerCollider != null)
            playerCollider.enabled = false; // Disable collider to make player invincible

        yield return new WaitForSeconds(invincibilityTime);

        if (playerCollider != null)
            playerCollider.enabled = true;  // Re-enable collider
    }

   
    public void EnableInvincibility()
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = false; // Disable collisions
        }
        Debug.Log("Player is now invincible!");
    }


    public void DisableInvincibility()
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = true; // Re-enable collisions
        }
        Debug.Log("Player invincibility removed!");
    }




    private void Die()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
        Time.timeScale = 0f;
    }
}
