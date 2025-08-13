using UnityEngine;


[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int count = 1;         
    public float interval = 1f;    
}


[System.Serializable]
public class Phase
{
    public string phaseName = "Phase";
    public EnemySpawnInfo[] enemies;
    public Transform[] spawnPoints;
}

public class PhaseManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Phase[] phases;

    private int currentPhaseIndex = 0;
    private int[] enemySpawnCounters;
    private float[] enemyTimers;

    void Start()
    {
        if (phases.Length > 0)
            InitializePhase(currentPhaseIndex);
    }

    void Update()
    {
        if (phases.Length == 0) return;

        Phase currentPhase = phases[currentPhaseIndex];
        if (currentPhase.spawnPoints.Length == 0) return;

        for (int i = 0; i < currentPhase.enemies.Length; i++)
        {
            if (enemySpawnCounters[i] >= currentPhase.enemies[i].count) continue;

            enemyTimers[i] += Time.deltaTime;

            if (enemyTimers[i] >= currentPhase.enemies[i].interval)
            {
           
                Transform spawnPoint = currentPhase.spawnPoints[Random.Range(0, currentPhase.spawnPoints.Length)];

                Instantiate(currentPhase.enemies[i].enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                enemySpawnCounters[i]++;
                enemyTimers[i] = 0f;
            }
        }

        bool phaseComplete = true;
        for (int i = 0; i < currentPhase.enemies.Length; i++)
        {
            if (enemySpawnCounters[i] < currentPhase.enemies[i].count)
            {
                phaseComplete = false;
                break;
            }
        }

        if (phaseComplete)
        {
            NextPhase();
        }
    }

    private void InitializePhase(int phaseIndex)
    {
        Phase currentPhase = phases[phaseIndex];
        enemySpawnCounters = new int[currentPhase.enemies.Length];
        enemyTimers = new float[currentPhase.enemies.Length];
    }

    public void NextPhase()
    {
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Length)
        {
            enabled = false; // all phases done
        }
        else
        {
            InitializePhase(currentPhaseIndex);
        }
    }

    public void ResetSpawner()
    {
        currentPhaseIndex = 0;
        InitializePhase(currentPhaseIndex);
        enabled = true;
    }
}



