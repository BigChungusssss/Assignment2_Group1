using System.Collections;
using UnityEngine;

[System.Serializable]
public class Phase
{
    public string phaseName = "Phase";
    public GameObject boss;             
    public GameObject[] spawners;         
}

public class PhaseManager2 : MonoBehaviour
{
    [Header("Phase Settings")]
    public Phase[] phases;
    public MonoBehaviour playerController;
    public PlayerHealth playerHealth;
    public Gun playerGun;                 
    public Animator blackScreenAnimator;   
    public float prePhaseDelay = 5f;       // Countdown after fade-in for first phase
    public float transitionDelay = 1f;     
    public float blackScreenDuration = 1f; 

    private int currentPhaseIndex = -1;    
    private bool phaseActive = false;

    void Start()
    {
        if (phases.Length == 0) return;

        DisableAllSpawners();

        if (playerController != null)
            playerController.enabled = false;
        if (playerGun != null)
            playerGun.StopShooting();

        StartCoroutine(PrePhaseSequence());
    }

    private IEnumerator PrePhaseSequence()
    {
        // // Fade-out black screen
        // if (blackScreenAnimator != null)
        //     blackScreenAnimator.SetTrigger("Start");

        // yield return new WaitForSeconds(blackScreenDuration);

        // Activate first boss (but keep player disabled)
        Phase firstPhase = phases[0];
        if (firstPhase.boss != null)
            firstPhase.boss.SetActive(true);

        // // Fade-in black screen
        // if (blackScreenAnimator != null)
        //     blackScreenAnimator.SetTrigger("End");

        //yield return new WaitForSeconds(blackScreenDuration);

        // Wait additional pre-phase countdown
        yield return new WaitForSeconds(prePhaseDelay);

        // Enable player and spawners
        if (playerController != null)
            playerController.enabled = true;

        if (firstPhase.spawners != null)
        {
            foreach (GameObject spawner in firstPhase.spawners)
            {
                if (spawner != null)
                    spawner.SetActive(true);
            }
        }

        currentPhaseIndex = 0;
        phaseActive = true;
        Debug.Log("First phase started: " + firstPhase.phaseName);
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
                playerHealth.EnableInvincibility();
                StartCoroutine(TransitionToNextPhase());

                

            }
        }
    }

    private IEnumerator TransitionToNextPhase()
    {
        phaseActive = false;
        Phase currentPhase = phases[currentPhaseIndex];

        // Stop player and shooting
        if (playerGun != null)
            playerGun.StopShooting();
        if (playerController != null)
            playerController.enabled = false;
        

        // Fade-out black screen
        if (blackScreenAnimator != null)
            blackScreenAnimator.SetTrigger("Start");


        yield return new WaitForSeconds(blackScreenDuration);


        // Disable current boss and spawners
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


        yield return new WaitForSeconds(transitionDelay);

        // Start next phase
        StartCoroutine(StartNextPhaseWithFade());
        
    }

    private IEnumerator StartNextPhaseWithFade()
    {
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Length)
        {
            Debug.Log("All phases completed!");
            yield break;
        }

        Phase nextPhase = phases[currentPhaseIndex];

        // Enable boss and spawners BEFORE fade-in
        if (nextPhase.boss != null)
            nextPhase.boss.SetActive(true);

        if (nextPhase.spawners != null)
        {
            foreach (GameObject spawner in nextPhase.spawners)
            {
                if (spawner != null)
                    spawner.SetActive(true);
            }
        }

        // Fade-in black screen
        if (blackScreenAnimator != null)
            blackScreenAnimator.SetTrigger("End");


        yield return new WaitForSeconds(blackScreenDuration);
       

        // Enable player after fade-in
        if (playerController != null)
            playerController.enabled = true;

        

        phaseActive = true;
         playerHealth.DisableInvincibility();
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

            if (phase.boss != null && phase != phases[0])
                phase.boss.SetActive(false);
        }
    }
}

