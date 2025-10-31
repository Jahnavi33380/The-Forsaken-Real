using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 0.5f;
    public float moveDistance = 1f;

    private Vector3 startPos;
    private bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float moveStep = speed * Time.deltaTime;
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;

        transform.Translate(direction * moveStep);

        float distanceFromStart = Vector3.Distance(transform.position, startPos);
        if (distanceFromStart >= moveDistance)
        {
            movingRight = !movingRight;
        }
    }
}
