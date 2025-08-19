using UnityEngine;
using System.Collections;

public class RocketShield : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public PlayerController player;
    public float contactDamage = 50f;   
    public float invincibilityDelay = 1.2f; // extra time after shield deactivates

    private bool isActive = false;

    private void Awake()
    {
        gameObject.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Enemy"))
        {
            var enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(contactDamage);
                Debug.Log("Rocket shield hit enemy!");
            }
        }
        
        if (player.isTransformed && other.CompareTag("Enemy"))
        {
            player.RevertTransformation();
        }
    }

    // Call to enable shield
    public void Activate()
    {
        gameObject.SetActive(true);
        playerHealth.EnableInvincibility();
        isActive = true;
    }

    // Call to disable shield (with delayed invincibility off)
    public void Deactivate()
    {

        isActive = false;
        StartCoroutine(DisableInvincibilityAfterDelay());
        gameObject.SetActive(false);
    }

    private IEnumerator DisableInvincibilityAfterDelay()
    {
        yield return new WaitForSeconds(invincibilityDelay);
        playerHealth.DisableInvincibility();
    }
}

