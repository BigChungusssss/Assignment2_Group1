using UnityEngine;

public class PlayerBoundsCheck : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        ClampToCameraBounds();
    }

    private void ClampToCameraBounds()
    {
        // Convert player position to viewport (0..1 range)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Clamp between 0 and 1 (inside camera view)
        viewportPos.x = Mathf.Clamp01(viewportPos.x);
        viewportPos.y = Mathf.Clamp01(viewportPos.y);

        // Convert back to world space
        Vector3 clampedWorldPos = mainCamera.ViewportToWorldPoint(viewportPos);

        // Keep player’s current Z position (in case it matters)
        clampedWorldPos.z = transform.position.z;

        transform.position = clampedWorldPos;
    }
}


