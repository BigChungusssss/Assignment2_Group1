using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats stats;       // Reference to PlayerStats
    public float enemyDamage = 50f; 
    public float parryableDamage = 30f;

    private PlayerController controller;
    private Collider2D playerCollider;
    public float invincibilityTime = 1f; // 1 second invincibility

    private void Awake()
    {
        if (stats == null && PlayerStats.Instance != null)
            stats = PlayerStats.Instance;

        controller = GetComponent<PlayerController>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore indestructible enemies
        // Normal enemies always damage
        if (collision.CompareTag("Enemy")||collision.CompareTag("IndestructibleEnemy"))
        {
            TakeDamage(enemyDamage);
        }
        // Parryable objects only damage if player is NOT parrying
        else if (collision.CompareTag("Parry"))
        {
            if (controller != null && controller.isParryActive)
            {
                Debug.Log("Parry blocked damage!");
                return; // Damage blocked
            }

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
            // Start temporary invincibility
            if (playerCollider != null)
                StartCoroutine(TemporaryInvincibility());
        }
    }

    private IEnumerator TemporaryInvincibility()
    {
        playerCollider.enabled = false; // Disable collisions
        yield return new WaitForSeconds(invincibilityTime);
        playerCollider.enabled = true;  // Re-enable collisions
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        gameObject.SetActive(false);
        // Add respawn/game over logic here
    }
}

