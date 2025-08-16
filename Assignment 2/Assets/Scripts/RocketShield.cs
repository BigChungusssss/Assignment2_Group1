using UnityEngine;

public class RocketShield : MonoBehaviour
{

    public PlayerHealth playerHealth;
    public PlayerController player;
    public float contactDamage = 50f;   
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

    // Call to disable shield
    public void Deactivate()
    {
        gameObject.SetActive(false);
        playerHealth.DisableInvincibility();
        isActive = false;
    }
}

