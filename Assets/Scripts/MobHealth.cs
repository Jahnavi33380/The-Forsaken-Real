using UnityEngine;
using System.Collections;

public class MobHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public float healthRegenRate = 0f; // Health per second
    public float healthRegenDelay = 5f; // Delay before regen starts after taking damage
    
    [Header("Death Settings")]
    public float deathDelay = 2f; // Time before destroying the object
    public GameObject[] deathEffects; // Effects to spawn on death
    public AudioClip deathSound;
    public float deathSoundVolume = 1f;
    
    [Header("Damage Settings")]
    public bool isInvulnerable = false;
    public float invulnerabilityTime = 0.5f; // Time of invulnerability after taking damage
    public Color damageFlashColor = Color.red;
    public float damageFlashDuration = 0.1f;
    
    private int currentHealth;
    private float lastDamageTime = 0f;
    private bool isDead = false;
    private MobAI mobAI;
    private Renderer[] renderers;
    private Color[] originalColors;
    private Coroutine damageFlashCoroutine;
    
    // Events
    public System.Action<int, int> OnHealthChanged; // currentHealth, maxHealth
    public System.Action OnDeath;
    public System.Action<int> OnDamageTaken; // damage amount

    void Start()
    {
        currentHealth = maxHealth;
        mobAI = GetComponent<MobAI>();
        
        // Get all renderers for damage flash effect
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }
        
        // Start health regeneration if enabled
        if (healthRegenRate > 0)
        {
            StartCoroutine(HealthRegeneration());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;
        
        // Check invulnerability
        if (Time.time < lastDamageTime + invulnerabilityTime) return;
        
        currentHealth -= damage;
        lastDamageTime = Time.time;
        
        // Trigger events
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamageTaken?.Invoke(damage);
        
        // Damage flash effect
        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);
        damageFlashCoroutine = StartCoroutine(DamageFlash());
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Alert AI that we took damage
            if (mobAI != null)
            {
                // Could trigger a "hurt" state or make AI more aggressive
                Debug.Log($"{gameObject.name} took {damage} damage! Health: {currentHealth}/{maxHealth}");
            }
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log($"{gameObject.name} healed for {healAmount}! Health: {currentHealth}/{maxHealth}");
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    IEnumerator HealthRegeneration()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(1f);
            
            // Only regen if we haven't taken damage recently and health is not full
            if (Time.time > lastDamageTime + healthRegenDelay && currentHealth < maxHealth)
            {
                Heal(1); // Heal 1 HP per second
            }
        }
    }
    
    IEnumerator DamageFlash()
    {
        // Flash red
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = damageFlashColor;
            }
        }
        
        yield return new WaitForSeconds(damageFlashDuration);
        
        // Return to original color
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        currentHealth = 0;
        
        // Trigger death event
        OnDeath?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Stop AI
        if (mobAI != null)
        {
            mobAI.enabled = false;
        }
        
        // Play death sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
        }
        
        // Spawn death effects
        foreach (GameObject effect in deathEffects)
        {
            if (effect != null)
            {
                Instantiate(effect, transform.position, transform.rotation);
            }
        }
        
        // Start death sequence
        StartCoroutine(DeathSequence());
    }
    
    IEnumerator DeathSequence()
    {
        // Could add death animation here
        // For now, just wait and destroy
        
        yield return new WaitForSeconds(deathDelay);
        
        // Notify spawner that this mob died (if using SpawnPointManager)
        SpawnPointManager spawnManager = FindFirstObjectByType<SpawnPointManager>();
        if (spawnManager != null)
        {
            // The SpawnPointManager will handle tracking this automatically
        }
        
        Destroy(gameObject);
    }
    
    // Public getters
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public bool IsDead() => isDead;
    public bool IsAtFullHealth() => currentHealth >= maxHealth;
    
    // Public setters
    public void SetInvulnerable(bool invulnerable) => isInvulnerable = invulnerable;
    
    void OnDrawGizmosSelected()
    {
        // Draw health bar above the mob
        if (Application.isPlaying && !isDead)
        {
            Vector3 position = transform.position + Vector3.up * 3f;
            
            // Background
            Gizmos.color = Color.red;
            Gizmos.DrawCube(position, new Vector3(2f, 0.2f, 0.1f));
            
            // Health bar
            Gizmos.color = Color.green;
            float healthPercentage = GetHealthPercentage();
            Gizmos.DrawCube(position, new Vector3(2f * healthPercentage, 0.2f, 0.1f));
        }
    }
}
