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

        int index = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[index];

        Vector3 spawnPos = point.position;

        spawnPos += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            spawnPos.y = hit.point.y;
        }

        Instantiate(mobPrefab, spawnPos, point.rotation);
        spawnedCount++;
    }
}
