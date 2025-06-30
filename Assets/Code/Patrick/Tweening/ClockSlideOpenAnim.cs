using UnityEngine;
using DG.Tweening;

public class ClockSlideOpenAnim : MonoBehaviour
{
    public GameObject clockClosed;
    public GameObject clockLeft;
    public GameObject clockRight;
    public float slideDistance = 1.0f;
    public float slideDuration = 1.0f;
    
    public Vector3 leftTargetPosition;
    public Vector3 rightTargetPosition;
    private Vector3 originalLeftPosition;
    private Vector3 originalRightPosition;
    
    void Start()
    {
        // Store original positions
        originalLeftPosition = clockLeft.transform.position;
        originalRightPosition = clockRight.transform.position;
        
        // Initialize target positions
        leftTargetPosition = originalLeftPosition + new Vector3(-slideDistance, 0, 0);
        rightTargetPosition = originalRightPosition + new Vector3(slideDistance, 0, 0);
    }
    
    public void animateSlideOpen()
    {
        // Calculate target positions
        leftTargetPosition = clockLeft.transform.position + new Vector3(-slideDistance, 0, 0);
        rightTargetPosition = clockRight.transform.position + new Vector3(slideDistance, 0, 0);
        
        // Animate the left clock
        clockLeft.transform.DOMove(leftTargetPosition, slideDuration).SetEase(Ease.OutQuad);
        
        // Animate the right clock
        clockRight.transform.DOMove(rightTargetPosition, slideDuration).SetEase(Ease.OutQuad);
    }
    
    public void animateSlideClose()
    {
        // Animate the left clock back to its original position
        clockLeft.transform.DOMove(clockLeft.transform.position, slideDuration).SetEase(Ease.InQuad);
        
        // Animate the right clock back to its original position
        clockRight.transform.DOMove(clockRight.transform.position, slideDuration).SetEase(Ease.InQuad);
    }
    
    public void animateSlide(bool open)
    {
        if (open)
        {
            animateSlideOpen();
        }
        else
        {
            animateSlideClose();
        }
    }
    
    // draw Gizmos to visualize the slide distance in the editor
    
}
