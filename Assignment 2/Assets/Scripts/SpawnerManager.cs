using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float spawnOffset = 1f;       // how far outside camera to spawn
    public float verticalPadding = 0.5f; // how far from top/bottom edges to avoid

    [SerializeField] private Camera mainCam;


    void Start()
    {
        mainCam = Camera.main;
    }

    /// <summary>
    /// Spawns an enemy prefab to the right of the camera view.
    /// </summary>

    public void SpawnEnemy(GameObject enemyPrefab, float minYPercent = 0f, float maxYPercent = 0f)
{
    if (enemyPrefab == null) return;

    Vector3 spawnPos = GetRightSideSpawnPosition(minYPercent, maxYPercent);
    Quaternion spawnRot = enemyPrefab.transform.rotation; // use prefab rotation

    Instantiate(enemyPrefab, spawnPos, spawnRot);
}
    private Vector3 GetRightSideSpawnPosition(float minYPercent, float maxYPercent)
    {
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camCenter = mainCam.transform.position;

        float maxX = camCenter.x + camWidth / 2;
        float minY = camCenter.y - camHeight / 2 + verticalPadding + camHeight * minYPercent;
        float maxY = camCenter.y + camHeight / 2 - verticalPadding + camHeight * maxYPercent;

        float spawnX = maxX + spawnOffset;
        float spawnY = Random.Range(minY, maxY);

        return new Vector3(spawnX, spawnY, 0f);
    }
}
