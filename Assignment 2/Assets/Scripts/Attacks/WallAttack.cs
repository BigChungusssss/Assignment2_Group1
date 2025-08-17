using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallAttack : AttackBase
{
    [SerializeField]
    float lifetime = 5f;
    private Rigidbody2D rb;
    [SerializeField]
    private float speed = 5f;

    [Header("Health Settings")]
    public float maxHealth = 25f;
    public float currentHealth;
    private void Start()
    {
        currentHealth = maxHealth;
        cooldown = 5;
        parryable = false;
        rb = GetComponent<Rigidbody2D>();
        StartAttack();
        Destroy(gameObject, lifetime); //destroy after lifetime
    }

    protected override void StartAttack()
    {
        StartCoroutine(BulletSequence());

    }

    private IEnumerator BulletSequence()
    {
        yield return new WaitForSeconds(0.6f);
        rb.velocity = Vector2.left * speed;

    }

    // public void TakeDamage(float amount)
    // {
    //     currentHealth -= amount;
    //     if (currentHealth <= 0) Destroy(gameObject);
    // }

}
