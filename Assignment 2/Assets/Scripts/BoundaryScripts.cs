using UnityEngine;

public class PlayerBoundsCheck : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;    // Reference to your health script
    public Camera mainCamera;

    [Header("Respawn Settings")]
    public Transform respawnPoint;       // Object to respawn player at

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (respawnPoint == null)
            Debug.LogWarning("Respawn point not assigned!");
    }

    void Update()
    {
        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Only trigger if player is significantly outside camera view
        float margin = 0.5f; // 20% outside the view
        if (viewportPos.x < -margin || viewportPos.x > 1f + margin ||
            viewportPos.y < -margin || viewportPos.y > 1f + margin)
        {
            TakeDamageAndRespawn();
        }

    }






    private void TakeDamageAndRespawn()
    {
        // Take 1 health
        if (playerHealth != null)
            playerHealth.TakeDamage(1f);

        // Respawn at respawnPoint
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("Respawn point not set! Player position not reset.");
        }
    }
}


