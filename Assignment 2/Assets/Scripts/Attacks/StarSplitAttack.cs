using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StarSplitAttack : AttackBase
{
    //movement
    float speed;
    [SerializeField]
    float slowSpeed = 1.5f;
    [SerializeField]
    float lifetime = 0.5f;

    //split
    [SerializeField]
    GameObject smallBullet;
    [SerializeField]
    GameObject smallBulletParry;
    [SerializeField]
    float timeBeforeSplit = 1f;
    [SerializeField]
    bool isMainBullet = true;

    private Rigidbody2D rb;

    private void Start()
    {;
        cooldown = 5;
        rb = GetComponent<Rigidbody2D>();
        speed = Random.Range(6, 9);
        StartAttack();
        Destroy(gameObject, lifetime); 
    }

    protected override void StartAttack()
    {
        if (isMainBullet)
        {
            StartCoroutine(MainBulletSequence());
        }
        else
        {
            StartCoroutine(SmallBulletSequence());
        }
    }

    private IEnumerator MainBulletSequence()
    {
        rb.velocity = Vector2.up * speed;

        float elapsed = 0f;
        float initialSpeed = speed;
        int num1 = Random.Range(1, 6);

        while (elapsed < timeBeforeSplit)
        {
            elapsed += Time.deltaTime;
            float currentSpeed = Mathf.Lerp(initialSpeed, slowSpeed, elapsed / timeBeforeSplit);
            rb.velocity = Vector2.up * currentSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);

        //Spawn 3 bullets
        if (num1 == 4)
        {
            SpawnParryBullet(180f);
        }
        else
        {
            SpawnSplitBullet(180f);
        }
        SpawnSplitBullet(90f);
        SpawnSplitBullet(0f);
        SpawnSplitBullet(315f);
        SpawnSplitBullet(225f);
        SpawnSplitBullet(270f);
        SpawnSplitBullet(45f);
        SpawnSplitBullet(135f);


        //Destroy the main bullet
        Destroy(gameObject);
    }

    private IEnumerator SmallBulletSequence()
    {
        rb.velocity = transform.right * speed;
        yield break;
    }

    private void SpawnSplitBullet(float angleDegrees)
    {
        Vector2 direction = Quaternion.Euler(0, 0, angleDegrees) * Vector2.right;

        GameObject bullet = Instantiate(smallBullet, transform.position, Quaternion.Euler(0f, 0f, angleDegrees));

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.velocity = direction.normalized * 6.5f;
        }
        Destroy(bullet, lifetime);
    }

    private void SpawnParryBullet(float angleDegrees)
    {
        Vector2 direction = Quaternion.Euler(0, 0, angleDegrees) * Vector2.right;

        GameObject parryBullet = Instantiate(smallBulletParry, transform.position, Quaternion.Euler(0f, 0f, angleDegrees));

        Rigidbody2D rbBullet = parryBullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.velocity = direction.normalized * 6.5f;
        }

        Destroy(parryBullet, lifetime);
    }
}
