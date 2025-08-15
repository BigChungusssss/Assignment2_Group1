using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBombAttack : AttackBase, IDamageable
{

    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float lifetime = 10f;
    private Transform player;
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject explosionPrefab;
    private bool hasExploded = false;

    [Header("Health Settings")]
    public float maxHealth = 50f;
    public float currentHealth;


    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        StartAttack();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    void OnDestroy()
    {
        if (!hasExploded)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject explodeRadius = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explodeRadius, 0.7f);
        }

        Destroy(gameObject);
    }

    protected override void StartAttack()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(10);
        Explode();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Explode();
    }
    

    



}
