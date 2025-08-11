using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public enum FireMode { Spread, Stream, Single, HeavySniper, Revolver }

    [Header("Targeting Settings")]
    public float bossDetectionRange = 10f;
    [Header("Damage Multipliers Per Mode")]
    public float spreadMultiplier = 1.5f;
    public float streamMultiplier = 1.3f;
    public float singleMultiplier = 1.9f;
    public float sniperMultiplier = 5f;
    public float revolverMultiplier = 3f;


   



    [Header("General Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    //public Transform targetEnemy;
    public float projectileSpeed = 50f;
    public bool rotateGunToShootDirection = true;
    public FireMode currentFireMode = FireMode.Single;

    [Header("Spread Mode Settings")]
    public int projectileCount = 5;
    public float spreadAngle = 45f;

    [Header("Stream Mode Settings")]
    public float streamFireRate = 0.1f;
    private Coroutine streamCoroutine;

    [Header("Heavy Sniper Settings")]
    public float sniperCooldown = 2f;
    public float sniperProjectileSpeed = 25f;
    public float sniperKnockbackForce = 5f;
    private bool canShootSniper = true;

    [Header("Revolver Mode Settings")]
    public int revolverClipSize = 6;
    public float revolverReloadTime = 1.5f;
    public float revolverProjectileSpeed = 12f;
    private int currentRevolverAmmo;
    private bool isReloading = false;

    [Header("Pickup Settings")]
    public bool isPickedUp = false;
    public Transform characterTransform;

    void Start()
    {
        currentRevolverAmmo = revolverClipSize;
    }

    void Update()
    {
        if (isPickedUp)
        {
            if (characterTransform != null)
                FaceCharacterDirection();

            if (PlayerStats.Instance != null && PlayerStats.Instance.currentGun != this)
            {
                PlayerStats.Instance.currentGun = this;
                float totalDamage = PlayerStats.Instance.GetBaseAttackPower();
               
            }
        }

        //if (targetEnemy == null)
        //{
            //GameObject boss = GameObject.FindGameObjectWithTag("Enemy");
            //if (boss != null)
                //targetEnemy = boss.transform;
        //}
    }



    private void FaceCharacterDirection()
    {
        float facingAngle = (characterTransform.localScale.x < 0) ? 180f : 0f;
        transform.rotation = Quaternion.Euler(0f, 0f, facingAngle);
    }

    public void Shoot(Vector2 direction)
{
    if (projectilePrefab == null || firePoint == null) return;

    float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    if (rotateGunToShootDirection)
    {
        float visualAngle = baseAngle;
        if (direction.x < 0)
            visualAngle += 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, visualAngle);
    }

    switch (currentFireMode)
    {
        case FireMode.Spread:
            FireSpread(baseAngle);
            break;
        case FireMode.Single:
            FireSingle(baseAngle);
            break;
        case FireMode.Stream:
            if (streamCoroutine == null)
                streamCoroutine = StartCoroutine(FireStream(direction));
            break;
        case FireMode.HeavySniper:
            if (canShootSniper)
                StartCoroutine(FireHeavySniper(baseAngle));
            break;
        case FireMode.Revolver:
            if (!isReloading)
            {
                if (currentRevolverAmmo > 0)
                {
                    FireRevolver(baseAngle);
                    currentRevolverAmmo--;
                    Debug.Log($"Revolver shot! {currentRevolverAmmo} bullets left.");
                }
                else
                {
                    StartCoroutine(ReloadRevolver());
                }
            }
            break;
    }
}



    private void FireSpread(float baseAngle)
    {
        float angleStep = (projectileCount > 1) ? spreadAngle / (projectileCount - 1) : 0f;
        float startAngle = baseAngle - (spreadAngle / 2f);

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startAngle + (i * angleStep);
            Vector2 shootDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = shootDir * projectileSpeed;
            
            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null && PlayerStats.Instance != null)
            {
                float calculatedDamage = PlayerStats.Instance.attackPower * spreadMultiplier;
                projScript.SetDamage(calculatedDamage);
                projScript.Initialize(shootDir);
            }
        }

        Debug.Log("Spread fire: " + projectileCount + " bullets!");
    }

    private void FireSingle(float angle)
    {
        Vector2 shootDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDir * projectileSpeed;
        
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null && PlayerStats.Instance != null)
        {
            float calculatedDamage = PlayerStats.Instance.attackPower * singleMultiplier;
            projScript.SetDamage(calculatedDamage);
            Debug.Log($"[Single] Damage set to: {calculatedDamage}");
            projScript.Initialize(shootDir);
        }

        Debug.Log("Single fire!");
    }
    private IEnumerator FireStream(Vector2 initialDirection)
{
    while (true)
    {
        Vector2 shootDir = initialDirection.normalized;

        //GameObject boss = GameObject.FindGameObjectWithTag("Enemy");
        //if (boss != null)
        //{
            //float distanceToBoss = Vector2.Distance(firePoint.position, boss.transform.position);
            //if (distanceToBoss <= bossDetectionRange)
            //{
                //shootDir = ((Vector2)boss.transform.position - (Vector2)firePoint.position).normalized;
            //}
        //}

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDir * projectileSpeed;

         Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null && PlayerStats.Instance != null)
        {
            float calculatedDamage = PlayerStats.Instance.attackPower * streamMultiplier;
            projScript.SetDamage(calculatedDamage);
            projScript.Initialize(shootDir);
        }

        Debug.Log("Stream fire bullet!");

        yield return new WaitForSeconds(streamFireRate);
    }
}




    private IEnumerator FireHeavySniper(float angle)
    {
        canShootSniper = false;

        Vector2 shootDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDir * sniperProjectileSpeed;

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null && PlayerStats.Instance != null)
        {
            float calculatedDamage = PlayerStats.Instance.attackPower * sniperMultiplier;
            projScript.SetDamage(calculatedDamage);
            projScript.Initialize(shootDir);
        }

        Debug.Log("Heavy Sniper fired!");
//


        yield return new WaitForSeconds(sniperCooldown);
        canShootSniper = true;
    }

    private void FireRevolver(float angle)
    {
        Vector2 shootDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDir * revolverProjectileSpeed;

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null && PlayerStats.Instance != null)
        {
            float calculatedDamage = PlayerStats.Instance.attackPower * revolverMultiplier;
            projScript.SetDamage(calculatedDamage);
            projScript.Initialize(shootDir);
        }
}

private IEnumerator ReloadRevolver()
{
    isReloading = true;
    Debug.Log("Reloading revolver...");
//
    yield return new WaitForSeconds(revolverReloadTime);
    currentRevolverAmmo = revolverClipSize;
    isReloading = false;
    Debug.Log("Revolver reloaded.");
}

    public void StopShooting()
    {
        if (streamCoroutine != null)
        {
            StopCoroutine(streamCoroutine);
            streamCoroutine = null;
        }
    }
}









