using UnityEngine;
using UnityEngine.SceneManagement; // optional (for restart later)

public class HazmatChaseAI : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera; // assign the camera here
    private Animator anim;

    [Header("Movement Settings")]
    public float runSpeed = 4.5f;
    public float rotationSpeed = 5f;

    [Header("Attack Settings")]
    public float attackRange = 2.2f;       // distance from camera before attacking
    public float attackCooldown = 2f;      // time between attacks

    private bool isAttacking = false;
    private bool hasHitPlayer = false;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        if (!playerCamera)
        {
            Camera cam = Camera.main;
            if (cam != null) playerCamera = cam.transform;
        }

        if (playerCamera == null)
            Debug.LogError("Hazmat AI: No camera assigned or found!");
    }

    void Update()
    {
        if (!playerCamera || isAttacking) return;

        float distance = Vector3.Distance(transform.position, playerCamera.position);

        if (distance <= attackRange)
        {
            StartCoroutine(AttackSequence());
        }
        else
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        anim.ResetTrigger("Attack");
        anim.SetFloat("Speed", runSpeed);

        Vector3 direction = (playerCamera.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        transform.position += direction * runSpeed * Time.deltaTime;
    }

    System.Collections.IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Face the camera before attacking
        Vector3 dir = (playerCamera.position - transform.position).normalized;
        dir.y = 0; // keep upright
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = lookRot;

        anim.SetFloat("Speed", 0);
        anim.SetTrigger("Attack");
        Debug.Log("Hazmat attacking camera!");

        // wait until attack connects
        yield return new WaitForSeconds(0.6f);
        PerformAttack();

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }


    void PerformAttack()
    {
        if (playerCamera == null) return;

        float dist = Vector3.Distance(transform.position, playerCamera.position);
        if (dist <= attackRange + 0.5f)
        {
            Debug.Log("⚠️ Player has been hit!");
            hasHitPlayer = true;

            // Example: Restart scene or kill player
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            // OR: Disable player movement component, etc.
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
