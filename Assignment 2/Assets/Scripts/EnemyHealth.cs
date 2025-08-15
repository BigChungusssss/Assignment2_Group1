using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Awake() => currentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Update()
    {
        Debug.Log(currentHealth);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

