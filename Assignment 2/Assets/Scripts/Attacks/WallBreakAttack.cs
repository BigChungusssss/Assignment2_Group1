using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallBreakAttack : AttackBase
{
    [SerializeField]
    float lifetime = 6f;
    private Rigidbody2D rb;
    [SerializeField]
    private float speed = 6.5f;
    private void Start()
    {
        hp = 100;
        cooldown = 5;
        parryable = false;
        rb = GetComponent<Rigidbody2D>();
        StartAttack();
        Destroy(gameObject, lifetime); 
    }

    protected override void StartAttack()
    {
        StartCoroutine(BulletSequence());

    }

    private IEnumerator BulletSequence()
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.left * speed;

    }
}
