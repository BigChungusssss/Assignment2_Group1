using System.Collections;
using UnityEngine;

[System.Serializable]
public class Phase
{
    public string phaseName = "Phase";
    public GameObject boss;                // Boss for this phase
    public GameObject[] spawners;          // Enemy spawners associated with this phase
}

public class PhaseManager2 : MonoBehaviour
{
    [Header("Phase Settings")]
    public Phase[] phases;
    public MonoBehaviour playerController; // Reference to player controller script
    public float prePhaseDelay = 5f;       // Time before first phase starts
    public float transitionDelay = 1f;     // Delay between phases

    private int currentPhaseIndex = -1;    // -1 = before first phase
    private bool phaseActive = false;

    void Start()
    {
        if (phases.Length == 0) return;

        // Disable all spawners initially
        DisableAllSpawners();

        // Disable player controller before first phase
        if (playerController != null)
            playerController.enabled = false;

        // Show first boss during pre-phase
        Phase firstPhase = phases[0];
        if (firstPhase.boss != null)
            firstPhase.boss.SetActive(true);

        // Start first phase after prePhaseDelay
        StartCoroutine(StartFirstPhaseAfterDelay(prePhaseDelay));
    }

    void Update()
    {
        if (!phaseActive || currentPhaseIndex < 0) return;

        Phase currentPhase = phases[currentPhaseIndex];

        if (currentPhase.boss != null)
        {
            EnemyHealth bossHealth = currentPhase.boss.GetComponent<EnemyHealth>();
            if (bossHealth != null && bossHealth.currentHealth <= 0)
            {
                StartCoroutine(TransitionToNextPhase());
            }
        }
    }

    private IEnumerator StartFirstPhaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Activate spawners for first phase
        StartNextPhase();
    }

    private IEnumerator TransitionToNextPhase()
    {
        phaseActive = false;

        // Disable current phase boss and spawners
        Phase currentPhase = phases[currentPhaseIndex];
        if (currentPhase.boss != null)
            currentPhase.boss.SetActive(false);

        if (currentPhase.spawners != null)
        {
            foreach (GameObject spawner in currentPhase.spawners)
            {
                if (spawner != null)
                    spawner.SetActive(false);
            }
        }

        // Disable player controller during transition
        if (playerController != null)
            playerController.enabled = false;

        yield return new WaitForSeconds(transitionDelay);

        StartNextPhase();
    }

    private void StartNextPhase()
    {
        currentPhaseIndex++;

        if (currentPhaseIndex >= phases.Length)
        {
            Debug.Log("All phases completed!");
            return;
        }

        Phase nextPhase = phases[currentPhaseIndex];

        // Enable boss for this phase
        if (nextPhase.boss != null)
            nextPhase.boss.SetActive(true);

        // Enable spawners
        if (nextPhase.spawners != null)
        {
            foreach (GameObject spawner in nextPhase.spawners)
            {
                if (spawner != null)
                    spawner.SetActive(true);
            }
        }

        // Enable player controller
        if (playerController != null)
            playerController.enabled = true;

        phaseActive = true;
        Debug.Log("Phase started: " + nextPhase.phaseName);
    }

    private void DisableAllSpawners()
    {
        foreach (Phase phase in phases)
        {
            if (phase.spawners != null)
            {
                foreach (GameObject spawner in phase.spawners)
                {
                    if (spawner != null)
                        spawner.SetActive(false);
                }
            }

            // Only deactivate bosses after first pre-phase boss is shown
            if (phase.boss != null && phase != phases[0])
                phase.boss.SetActive(false);
        }
    }
}
