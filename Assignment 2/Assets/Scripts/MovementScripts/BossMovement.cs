using UnityEngine;
using System.Collections;

public class BossVerticalFigureEight : MonoBehaviour
{
    [Header("Movement Settings")]
    public float radius = 2f;            // size of movement
    public float speed = 1f;             // radians per second
    public float pauseTime = 1f;         // pause at top/bottom points
    public float verticalStretch = 1f;   // controls vertical elongation
    public float resumeSmoothTime = 0.01f; // seconds to ease in

    private Vector2 centerPos;
    private Coroutine movementRoutine;

    [HideInInspector]
    public bool isMovementPaused = false;

    private bool wasPausedLastFrame = false; // track transitions
    private float speedMultiplier = 1f;      // easing control

    void Start()
    {
        centerPos = transform.position;
        movementRoutine = StartCoroutine(FigureEightRoutine());
    }

    IEnumerator FigureEightRoutine()
    {
        float t = 0f;
        bool pausedAtTop = false;
        bool pausedAtBottom = false;

        while (true)
        {
            while (t < Mathf.PI * 2f)
            {
                // Pause at top (t = 0)
                if (!pausedAtTop && Mathf.Abs(t - 0f) < 0.01f)
                {
                    pausedAtTop = true;
                    yield return new WaitForSeconds(pauseTime);
                }
                else if (pausedAtTop && Mathf.Abs(t - 0f) > 0.05f)
                {
                    pausedAtTop = false;
                }

                // Pause at bottom (t = π)
                if (!pausedAtBottom && Mathf.Abs(t - Mathf.PI) < 0.01f)
                {
                    pausedAtBottom = true;
                    yield return new WaitForSeconds(pauseTime);
                }
                else if (pausedAtBottom && Mathf.Abs(t - Mathf.PI) > 0.05f)
                {
                    pausedAtBottom = false;
                }

                // If paused externally
                if (isMovementPaused)
                {
                    wasPausedLastFrame = true;
                    yield return null;
                    continue;
                }

                // Smooth resume after unpause
                if (wasPausedLastFrame)
                {
                    yield return StartCoroutine(SmoothResume());
                    wasPausedLastFrame = false;
                }

                // Movement
                float x = Mathf.Sin(2 * t) / verticalStretch * radius;
                float y = Mathf.Sin(t) * radius;
                transform.position = centerPos + new Vector2(x, y);

                t += Time.deltaTime * speed * speedMultiplier;
                yield return null;
            }

            t -= Mathf.PI * 2f; // Loop
        }
    }

    IEnumerator SmoothResume()
    {
        float elapsed = 0f;
        speedMultiplier = 0f;

        while (elapsed < resumeSmoothTime)
        {
            elapsed += Time.deltaTime;
            speedMultiplier = Mathf.SmoothStep(0f, 1f, elapsed / resumeSmoothTime);
            yield return null;
        }

        speedMultiplier = 1f;
    }

    public void StopMovement()
    {
        isMovementPaused = true;
    }

    public void ResumeMovement()
    {
        isMovementPaused = false;
    }
}
