using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] public GameObject enemyDamage;

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
        StartCoroutine(FlashDamage());

        if (currentHealth <= 0)
        {
            Die();
        }
    }


     private IEnumerator FlashDamage()
    {
        
        float elapsed = 0f;
        while (elapsed < 2f)
        {
            
            
                // Toggle between black and original
                enemyDamage.SetActive(true);
                
            

            yield return new WaitForSeconds(0.08f);
            elapsed += 0.08f;
        }

   
            enemyDamage.SetActive(false);
           
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

