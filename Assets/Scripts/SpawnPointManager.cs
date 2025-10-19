using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform point;
        public bool isActive = true;
        public float spawnRadius = 2f;
        public SpawnType spawnType = SpawnType.Normal;
        public float spawnDelay = 3f;
        public int maxMobsAtPoint = 3;
        public float cooldownTime = 10f;
        
        [HideInInspector]
        public float lastSpawnTime = 0f;
        [HideInInspector]
        public int currentMobCount = 0;
    }
    
    public enum SpawnType
    {
        Normal,     // Regular spawning
        Wave,       // Spawn in waves
        Continuous, // Continuous spawning
        Triggered   // Spawn when triggered
    }
    
    [Header("Spawn Points")]
    public SpawnPoint[] spawnPoints;
    
    [Header("Wave Settings")]
    public bool useWaves = true;
    public int wavesPerRound = 3;
    public float timeBetweenWaves = 15f;
    public int mobsPerWave = 5;
    
    [Header("Global Settings")]
    public GameObject[] mobPrefabs;
    public float globalSpawnDelay = 2f;
    public int maxTotalMobs = 20;
    public bool spawnOnStart = true;
    
    [Header("Player Proximity")]
    public bool spawnNearPlayer = true;
    public float playerProximityRange = 30f;
    public float minDistanceFromPlayer = 10f;
    
    private Transform player;
    private int currentWave = 0;
    private int totalMobsSpawned = 0;
    private bool isSpawning = false;
    private List<GameObject> activeMobs = new List<GameObject>();
    
    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        // Initialize spawn points
        InitializeSpawnPoints();
        
        if (spawnOnStart)
        {
            StartCoroutine(StartSpawning());
        }
    }
    
    void InitializeSpawnPoints()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i].lastSpawnTime = -spawnPoints[i].cooldownTime;
            spawnPoints[i].currentMobCount = 0;
        }
    }
    
    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(1f); // Initial delay
        
        if (useWaves)
        {
            StartCoroutine(SpawnWaves());
        }
        else
        {
            StartCoroutine(ContinuousSpawning());
        }
    }
    
    IEnumerator SpawnWaves()
    {
        while (currentWave < wavesPerRound)
        {
            Debug.Log($"Starting Wave {currentWave + 1}");
            
            // Spawn mobs for this wave
            int mobsToSpawn = mobsPerWave;
            while (mobsToSpawn > 0 && totalMobsSpawned < maxTotalMobs)
            {
                SpawnMobAtBestPoint();
                mobsToSpawn--;
                totalMobsSpawned++;
                
                yield return new WaitForSeconds(globalSpawnDelay);
            }
            
            currentWave++;
            
            // Wait between waves
            if (currentWave < wavesPerRound)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
        
        Debug.Log("All waves completed!");
    }
    
    IEnumerator ContinuousSpawning()
    {
        while (totalMobsSpawned < maxTotalMobs)
        {
            SpawnMobAtBestPoint();
            totalMobsSpawned++;
            
            yield return new WaitForSeconds(globalSpawnDelay);
        }
    }
    
    void SpawnMobAtBestPoint()
    {
        SpawnPoint bestPoint = GetBestSpawnPoint();
        if (bestPoint != null)
        {
            SpawnMobAtPoint(bestPoint);
        }
    }
    
    SpawnPoint GetBestSpawnPoint()
    {
        List<SpawnPoint> validPoints = new List<SpawnPoint>();
        
        foreach (SpawnPoint point in spawnPoints)
        {
            if (IsSpawnPointValid(point))
            {
                validPoints.Add(point);
            }
        }
        
        if (validPoints.Count == 0) return null;
        
        // If spawning near player, prioritize points closer to player
        if (spawnNearPlayer && player != null)
        {
            validPoints.Sort((a, b) => 
            {
                float distA = Vector3.Distance(a.point.position, player.position);
                float distB = Vector3.Distance(b.point.position, player.position);
                return distA.CompareTo(distB);
            });
        }
        
        return validPoints[0];
    }
    
    bool IsSpawnPointValid(SpawnPoint point)
    {
        if (!point.isActive || point.point == null) return false;
        
        // Check cooldown
        if (Time.time < point.lastSpawnTime + point.cooldownTime) return false;
        
        // Check mob limit at this point
        if (point.currentMobCount >= point.maxMobsAtPoint) return false;
        
        // Check distance from player
        if (spawnNearPlayer && player != null)
        {
            float distanceToPlayer = Vector3.Distance(point.point.position, player.position);
            if (distanceToPlayer < minDistanceFromPlayer || distanceToPlayer > playerProximityRange)
                return false;
        }
        
        return true;
    }
    
    void SpawnMobAtPoint(SpawnPoint spawnPoint)
    {
        if (mobPrefabs.Length == 0) return;
        
        // Choose random mob prefab
        GameObject mobPrefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
        
        // Calculate spawn position
        Vector3 spawnPos = GetSpawnPosition(spawnPoint);
        
        // Spawn the mob
        GameObject newMob = Instantiate(mobPrefab, spawnPos, spawnPoint.point.rotation);
        
        // Set up the mob
        SetupSpawnedMob(newMob, spawnPoint);
        
        // Update spawn point data
        spawnPoint.lastSpawnTime = Time.time;
        spawnPoint.currentMobCount++;
        
        // Track active mobs
        activeMobs.Add(newMob);
        
        Debug.Log($"Spawned {mobPrefab.name} at {spawnPoint.point.name}");
    }
    
    Vector3 GetSpawnPosition(SpawnPoint spawnPoint)
    {
        Vector3 basePos = spawnPoint.point.position;
        
        // Add random offset within spawn radius
        Vector2 randomOffset = Random.insideUnitCircle * spawnPoint.spawnRadius;
        Vector3 spawnPos = basePos + new Vector3(randomOffset.x, 0, randomOffset.y);
        
        // Raycast to ground
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            spawnPos.y = hit.point.y;
        }
        
        return spawnPos;
    }
    
    void SetupSpawnedMob(GameObject mob, SpawnPoint spawnPoint)
    {
        // Set up MobAI if it exists
        MobAI mobAI = mob.GetComponent<MobAI>();
        if (mobAI != null)
        {
            // Set patrol points if this spawn point has them
            if (spawnPoint.spawnType == SpawnType.Normal)
            {
                // Could set up patrol points here
            }
        }
        
        
        // Add callback for when mob dies
        StartCoroutine(TrackMobLifecycle(mob, spawnPoint));
    }
    
    IEnumerator TrackMobLifecycle(GameObject mob, SpawnPoint spawnPoint)
    {
        while (mob != null)
        {
            yield return new WaitForSeconds(1f);
        }
        
        // Mob died, update count
        spawnPoint.currentMobCount--;
        activeMobs.Remove(mob);
    }
    
    // Public methods for external control
    public void StartNewWave()
    {
        if (!isSpawning)
        {
            currentWave = 0;
            totalMobsSpawned = 0;
            StartCoroutine(SpawnWaves());
        }
    }
    
    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
    }
    
    public void SetSpawnPointActive(int index, bool active)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            spawnPoints[index].isActive = active;
        }
    }
    
    public void TriggerSpawnAtPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            SpawnPoint point = spawnPoints[index];
            if (point.isActive)
            {
                SpawnMobAtPoint(point);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        
        foreach (SpawnPoint point in spawnPoints)
        {
            if (point.point == null) continue;
            
            // Draw spawn point
            Gizmos.color = point.isActive ? Color.green : Color.red;
            Gizmos.DrawWireSphere(point.point.position, 0.5f);
            
            // Draw spawn radius
            Gizmos.color = point.isActive ? Color.cyan : Color.gray;
            Gizmos.DrawWireSphere(point.point.position, point.spawnRadius);
            
            // Draw connection to player if spawning near player
            if (spawnNearPlayer && player != null)
            {
                float distance = Vector3.Distance(point.point.position, player.position);
                if (distance <= playerProximityRange && distance >= minDistanceFromPlayer)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(point.point.position, player.position);
                }
            }
        }
        
        // Draw player proximity range
        if (spawnNearPlayer && player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, playerProximityRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}
