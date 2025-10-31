using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Enemy Stats")]
    public float health = 100f;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 10f;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 2f;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform attackPoint;

    [Header("Ranges")]
    public float sightRange = 20f;
    public float attackRange = 10f;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Confined Area")]
    public Transform centerPoint;
    public float maxDistance = 8f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ConfineToArea();

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(centerPoint.position.x + randomX, transform.position.y, centerPoint.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

            Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        animator.SetBool("isAttacking", false);
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            agent.enabled = false;
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(DestroyEnemy), 3f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void ConfineToArea()
    {
        float distance = Vector3.Distance(transform.position, centerPoint.position);
        if (distance > maxDistance)
        {
            agent.SetDestination(centerPoint.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (centerPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(centerPoint.position, maxDistance);
        }
    }
}
