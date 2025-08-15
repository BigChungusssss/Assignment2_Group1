using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;

    private bool isDead = false;
    private Rigidbody2D rb;
    private BossVerticalFigureEight movementScript; // Replace with your movement script class

    void Awake()
    {
        currentHealth = maxHealth;

        // Cache references
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<BossVerticalFigureEight>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        // Optional: debug current health
        Debug.Log(currentHealth);
    }

    private void Die()
    {
        isDead = true;

        // Stop Rigidbody movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Disable movement script if exists
        if (movementScript != null)
            movementScript.enabled = false;

        // Optional: play death animation, particle effect, etc.
        Debug.Log(gameObject.name + " has died but is still in scene.");
    }
}

