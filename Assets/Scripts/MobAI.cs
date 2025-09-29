using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MobAI : MonoBehaviour
{
    public Transform target;           
    public float chaseRange = 10f;    
    public float attackRange = 2f;     
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            if (Time.time > lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
}
