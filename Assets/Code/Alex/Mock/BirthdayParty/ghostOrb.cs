using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ghostOrb : MonoBehaviour
{
    
    //TO-DO: maybe as nav agent or so?
    [SerializeField] private List<Transform> waypoints; // Set these in Inspector
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private Ease moveEase = Ease.InOutSine;

    [SerializeField] private int currentIndex = 0;
    private Tween currentTween;

    public void MoveToNextPoint()
    {
        Debug.Log("Ghost moved");
        
        if (waypoints == null || waypoints.Count == 0) return;

        currentIndex = (currentIndex + 1) % waypoints.Count; 
        Vector3 targetPosition = waypoints[currentIndex].position;

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(); // Stop current movement if needed
        }

        currentTween = transform.DOMove(targetPosition, moveDuration).SetEase(moveEase);
        
    }
}
