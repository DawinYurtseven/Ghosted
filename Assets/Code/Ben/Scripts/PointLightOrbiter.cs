using UnityEngine;

public class PointLightOrbiter : MonoBehaviour
{
    [Header("Wander Settings")]
    public float radius = 5f;              // Max distance from the origin
    public float moveSpeed = 2f;           // Speed toward the target
    public float pauseTime = 1.5f;         // Time before choosing a new point

    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If we've reached the target or timer expires, pick a new one
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f || timer >= pauseTime)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        Vector3 randomDirection = Random.onUnitSphere * radius;
        targetPosition = randomDirection;
        timer = 0f;
    }
}