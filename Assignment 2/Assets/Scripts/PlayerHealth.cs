using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats stats;
    public GameOverManager gameOver;  
    public float enemyDamage = 50f; 
    public float parryableDamage = 30f;

    private PlayerController controller;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public float invincibilityTime = 1f; 
    public float blinkInterval = 0.1f; // how fast player blinks

    private void Awake()
    {
        if (stats == null && PlayerStats.Instance != null)
            stats = PlayerStats.Instance;

        controller = GetComponent<PlayerController>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
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
            TakeDamage(enemyDamage);
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

        float elapsed = 0f;
        while (elapsed < invincibilityTime)
        {
            if (spriteRenderer != null)
            {
                // Toggle between black and original
                spriteRenderer.color = (spriteRenderer.color == originalColor) ? Color.black : originalColor;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (playerCollider != null)
            playerCollider.enabled = true;  // Re-enable collider

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor; // Restore color
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
        gameOver.TriggerGameOver();
    }
}

