using UnityEngine;
using DG.Tweening;

public class ClockAnim : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;
    public Vector3 rotationAxis = Vector3.back; // Default: z-axis (like a 2D clock)
    
    [Header("Clock Hand Speeds")]
    public float minuteHandSpeed = 10f; // degrees per second
    public float hourHandSpeed = 2f; // degrees per second
    public float secondHandSpeed = 15f; // degrees per second (if you want to add a second hand later)
    
    void Start()
    {
        AnimateZeiger(minuteHand, minuteHandSpeed);
        AnimateZeiger(hourHand, hourHandSpeed);
        AnimateZeiger(secondHand, secondHandSpeed);
    }

    public void AnimateZeiger(Transform hand, float speed)
    {
        float fullRotationTime = 360f / speed;
        // Animate the clock hand to rotate continuously around the specified axis
        hand.DOLocalRotate(rotationAxis * 360f, fullRotationTime, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    
    public void StartAnimation()
    {
        // Start the animations for the clock hands
        AnimateZeiger(hourHand, hourHandSpeed);
        AnimateZeiger(minuteHand, minuteHandSpeed);
        AnimateZeiger(secondHand, secondHandSpeed);
    }
    
    public void stopAnimation()
    {
        // Stop all animations on the clock hands
        hourHand.DOKill();
        minuteHand.DOKill();
        secondHand.DOKill();
    }
    
    public void setTime(float hour, float minute)
    {
        // Set the clock hands to a specific time
        float hourRotation = (hour % 12) * 30f + (minute / 60f) * 30f; // 30 degrees per hour
        float minuteRotation = minute * 6f; // 6 degrees per minute

        hourHand.rotation = Quaternion.Euler(rotationAxis * hourRotation);
        minuteHand.rotation = Quaternion.Euler(rotationAxis * minuteRotation);
        secondHand.rotation = Quaternion.Euler(rotationAxis * 0f); 
    }

    public void AnimateSolution(bool solved, float hour, float minute)
    {
        // Stop the current animations
        stopAnimation();
        
        // If the puzzle is solved, set the clock hands to the solution time
        if (solved)
        {
            setTime(hour, minute);
        }
        else
        {
            setTime(hour, minute);
            
            // save the material colors before changing them
            Color originalHourColor = hourHand.GetComponent<Renderer>().material.color;
            Color originalMinuteColor = minuteHand.GetComponent<Renderer>().material.color;
            Color originalSecondColor = secondHand.GetComponent<Renderer>().material.color;
            
            // wait for a short time before flashing the colors
            DOVirtual.DelayedCall(0.5f, () =>
            {
                // Flash red (or any other color) to indicate the wrong solution
                hourHand.GetComponent<Renderer>().material.color = Color.red;
                minuteHand.GetComponent<Renderer>().material.color = Color.red;
                secondHand.GetComponent<Renderer>().material.color = Color.red;
            }).OnComplete(() =>
            {
                // Restore the original colors after flashing
                hourHand.GetComponent<Renderer>().material.color = originalHourColor;
                minuteHand.GetComponent<Renderer>().material.color = originalMinuteColor;
                secondHand.GetComponent<Renderer>().material.color = originalSecondColor;
            }).SetDelay(0.5f).OnComplete(StartAnimation); // Restart the animations after flashing
            
        }
    }
    
    // draw the rotation axis in the scene view for debugging
    private void OnDrawGizmos()
    {
        if (hourHand != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hourHand.position, hourHand.position + rotationAxis * 2.5f);
        }
        if (minuteHand != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(minuteHand.position, minuteHand.position + rotationAxis * 2.5f);
        }
        if (secondHand != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(secondHand.position, secondHand.position + rotationAxis * 2.5f);
        }
        
        // Draw the rotation axis in the center of the clock
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + rotationAxis * 2.5f);
    }
}
