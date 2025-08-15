using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public float interval = 1f;    // interval between spawns
}

public class Enemies : MonoBehaviour
{
    [Header("Spawner Settings")]
    public EnemySpawnInfo[] enemies;
    public Transform[] spawnPoints;
    public GameObject boss; // optional boss for health trigger
    [Range(0f, 1f)]
    //public float bossHealthThreshold = 0f; // 0 = disabled

    private float[] enemyTimers;

    void Start()
    {
        if (enemies.Length > 0)
            enemyTimers = new float[enemies.Length];
    }

    void Update()
    {
        if (enemies == null || enemies.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        // Check boss health trigger
        // if (boss != null && bossHealthThreshold > 0f)
        // {
        //     EnemyHealth bossHealth = boss.GetComponent<EnemyHealth>();
        //     // if (bossHealth != null)
        //     // {
        //     //     float healthPercentage = (float)bossHealth.CurrentHealth / bossHealth.maxHealth;
        //     //     if (healthPercentage <= bossHealthThreshold)
        //     //     {
        //     //         // Optional: trigger some event here if boss reaches threshold
        //     //     }
        //     // }
        // }

        // Spawn enemies continuously
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyTimers[i] += Time.deltaTime;

            if (enemyTimers[i] >= enemies[i].interval)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 spawnPos = spawnPoint.position;
                spawnPos.z = 0f;

                Instantiate(enemies[i].enemyPrefab, spawnPos, spawnPoint.rotation);

                enemyTimers[i] = 0f;
            }
        }
    }

    public void ResetSpawner()
    {
        if (enemies.Length > 0)
            enemyTimers = new float[enemies.Length];
    }
}

