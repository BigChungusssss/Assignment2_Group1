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

public class BossAttackManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    public AttackSpawnInfo[] attacks;
    public Transform[] spawnPoints;
    public GameObject boss;

    private float[] attackTimers;

    void Start()
    {
        if (attacks.Length > 0)
            attackTimers = new float[attacks.Length];
    }

    void Update()
    {
        if (attacks == null || attacks.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < attacks.Length; i++)
        {
            attackTimers[i] += Time.deltaTime;

            if (attackTimers[i] >= attacks[i].interval)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 spawnPos = spawnPoint.position;
                spawnPos.z = 0f;

                if (attacks[i].pauseBossOnSpawn && boss != null)
                {
                    BossVerticalFigureEight bossMove = boss.GetComponent<BossVerticalFigureEight>();
                    Animator bossAnim = boss.GetComponent<Animator>();
                    if (bossMove != null)
                    {
                        StartCoroutine(PauseAnimateAndSpawn(bossMove, bossAnim, attacks[i], spawnPos, spawnPoint.rotation));
                    }
                }
                else
                {
                    // Trigger animation if available
                    if (boss != null && !string.IsNullOrEmpty(attacks[i].attackAnimationTrigger))
                    {
                        Animator bossAnim = boss.GetComponent<Animator>();
                        if (bossAnim != null)
                            bossAnim.SetTrigger(attacks[i].attackAnimationTrigger);
                    }

                    StartCoroutine(SpawnAttackWithDelay(attacks[i], spawnPos, spawnPoint.rotation));
                }

                attackTimers[i] = 0f;
            }
        }
    }

    // --- Coroutine for pause, animation, and spawn ---
    private IEnumerator PauseAnimateAndSpawn(BossVerticalFigureEight bossMove, Animator bossAnim, AttackSpawnInfo attack, Vector3 spawnPos, Quaternion rotation)
    {
        bossMove.StopMovement();

        if (bossAnim != null && !string.IsNullOrEmpty(attack.attackAnimationTrigger))
            bossAnim.SetTrigger(attack.attackAnimationTrigger);

        float waitTime = Mathf.Max(attack.animationWindUp, attack.bossPauseDuration);
        yield return new WaitForSeconds(waitTime);

        Instantiate(attack.attackPrefab, spawnPos, rotation);

        bossMove.ResumeMovement();
    }

    private IEnumerator SpawnAttackWithDelay(AttackSpawnInfo attack, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(attack.animationWindUp);
        Instantiate(attack.attackPrefab, position, rotation);
    }
}
