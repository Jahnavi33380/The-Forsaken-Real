using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MobAI : MonoBehaviour
{
    public Transform target;           // the player
    public float chaseRange = 10f;     // distance to start chasing
    public float attackRange = 2f;     // distance to attack
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(); // assumes Animator is on child (model)
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            // stop and attack
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            if (Time.time > lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack"); // you’ll add attack anim later
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // always chase
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
}
