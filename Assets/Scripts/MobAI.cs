using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MobAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;

    [Header("Target Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float stopDistance = 2.5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2.7f;
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private string attackTrigger = "Attack";

    private float nextAttackTime = 0f;
    private bool attacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.stoppingDistance = stopDistance;

        var playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning("⚠️ No GameObject tagged 'Player' found!");
    }

    void Update()
    {
        if (!target) return;

        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        // Rotate toward player
        if (distance > 0.1f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        // Move until close
        if (distance > stopDistance && !attacking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
        }

        // Set animator movement blend
        float normalizedSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", normalizedSpeed);

        // Trigger attack
        if (!attacking && distance <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (target == null) yield break;

        attacking = true;
        agent.isStopped = true;

        // Face player
        transform.LookAt(target.position);

        // Trigger attack animation
        animator.SetTrigger(attackTrigger);

        // Cooldown setup
        nextAttackTime = Time.time + attackCooldown;

        // Wait for cooldown before attacking again
        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }

    // Called by an Animation Event on the punch impact frame
    public void OnMobHit()
    {
        DealDamageIfValid();
    }

    private void DealDamageIfValid()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange + 0.2f)
        {
            var playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerHit);
                Debug.Log("Player took " + damagePerHit + " damage.");
            }
        }
    }
}
