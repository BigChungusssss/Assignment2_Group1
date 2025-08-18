using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
}

public class Enemies : MonoBehaviour
{
    [Header("Spawner Settings")]
    public EnemySpawnInfo[] enemies;   // just the prefabs
    public Transform[] spawnPoints;    // spawn locations
    public float spawnInterval = 2f;   // one global interval

    private float spawnTimer;

    void Update()
    {
        if (enemies == null || enemies.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            // Pick random enemy
            EnemySpawnInfo chosenEnemy = enemies[Random.Range(0, enemies.Length)];

            // Pick random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Force Z = 0
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.z = 0f;

            // Spawn enemy
            Instantiate(chosenEnemy.enemyPrefab, spawnPos, spawnPoint.rotation);

            // Reset timer
            spawnTimer = 0f;
        }
    }

    public void ResetSpawner()
    {
        spawnTimer = 0f;
    }
}

