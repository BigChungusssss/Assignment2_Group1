using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    
    public int CurrentHealth => currentHealth;

 
    public delegate void EnemyDied(EnemyHealth enemy);
    public event EnemyDied OnEnemyDied;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // void Update()
    // {
    //     Debug.Log(CurrentHealth);
    // }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }
}
