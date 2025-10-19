using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MobAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;           
    public float detectionRange = 15f;    
    public float chaseRange = 20f;    
    public float attackRange = 2f;     
    public float attackCooldown = 1.5f;
    
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 5f;
    
    [Header("Detection Settings")]
    public LayerMask obstacleLayerMask = 1; // Default layer
    public float fieldOfView = 120f;
    
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    public float patrolRange = 10f;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = 0f;
    private float lastPatrolTime = 0f;
    private int currentPatrolIndex = 0;
    
    // AI States
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Return
    }
    
    public AIState currentState = AIState.Idle;
    private Vector3 originalPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        originalPosition = transform.position;
        
        agent.speed = walkSpeed;
        agent.angularSpeed = rotationSpeed;
        
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        bool canSeePlayer = CanSeePlayer();
        
        Debug.Log($"{name} | State: {currentState} | OnNavMesh: {agent.isOnNavMesh} | PathPending: {agent.pathPending} | HasPath: {agent.hasPath}");
        
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdleState(distanceToPlayer, canSeePlayer);
                break;
            case AIState.Patrol:
                HandlePatrolState(distanceToPlayer, canSeePlayer);
                break;
            case AIState.Chase:
                HandleChaseState(distanceToPlayer, canSeePlayer);
                break;
            case AIState.Attack:
                HandleAttackState(distanceToPlayer, canSeePlayer);
                break;
            case AIState.Return:
                HandleReturnState();
                break;
        }
        
        UpdateAnimator();
    }
    
    void HandleIdleState(float distanceToPlayer, bool canSeePlayer)
    {
        if (target != null)
        {
            ChangeState(AIState.Chase);
            return;
        }
        
        if (patrolPoints.Length > 0)
        {
            ChangeState(AIState.Patrol);
        }
    }
    
    void HandlePatrolState(float distanceToPlayer, bool canSeePlayer)
    {
        if (canSeePlayer && distanceToPlayer <= detectionRange)
        {
            ChangeState(AIState.Chase);
            return;
        }
        
        if (patrolPoints.Length == 0) return;
        
        if (agent.remainingDistance < 0.5f)
        {
            if (Time.time > lastPatrolTime + patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                lastPatrolTime = Time.time;
            }
        }
    }
    
    void HandleChaseState(float distanceToPlayer, bool canSeePlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            ChangeState(AIState.Attack);
            return;
        }
        
        if (!canSeePlayer || distanceToPlayer > chaseRange)
        {
            ChangeState(AIState.Return);
            return;
        }
        
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name}: Not on NavMesh!");
            return;
        }

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
            agent.isStopped = false;

        agent.speed = runSpeed;
        agent.SetDestination(target.position);
    }
    
    void HandleAttackState(float distanceToPlayer, bool canSeePlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            ChangeState(AIState.Chase);
            return;
        }
        
        if (!canSeePlayer)
        {
            ChangeState(AIState.Return);
            return;
        }
        
        agent.isStopped = true;
        
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        
    }
    
    void HandleReturnState()
    {
        float distanceToOrigin = Vector3.Distance(transform.position, originalPosition);
        
        if (distanceToOrigin < 1f)
        {
            ChangeState(AIState.Idle);
            return;
        }
        
        if (agent.isOnNavMesh)
        {
            agent.speed = walkSpeed;
            agent.isStopped = false;
            agent.SetDestination(originalPosition);
        }
        else
        {
            Vector3 direction = (originalPosition - transform.position).normalized;
            direction.y = 0;
            transform.position += direction * walkSpeed * Time.deltaTime;
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
    
    void ChangeState(AIState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        agent.isStopped = false;
        
        Debug.Log($"{gameObject.name} changed state to: {newState}");
    }
    
    bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 dir = (target.position - origin).normalized;
        float dist = Vector3.Distance(origin, target.position);

        if (Vector3.Angle(transform.forward, dir) > fieldOfView / 2f) return false;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, Physics.AllLayers))
            return hit.transform == target || hit.transform.IsChildOf(target);

        return false;
    }
    
    void UpdateAnimator()
    {
        if (animator == null) return;
        
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.cyan;
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                    Gizmos.DrawLine(transform.position, point.position);
                }
            }
        }
    }
}
