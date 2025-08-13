using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public float interval = 1f;    // interval between spawns
}

[System.Serializable]
public class Phase
{
    public string phaseName = "Phase";
    public EnemySpawnInfo[] enemies;
    public Transform[] spawnPoints;

    [Header("Optional Boss Health Trigger")]
    [Range(0f, 1f)]
    public float bossHealthThreshold = 0f; // 0 = disabled
    public GameObject boss; // assign the boss GameObject if using health trigger

    [Header("Phase Transition Delay")]
    public float phaseTransitionDelay = 1f; // seconds to wait before next phase
}

public class PhaseManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Phase[] phases;

    private int currentPhaseIndex = 0;
    private float[] enemyTimers;
    private bool isTransitioning = false;

    void Start()
    {
        if (phases.Length > 0)
            InitializePhase(currentPhaseIndex);
    }
    void Update()
    {
        if (phases.Length == 0) return;

        Phase currentPhase = phases[currentPhaseIndex];

        // Check if phase has enemies
        if (currentPhase.enemies == null || currentPhase.enemies.Length == 0) return;
        if (currentPhase.spawnPoints.Length == 0) return;

        // Check boss health trigger
        if (!isTransitioning && currentPhase.boss != null && currentPhase.bossHealthThreshold > 0f)
        {
            EnemyHealth bossHealth = currentPhase.boss.GetComponent<EnemyHealth>();
            if (bossHealth != null)
            {
                float healthPercentage = (float)bossHealth.CurrentHealth / bossHealth.maxHealth;
                if (healthPercentage <= currentPhase.bossHealthThreshold)
                {
                    StartCoroutine(TransitionToNextPhase(currentPhase.phaseTransitionDelay));
                }
            }
        }

        // Spawn enemies continuously
        for (int i = 0; i < currentPhase.enemies.Length; i++)
        {
            enemyTimers[i] += Time.deltaTime;

            if (enemyTimers[i] >= currentPhase.enemies[i].interval)
            {
                Transform spawnPoint = currentPhase.spawnPoints[Random.Range(0, currentPhase.spawnPoints.Length)];
                Vector3 spawnPos = spawnPoint.position;
                spawnPos.z = 0f;

                Instantiate(currentPhase.enemies[i].enemyPrefab, spawnPos, spawnPoint.rotation);

                enemyTimers[i] = 0f;
            }
        }
    }





    private void InitializePhase(int phaseIndex)
    {
        Phase currentPhase = phases[phaseIndex];
        enemyTimers = new float[currentPhase.enemies.Length];
    }

    private IEnumerator TransitionToNextPhase(float delay)
    {
        if (isTransitioning) yield break; // prevent multiple coroutines
        isTransitioning = true;

        yield return new WaitForSeconds(delay);

        NextPhase();
    }

    public void NextPhase()
    {
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Length)
        {
            currentPhaseIndex = phases.Length - 1; // stay on last phase
            isTransitioning = false; // allow spawning to continue
            return;
        }

        InitializePhase(currentPhaseIndex);
        isTransitioning = false;
    }



 

    public void ResetSpawner()
    {
        currentPhaseIndex = 0;
        InitializePhase(currentPhaseIndex);
        enabled = true;
        isTransitioning = false;
    }
}
