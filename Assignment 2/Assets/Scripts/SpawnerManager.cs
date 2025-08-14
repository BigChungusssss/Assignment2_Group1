using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab == null || spawnPoint == null) return;

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
