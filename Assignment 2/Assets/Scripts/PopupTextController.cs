using UnityEngine;
using TMPro;
using System.Collections;

public class PopupTextController : MonoBehaviour
{
    public TextMeshProUGUI popupText; // Assign your PopupText UI here
    public float duration = 0.5f;
    public float popScale = 2f;

    private Vector3 originalScale;

    private void Awake()
    {
        if (popupText != null)
            originalScale = popupText.transform.localScale;
    }

    public void ShowText(string message, Color color)
    {
        if (popupText == null) return;

        popupText.text = message;
        popupText.color = color;
        popupText.transform.localScale = originalScale;
        popupText.gameObject.SetActive(true);

        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        float elapsed = 0f;
        Vector3 targetScale = originalScale * popScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / duration) * Mathf.PI);
            popupText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        popupText.gameObject.SetActive(false);
        popupText.transform.localScale = originalScale;
    }
}
