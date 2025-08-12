using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitShotAttack : AttackBase
{
    //movement
    [SerializeField] 
    float speed = 5f;
    [SerializeField]
    float slowSpeed = 1.5f;
    [SerializeField] 
    float lifetime = 5f;

    //split
    [SerializeField]
    GameObject smallBullet;
    [SerializeField] 
    float timeBeforeSplit = 1f;
    [SerializeField] 
    bool isMainBullet = true; 

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartAttack();
        Destroy(gameObject, lifetime); //destroy after lifetime
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
        // Phase 1: Move left at full speed
        rb.velocity = Vector2.left * speed;

        float elapsed = 0f;
        float initialSpeed = speed;

        while (elapsed < timeBeforeSplit)
        {
            elapsed += Time.deltaTime;
            float currentSpeed = Mathf.Lerp(initialSpeed, slowSpeed, elapsed / timeBeforeSplit);
            rb.velocity = Vector2.left * currentSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);

        // Phase 3: Spawn 3 bullets
        SpawnSplitBullet(180f);    
        SpawnSplitBullet(155f);   
        SpawnSplitBullet(205f); 

        // Destroy the main bullet
        Destroy(gameObject);
    }

    private IEnumerator SmallBulletSequence()
    {
        rb.velocity = transform.right * speed;
        yield break;
    }

    private void SpawnSplitBullet(float angleDegrees)
    {
        // Create direction vector by rotating Vector2.right by the angle
        Vector2 direction = Quaternion.Euler(0, 0, angleDegrees) * Vector2.right;

        // Instantiate the small bullet prefab at current position with rotation matching the angle
        GameObject bullet = Instantiate(smallBullet, transform.position, Quaternion.Euler(0f, 0f, angleDegrees));

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.velocity = direction.normalized * 10f;
        }
    }

    
}
