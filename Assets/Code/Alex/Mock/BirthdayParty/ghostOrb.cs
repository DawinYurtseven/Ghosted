using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ghostOrb : MonoBehaviour
{
    
    [Header("Waypoints")]
    [SerializeField] private List<Transform> waypoints; // Set in inspector
    [SerializeField] private float moveDuration_WayPoint = 1.5f;
    [SerializeField] private float moveDuration_fast = 0.5f;

    private float moveDuration = 1.5f;
    [SerializeField] private Ease moveEase = Ease.InOutSine;
    [SerializeField] private float randomOffsetMagnitude = 0.5f;
    
    private int currentWaypointIndex = 0;
    private Tween currentTween;
    private Transform followTarget;
    private bool isFollowing = false;
    
    
 private void Update()
    {
        // follow the target if set
        if (isFollowing && followTarget != null)
        {
            FollowTarget(followTarget);
        }
    }

    //  Move to next waypoint
    public void MoveToNextWaypoint()
    {
        Debug.Log("Ghost moved");
        
        if (waypoints == null || waypoints.Count == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        MoveToCurrentWaypoint();
    }

    //  Move to a specific transform point (f.e. for dialogues for collectable objects)
    public void MoveToTransform(Transform target)
    {
        if (target == null) return;
        isFollowing = false;
        moveDuration = moveDuration_fast;
        MoveToPosition(target.position);
    }

    // 3Move back to the current main waypoint
    public void MoveToCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0) return;
        isFollowing = false;
        moveDuration = moveDuration_WayPoint;
        MoveToPosition(waypoints[currentWaypointIndex].position);
    }

    // Follow an object (f.e. for train)
    public void FollowObject(Transform target)
    {
        if (target == null) return;
        followTarget = target;
        isFollowing = true;
    }

    private void FollowTarget(Transform target)
    {
        // Optionally smooth or randomize movement to simulate floating
        Vector3 followPos = target.position;
        MoveToPosition(followPos, true); 
    }

    // Movement logic
    private void MoveToPosition(Vector3 targetPos, bool smoothFollow = false)
    {
        if (!smoothFollow && currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        Vector3 offsetTarget = targetPos;
        
        currentTween = transform.DOMove(offsetTarget, moveDuration)
            .SetEase(moveEase)
            .SetUpdate(UpdateType.Normal, true);
    }
    
    
    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private float floatDuration = 1.5f;
    [SerializeField] private float jitterAmount = 0.1f;


    public void StartGhostFloatAnimation()
    {
        // Stop existing tween if running
        if (currentTween != null && currentTween .IsActive())
            currentTween .Kill();

        Vector3 originalPos = transform.position;

        currentTween = DOTween.To(() => 0f, t =>
            {
                // Y bobbing
                float yOffset = Mathf.Sin(t * Mathf.PI * 2) * floatHeight;

                // Small random X/Z jitter every frame
               // float xOffset = Random.Range(-jitterAmount, jitterAmount);
              //  float zOffset = Random.Range(-jitterAmount, jitterAmount);

                transform.position = originalPos + new Vector3(0, yOffset, 0);

            }, 1f, floatDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

}
