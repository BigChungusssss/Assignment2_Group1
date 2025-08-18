using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverScreen;   // Assign your UI panel
    public string fadeInTrigger = "FadeIn";
    public string fadeOutTrigger = "FadeOut";
    public float waitAfterFadeIn = 2f;  // How long to keep the screen before fade out
    public float waitAfterFadeOut = 1f; // How long before restart

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private System.Collections.IEnumerator GameOverSequence()
    {
        // Stop time
        Time.timeScale = 0f;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Animator anim = gameOverScreen.GetComponent<Animator>();

        if (anim != null)
        {
            // Play fade in
            anim.SetTrigger(fadeInTrigger);
            yield return new WaitForSecondsRealtime(waitAfterFadeIn);

            // Play fade out
            anim.SetTrigger(fadeOutTrigger);
            yield return new WaitForSecondsRealtime(waitAfterFadeOut);
        }
        else
        {
            // fallback if no animator
            yield return new WaitForSecondsRealtime(waitAfterFadeIn + waitAfterFadeOut);
        }

        // Restart current scene
        //Time.timeScale = 1f; 
        //SceneManager.LoadScene(0);
    }
}
