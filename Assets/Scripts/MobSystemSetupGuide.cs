using UnityEngine;

/*
 * MOB AI & SPAWNER SYSTEM SETUP GUIDE
 * ===================================
 * 
 * This guide explains how to set up the improved mob AI and spawner system.
 * 
 * COMPONENTS OVERVIEW:
 * -------------------
 * 1. MobAI - Advanced AI with states (Idle, Patrol, Chase, Attack, Return)
 * 2. MobHealth - Enhanced health system with damage effects and events
 * 3. MobSpawner - Basic spawner with wave support
 * 4. SpawnPointManager - Advanced spawn management with multiple spawn types
 * 
 * SETUP INSTRUCTIONS:
 * ------------------
 * 
 * STEP 1: SET UP YOUR MOB PREFAB
 * - Add MobAI component to your mob prefab
 * - Add MobHealth component to your mob prefab
 * - Add NavMeshAgent component to your mob prefab
 * - Add Animator component to your mob prefab
 * - Make sure your mob has a Collider
 * - Tag your mob prefab appropriately
 * 
 * STEP 2: SET UP SPAWN POINTS
 * - Create empty GameObjects for spawn points
 * - Position them where you want mobs to spawn
 * - Name them descriptively (e.g., "SpawnPoint_1", "SpawnPoint_2")
 * 
 * STEP 3: CHOOSE YOUR SPAWNING SYSTEM
 * 
 * OPTION A: BASIC SPAWNER (MobSpawner)
 * - Add MobSpawner component to a GameObject in your scene
 * - Assign your mob prefab(s) to the mobPrefabs array
 * - Assign your spawn points to the spawnPoints array
 * - Configure spawn settings (delay, count, etc.)
 * 
 * OPTION B: ADVANCED SPAWNER (SpawnPointManager)
 * - Add SpawnPointManager component to a GameObject in your scene
 * - Create SpawnPointManager.SpawnPoint objects in the inspector
 * - Assign spawn points and configure each one individually
 * - Set up wave settings and global parameters
 * 
 * STEP 4: CONFIGURE MOB AI
 * - Set detection range (how far mob can see player)
 * - Set chase range (how far mob will chase player)
 * - Set attack range (how close to attack)
 * - Configure movement speeds (walk vs run)
 * - Set up patrol points if desired
 * - Configure field of view and obstacle detection
 * 
 * STEP 5: CONFIGURE MOB HEALTH
 * - Set max health
 * - Configure damage effects (flash color, duration)
 * - Set up death effects and sounds
 * - Configure invulnerability settings
 * 
 * STEP 6: ANIMATION SETUP
 * - Make sure your Animator has these parameters:
 *   - "Speed" (Float) - for movement speed
 *   - "IsRunning" (Bool) - for running animation
 *   - "Attack" (Trigger) - for attack animation
 * 
 * STEP 7: NAVMESH SETUP
 * - Bake NavMesh for your level
 * - Make sure spawn points are on the NavMesh
 * - Test that mobs can navigate to the player
 * 
 * TIPS:
 * -----
 * - Use the Scene view gizmos to visualize detection ranges
 * - Test different spawn point configurations
 * - Adjust AI parameters based on your game's difficulty
 * - Use the SpawnPointManager for complex spawning scenarios
 * - The MobSpawner is simpler but less flexible
 * 
 * DEBUGGING:
 * ----------
 * - Check console for AI state changes
 * - Use Scene view to see detection ranges and spawn points
 * - Make sure player has "Player" tag
 * - Verify NavMesh is properly baked
 * 
 * EXAMPLE CONFIGURATION:
 * ----------------------
 * Detection Range: 15f
 * Chase Range: 20f
 * Attack Range: 2f
 * Walk Speed: 2f
 * Run Speed: 5f
 * Field of View: 120f
 * Max Health: 50
 * 
 * This creates mobs that can see the player from 15 units away,
 * chase them up to 20 units, and attack when within 2 units.
 * They walk at 2 units/second and run at 5 units/second when chasing.
 */

public class MobSystemSetupGuide : MonoBehaviour
{
    [Header("Quick Setup Helper")]
    [Tooltip("Drag your mob prefab here")]
    public GameObject mobPrefab;
    
    [Tooltip("Drag your spawn point GameObjects here")]
    public Transform[] spawnPoints;
    
    [Tooltip("Check this to auto-setup basic spawner")]
    public bool autoSetupBasicSpawner = false;
    
    [Tooltip("Check this to auto-setup advanced spawner")]
    public bool autoSetupAdvancedSpawner = false;
    
    void Start()
    {
        if (autoSetupBasicSpawner)
        {
            SetupBasicSpawner();
        }
        
        if (autoSetupAdvancedSpawner)
        {
            SetupAdvancedSpawner();
        }
    }
    
    void SetupBasicSpawner()
    {
        GameObject spawnerObj = new GameObject("Basic Mob Spawner");
        MobSpawner spawner = spawnerObj.AddComponent<MobSpawner>();
        
        if (mobPrefab != null)
        {
            spawner.mobPrefabs = new GameObject[] { mobPrefab };
        }
        
        if (spawnPoints.Length > 0)
        {
            spawner.spawnPoints = spawnPoints;
        }
        
        Debug.Log("Basic spawner setup complete!");
    }
    
    void SetupAdvancedSpawner()
    {
        GameObject spawnerObj = new GameObject("Advanced Spawn Point Manager");
        SpawnPointManager manager = spawnerObj.AddComponent<SpawnPointManager>();
        
        if (mobPrefab != null)
        {
            manager.mobPrefabs = new GameObject[] { mobPrefab };
        }
        
        if (spawnPoints.Length > 0)
        {
            manager.spawnPoints = new SpawnPointManager.SpawnPoint[spawnPoints.Length];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                manager.spawnPoints[i] = new SpawnPointManager.SpawnPoint
                {
                    point = spawnPoints[i],
                    isActive = true,
                    spawnRadius = 2f,
                    spawnType = SpawnPointManager.SpawnType.Normal,
                    spawnDelay = 3f,
                    maxMobsAtPoint = 3,
                    cooldownTime = 10f
                };
            }
        }
        
        Debug.Log("Advanced spawner setup complete!");
    }
}
