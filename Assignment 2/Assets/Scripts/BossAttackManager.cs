using System.Collections;
using UnityEngine;

[System.Serializable]
public class AttackSpawnInfo
{
    public GameObject attackPrefab;
    public float interval = 1f;
    public bool pauseBossOnSpawn = false; // pause boss movement when this prefab spawns
    public float bossPauseDuration = 1f;  // how long boss stays paused
    public string attackAnimationTrigger; // optional animation trigger
    public float animationWindUp = 0.3f;  // seconds to wait before spawning attack prefab
}

[System.Serializable]
public class BossPhase
{
    public string phaseName = "Phase";
    public AttackSpawnInfo[] attacks;
    public Transform[] spawnPoints;

    [Header("Optional Boss Health Trigger")]
    [Range(0f, 1f)]
    public float bossHealthThreshold = 0f;
    public GameObject boss;

    [Header("Phase Transition Delay")]
    public float phaseTransitionDelay = 1f;
}

public class BossAttackManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    public BossPhase[] phases;

    private int currentPhaseIndex = 0;
    private float[] attackTimers;
    private bool isTransitioning = false;

    void Start()
    {
        if (phases.Length > 0)
            InitializePhase(currentPhaseIndex);
    }

    void Update()
    {
        if (phases.Length == 0) return;

        BossPhase currentPhase = phases[currentPhaseIndex];
        if (currentPhase.attacks == null || currentPhase.attacks.Length == 0) return;
        if (currentPhase.spawnPoints.Length == 0) return;

        // Boss health phase trigger
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

        // Attack spawn logic
        for (int i = 0; i < currentPhase.attacks.Length; i++)
        {
            attackTimers[i] += Time.deltaTime;

            if (attackTimers[i] >= currentPhase.attacks[i].interval)
            {
                Transform spawnPoint = currentPhase.spawnPoints[Random.Range(0, currentPhase.spawnPoints.Length)];
                Vector3 spawnPos = spawnPoint.position;
                spawnPos.z = 0f;

                // Pause boss if required
                if (currentPhase.attacks[i].pauseBossOnSpawn && currentPhase.boss != null)
                {
                    BossVerticalFigureEight bossMove = currentPhase.boss.GetComponent<BossVerticalFigureEight>();
                    if (bossMove != null)
                    {
                        StartCoroutine(PauseAnimateAndSpawn(
                            bossMove,
                            currentPhase.boss.GetComponent<Animator>(),
                            currentPhase.attacks[i],
                            spawnPos,
                            spawnPoint.rotation
                        ));
                    }
                }
                else
                {
                    // If no pause, just trigger animation and spawn
                    if (currentPhase.boss != null && !string.IsNullOrEmpty(currentPhase.attacks[i].attackAnimationTrigger))
                    {
                        Animator bossAnim = currentPhase.boss.GetComponent<Animator>();
                        if (bossAnim != null)
                            bossAnim.SetTrigger(currentPhase.attacks[i].attackAnimationTrigger);
                    }

                    StartCoroutine(SpawnAttackWithDelay(currentPhase.attacks[i], spawnPos, spawnPoint.rotation));
                }

                attackTimers[i] = 0f;
            }
        }
    }

    private void InitializePhase(int phaseIndex)
    {
        BossPhase currentPhase = phases[phaseIndex];
        attackTimers = new float[currentPhase.attacks.Length];
    }

    private IEnumerator TransitionToNextPhase(float delay)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        yield return new WaitForSeconds(delay);

        NextPhase();
    }

    public void NextPhase()
    {
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Length)
        {
            currentPhaseIndex = phases.Length - 1;
            isTransitioning = false;
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

    // --- New coroutine for pause, animation, and spawn ---
    private IEnumerator PauseAnimateAndSpawn(BossVerticalFigureEight bossMove, Animator bossAnim, AttackSpawnInfo attack, Vector3 spawnPos, Quaternion rotation)
    {
        bossMove.StopMovement(); // stop movement

        // Trigger animation if available
        if (bossAnim != null && !string.IsNullOrEmpty(attack.attackAnimationTrigger))
        {
            bossAnim.SetTrigger(attack.attackAnimationTrigger);
        }

        // Wait for the wind-up + optional pause duration
        float waitTime = Mathf.Max(attack.animationWindUp, attack.bossPauseDuration);
        yield return new WaitForSeconds(waitTime);

        // Instantiate the attack prefab
        Instantiate(attack.attackPrefab, spawnPos, rotation);

        // Resume boss movement
        bossMove.ResumeMovement();
    }

    private IEnumerator SpawnAttackWithDelay(AttackSpawnInfo attack, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(attack.animationWindUp);
        Instantiate(attack.attackPrefab, position, rotation);
    }
}
