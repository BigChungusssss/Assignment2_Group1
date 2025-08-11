using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GamePhaseManager : MonoBehaviour
{
    public SpawnManager spawner; // Assign in Inspector
    public GameObject enemyType1;
    public GameObject enemyType2;

    public float spawnDelay = 1f;

    void Start()
    {
        StartCoroutine(RunPhases());
    }

    private System.Collections.IEnumerator RunPhases()
    {
        // Phase 1 - spawn enemyType1 five times
        for (int i = 0; i < 5; i++)
        {
            spawner.SpawnEnemy(enemyType1, 0.1f, -0.1f);
            yield return new WaitForSeconds(spawnDelay);
        }

        yield return new WaitForSeconds(2f); // short break before next phase

        // Phase 2 - spawn enemyType2 two at a time, three times
        for (int i = 0; i < 10; i++)
        {
            spawner.SpawnEnemy(enemyType2,  0.75f, -0.09f);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
