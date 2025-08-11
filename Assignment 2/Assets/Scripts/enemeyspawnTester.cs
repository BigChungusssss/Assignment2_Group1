using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f; // seconds between spawns
    public float spawnOffset = 1f;   // how far outside camera to spawn
    public float verticalPadding = 0.5f; // how far from top/bottom edges to avoid

    private Camera mainCam;
    private float timer;

    

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRightSideSpawnPosition();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    Vector3 GetRightSideSpawnPosition()
    {
        // Camera bounds
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camCenter = mainCam.transform.position;

        float maxX = camCenter.x + camWidth / 2;
        float minY = camCenter.y - camHeight / 2 + verticalPadding;
        float maxY = camCenter.y + camHeight / 2 - verticalPadding;

        // Always to the right of camera
        float spawnX = maxX + spawnOffset;

        // Random Y within camera height, but with padding
        float spawnY = Random.Range(minY, maxY);

        return new Vector3(spawnX, spawnY, 0f);
    }
}
