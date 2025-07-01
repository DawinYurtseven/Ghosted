using UnityEngine;
using DG.Tweening;

public class ClockSlideOpenAnim : MonoBehaviour
{
    public GameObject clockOpen; 
    public GameObject clockClosed;
    public GameObject clockLeft;
    public GameObject clockRight;
    public bool repeatable = false;
    public float slideDistance = 1.0f;
    public float slideDuration = 1.0f;
    
    public Vector3 leftTargetPosition;
    public Vector3 rightTargetPosition;
    private Vector3 originalLeftPosition;
    private Vector3 originalRightPosition;
    private bool wasOpen = false;
    
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
        Debug.Log("Animating slide open");
        
        // Calculate target positions
        leftTargetPosition = clockLeft.transform.position + new Vector3(-slideDistance, 0, 0);
        rightTargetPosition = clockRight.transform.position + new Vector3(slideDistance, 0, 0);
        
        if(clockClosed!= null)
        {
            // Disable the closed clock if it exists
            clockClosed.SetActive(false);
            clockOpen.SetActive(true);
        }
        
        // Animate the left clock to the left and the right clock to the right
        Sequence slideSequence = DOTween.Sequence();
        slideSequence.Append(clockLeft.transform.DOMove(leftTargetPosition, slideDuration).SetEase(Ease.OutQuad));
        slideSequence.Join(clockRight.transform.DOMove(rightTargetPosition, slideDuration).SetEase(Ease.OutQuad));
        
    }
    
    public void animateSlideClose()
    {
        Debug.Log("Animating slide close");
        
        // Animate the left clock back to its original position and the right clock back to its original position
        Sequence slideSequence = DOTween.Sequence();
        slideSequence.Append(clockLeft.transform.DOMove(originalLeftPosition, slideDuration).SetEase(Ease.InQuad));
        slideSequence.Join(clockRight.transform.DOMove(originalRightPosition, slideDuration).SetEase(Ease.InQuad));
        
        if (clockClosed != null)
        {
            slideSequence.OnComplete(
                () =>
                {
                    clockClosed.SetActive(true);
                    clockOpen.SetActive(false);
                });
        }
    }
    
    public void animateSlide(bool open)
    {
        if (open || repeatable)
        {
            if(!wasOpen || repeatable)
                animateSlideOpen();
            wasOpen = true;
        }
        else
        {
            animateSlideClose();
        }
    }
    
    
    // draw Gizmos to visualize the slide distance in the editor
    private void OnDrawGizmos()
    {
        if (clockLeft != null && clockRight != null && slideDistance > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(clockLeft.transform.position,
                clockLeft.transform.position + new Vector3(-slideDistance, 0, 0));
            Gizmos.DrawLine(clockRight.transform.position,
                clockRight.transform.position + new Vector3(slideDistance, 0, 0));
        }
    }
}
