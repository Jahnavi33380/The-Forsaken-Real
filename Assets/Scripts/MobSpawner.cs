using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public GameObject mobPrefab;
    public Transform[] spawnPoints;
    public float spawnDelay = 3f;
    public int mobsToSpawn = 5;

    private int spawnedCount = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnMob), 1f, spawnDelay);
    }

    void SpawnMob()
    {
        if (spawnedCount >= mobsToSpawn) return;

        // Pick random spawn point
        int index = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[index];

        // Start with base spawn position
        Vector3 spawnPos = point.position;

        // Add a small random offset (avoid stacking)
        spawnPos += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        // Raycast down to terrain/ground
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            spawnPos.y = hit.point.y; // snap to ground
        }

        Instantiate(mobPrefab, spawnPos, point.rotation);
        spawnedCount++;
    }
}
