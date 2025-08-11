using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;
    private float damage;
    private Vector2 moveDirection;

    // Set by Gun when firing
    public void SetDamage(float calculatedDamage)
    {
        damage = calculatedDamage;
        Debug.Log("Projectile damage set to: " + damage);
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;

        // Optional: face the direction of movement
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}


