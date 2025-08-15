using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : AttackBase
{
    //movement
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float lifetime = 0.5f;
    [SerializeField]
    float travelTime = 1f;

    //split
    [SerializeField]
    GameObject smallBullet;
    [SerializeField]
    GameObject smallBulletParry;
    [SerializeField]
    float fireDelay = 1f;
    [SerializeField]
    float smallBulletSpeed = 8f;
    private Transform player;
    private Rigidbody2D rb;
    private int num;

    [Header("Health Settings")]
    public float maxHealth = 10f;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        num = Random.Range(1, 6);
        StartAttack();
        Destroy(gameObject, lifetime); //destroy after lifetime
    }

    protected override void StartAttack()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.left * speed;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // Move phase
        yield return new WaitForSeconds(travelTime);
        rb.velocity = Vector2.zero;

        // Wait before firing
        yield return new WaitForSeconds(fireDelay);

        // Fire at player
        if (player != null)
        {
            if (num != 4)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                GameObject bullet = Instantiate(smallBullet, transform.position, Quaternion.identity);
                Rigidbody2D smallRb = bullet.GetComponent<Rigidbody2D>();
                smallRb.velocity = direction * smallBulletSpeed;
                Destroy(bullet, 2.5f);
            }
            else
            {
                Vector2 direction = (player.position - transform.position).normalized;
                GameObject bulletParry = Instantiate(smallBulletParry, transform.position, Quaternion.identity);
                Rigidbody2D smallRb = bulletParry.GetComponent<Rigidbody2D>();
                smallRb.velocity = direction * smallBulletSpeed;
                Destroy(bulletParry, 2.5f);
            }
        }

        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f); // Clamp to 0

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
    

    
}
