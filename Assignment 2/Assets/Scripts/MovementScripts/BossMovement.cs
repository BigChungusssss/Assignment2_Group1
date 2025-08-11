using UnityEngine;
using System.Collections;

public class BossVerticalFigureEight : MonoBehaviour
{
    public float radius = 2f;
    public float speed = 1f;          // radians per second
    public float pauseTime = 1f;
    public float verticalStretch = 1f; // controls vertical elongation

    private Vector2 centerPos;

    void Start()
    {
        centerPos = transform.position;
        StartCoroutine(FigureEightRoutine());
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
                // Pause at start of top ring (t=0)
                if (!pausedAtTop && Mathf.Abs(t - 0f) < 0.01f)
                {
                    pausedAtTop = true;
                    yield return new WaitForSeconds(pauseTime);
                }
                else if (pausedAtTop && Mathf.Abs(t - 0f) > 0.05f)
                {
                    // Reset flag after passing the pause range
                    pausedAtTop = false;
                }

                // Pause at start of bottom ring (t=π)
                if (!pausedAtBottom && Mathf.Abs(t - Mathf.PI) < 0.01f)
                {
                    pausedAtBottom = true;
                    yield return new WaitForSeconds(pauseTime);
                }
                else if (pausedAtBottom && Mathf.Abs(t - Mathf.PI) > 0.05f)
                {
                    pausedAtBottom = false;
                }
                float x = Mathf.Sin(2 * t) / verticalStretch * radius;
                float y = Mathf.Sin(t) * radius;
                transform.position = centerPos + new Vector2(x, y);



                t += Time.deltaTime * speed;
                yield return null;
            }

            t -= Mathf.PI * 2f;
        }
    }
}
