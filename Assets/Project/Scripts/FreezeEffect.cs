using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    public float freezeDuration = 5f;

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                Unfreeze();
            }
        }
    }

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void Freeze()
    {
        Debug.Log($"{gameObject.name} is now FROZEN for {freezeDuration} seconds.");

        if (isFrozen) return;

        isFrozen = true;
        freezeTimer = freezeDuration;

        var enemyAI = GetComponent<EnemyController>();
        if (enemyAI != null) enemyAI.enabled = false;

        if (rend != null)
            rend.material.color = Color.cyan; // freeze color
    }

    void Unfreeze()
    {
        isFrozen = false;

        var enemyAI = GetComponent<EnemyController>();
        if (enemyAI != null) enemyAI.enabled = true;

        if (rend != null)
            rend.material.color = Color.red; // normal color
    }

}
