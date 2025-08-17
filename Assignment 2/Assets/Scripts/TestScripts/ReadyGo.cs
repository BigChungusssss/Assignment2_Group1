using System.Collections;
using UnityEngine;
using TMPro;

public class ReadyGo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI readyGoText;

    [Header("Timing")]
    [SerializeField] private float readyTime = 1f;
    [SerializeField] private float goTime = 0.5f;

    [Header("Animation")]
    [SerializeField] private float popScale = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip readyClip;
    [SerializeField] private AudioClip goClip;
    [SerializeField] private AudioSource audioSource;

    private Animator[] allAnimators;
    private Rigidbody2D[] allRigidbodies2D;

    private void Start()
    {
        // Get all animators and rigidbodies in the scene
        allAnimators = FindObjectsOfType<Animator>();
        allRigidbodies2D = FindObjectsOfType<Rigidbody2D>();

        // Freeze game
        //PauseAll();

        // Start Ready/Go sequence
        StartCoroutine(ShowReadyGo());
    }

    private void PauseAll()
    {
        // Freeze physics
        Time.timeScale = 0f;

        // Pause animators
        foreach (Animator anim in allAnimators)
            anim.updateMode = AnimatorUpdateMode.Normal; // reset if needed
        foreach (Animator anim in allAnimators)
            anim.enabled = false;

        // Freeze rigidbodies
        foreach (Rigidbody2D rb in allRigidbodies2D)
            rb.simulated = false;
    }

    private void ResumeAll()
    {
        Time.timeScale = 1f;

        foreach (Animator anim in allAnimators)
            anim.enabled = true;

        foreach (Rigidbody2D rb in allRigidbodies2D)
            rb.simulated = true;
    }

    private IEnumerator ShowReadyGo()
    {
        readyGoText.gameObject.SetActive(true);

        // READY
        readyGoText.text = "READY?";
        if (audioSource && readyClip) audioSource.PlayOneShot(readyClip);
        yield return StartCoroutine(PopTextAnimation(readyTime));

        // GO
        readyGoText.text = "GO!";
        if (audioSource && goClip) audioSource.PlayOneShot(goClip);
        yield return StartCoroutine(PopTextAnimation(goTime));

        readyGoText.gameObject.SetActive(false);

        // Resume gameplay
        ResumeAll();
    }

    private IEnumerator PopTextAnimation(float duration)
    {
        float elapsed = 0f;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * popScale;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Sin((elapsed / duration) * Mathf.PI);
            readyGoText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        readyGoText.transform.localScale = originalScale;
    }
}
