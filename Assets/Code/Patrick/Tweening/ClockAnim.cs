using UnityEngine;
using DG.Tweening;
public enum ClockHand
{
    Hour,
    Minute,
    Second
}

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
    
    public bool isClockRunning = true; // Flag to control clock running state
    
    [Header("Animation Settings")]
    public Material wrongMaterial; // Material to indicate wrong solution
    
    private Tween hourTween;
    private Tween minuteTween;
    private Tween secondTween;
    protected float hourOffset = 0f; // Offset for hour hand rotation
    protected float minuteOffset = 0f; // Offset for minute hand rotation
    protected float secondOffset = 0f; // Offset for second hand rotation
    
    private void Awake()
    {
        // Ensure the clock hands are set
        if (hourHand == null || minuteHand == null || secondHand == null)
        {
            Debug.LogError("Clock hands are not assigned in the ClockAnim script.");
        }
    }
    
    public virtual void Start()
    {
        hourOffset = hourHand.localRotation.eulerAngles.y;
        minuteOffset = minuteHand.localRotation.eulerAngles.y;
        secondOffset = secondHand.localRotation.eulerAngles.y;

        // Debug.Log("Set offsets for clock " + this.gameObject.name + ": " +
        //           $"Hour Offset: {hourOffset}, " +
        //           $"Minute Offset: {minuteOffset}, " +
        //           $"Second Offset: {secondOffset}");
        
        AnimateZeiger(minuteHand, minuteHandSpeed);
        AnimateZeiger(hourHand, hourHandSpeed);
        AnimateZeiger(secondHand, secondHandSpeed);
    }

    public void AnimateZeiger(Transform hand, float speed)
    {
        float fullRotationTime = 360f / speed;
        // Animate the clock hand to rotate continuously around the specified axis
        Tween tween = hand.DOLocalRotate(hand.localEulerAngles + new Vector3(0, 360f,0), 
                fullRotationTime, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        
        switch (hand)
        {
            case var _ when hand == hourHand:
                hourTween = tween;
                break;
            case var _ when hand == minuteHand:
                minuteTween = tween;
                break;
            case var _ when hand == secondHand:
                secondTween = tween;
                break;
            default:
                Debug.Log("Animation not set for hand: " + hand.name);
                break;
        }

        isClockRunning = true;
    }
    
    public void StartAnimation()
    {
        // Start the animations for the clock hands
        AnimateZeiger(hourHand, hourHandSpeed);
        AnimateZeiger(minuteHand, minuteHandSpeed);
        AnimateZeiger(secondHand, secondHandSpeed);
        isClockRunning = true; 
    }
    
    public void stopAnimation()
    {
        // Stop all animations on the clock hands
        hourHand.DOKill();
        minuteHand.DOKill();
        secondHand.DOKill();
        isClockRunning = false; 
    }
    
    public void setTime(int hour, int minute, int second = 0)
    {
        // Set the clock hands to a specific time
        float hourRotation = (hour % 12) * 30f + (minute / 60f) * 30f; // 30 degrees per hour
        float minuteRotation = minute * 6f; // 6 degrees per minute
        float secondRotation = second * 6f; // again, 6 degrees per second
        
        
        hourHand.localRotation = Quaternion.Euler(rotationAxis * hourRotation);
        minuteHand.localRotation = Quaternion.Euler(rotationAxis * minuteRotation);
        secondHand.localRotation = Quaternion.Euler(rotationAxis * secondRotation);

        isClockRunning = false;
        
        Debug.Log("Set time to: " + hour + ":" + minute + ":" + second);
    }

    public void setHand(ClockHand hand, int time)
    {
        
        if (ClockHand.Hour == hand)
        {
            float hourRotation = (time % 12) * 30f; // 30 degrees per hour
            hourHand.localRotation = Quaternion.Euler(rotationAxis * hourRotation);
        }
        else
        {
            float secondRotation = time * 6f; // 6 degrees per second
            secondHand.localRotation = Quaternion.Euler(rotationAxis * secondRotation);
        } 
        
        checkClockStopped();
        Debug.Log("Set " + hand + " hand to: " + time);
    }
    
    public void stopHand(ClockHand hand)
    {
        // Stop the animation of a specific clock hand
        switch (hand)
        {
            case ClockHand.Hour:
                hourHand.DOKill();
                break;
            case ClockHand.Minute:
                minuteHand.DOKill();
                break;
            case ClockHand.Second:
                secondHand.DOKill();
                break;
        }
        
        checkClockStopped();
    }
    
    public void startHand(ClockHand hand)
    {
        // Start the animation of a specific clock hand
        switch (hand)
        {
            case ClockHand.Hour:
                AnimateZeiger(hourHand, hourHandSpeed);
                break;
            case ClockHand.Minute:
                AnimateZeiger(minuteHand, minuteHandSpeed);
                break;
            case ClockHand.Second:
                AnimateZeiger(secondHand, secondHandSpeed);
                break;
        }
    }
    
    public int[] getCurrentTime()
    {
        // Adjusted angles
        float hourAngle = (hourHand.localEulerAngles.y ) % 360f;
        float minuteAngle = (minuteHand.localEulerAngles.y ) % 360f;
        float secondAngle = (secondHand.localEulerAngles.y ) % 360f;
        
        // Get the current time from the clock hands
        int hour = Mathf.RoundToInt(hourAngle / 30f) % 12;    // 30 degrees per hour
        int minute = Mathf.RoundToInt(minuteAngle / 6f) % 60;      // 6 degrees per minute
        int second = Mathf.RoundToInt(secondAngle / 6f) % 60;
        
        // Ensure hour is in the range [1, 12]
        if (hour == 0) hour = 12;       // Convert 0 to 12 for clock representation
        // if (minute < 0) minute += 60;   // Ensure minute is in the range [0, 59]
        // if (second < 0) second += 60;   // Ensure second is in the range [0, 59]
        
        //Debug.Log($"Current Time: {hour:D2}:{minute:D2}:{second:D2}");
        return new []{hour, minute, second};
    }
    
    private void checkClockStopped()
    {
        // all hands dont move
        isClockRunning = hourTween.IsActive() || minuteTween.IsActive() || secondTween.IsActive();
    }
    
    public void AnimateSolution(bool solved, int hour, int minute, int second = 0)
    {
        // Stop the current animations
        stopAnimation();
        
        // If the puzzle is solved, set the clock hands to the solution time
        if (solved)
        {
            setTime(hour, minute, second);
        }
        else
        {
            
            AnimateNotSolved(hour, minute, second);
        }
    }
    
    private void AnimateNotSolved(int hour, int minute, int second = 0)
    {
        setTime(hour, minute, second);

        var hourRenderer = hourHand.GetChild(0).GetComponent<Renderer>();
        var minuteRenderer = minuteHand.GetChild(0).GetComponent<Renderer>();
        var secondRenderer = secondHand.GetChild(0).GetComponent<Renderer>();

        Material originalHourMat = hourRenderer.material;
        Material originalMinuteMat = minuteRenderer.material;
        Material originalSecondMat = secondRenderer.material;
        
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f) // wait
            .AppendCallback(() =>
            {
                // turn red
                hourRenderer.material = wrongMaterial;
                minuteRenderer.material = wrongMaterial;
                secondRenderer.material = wrongMaterial;
            })
            .Append(WiggleHands(0.6f)) // wiggle duration
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                // restore color
                hourRenderer.material = originalHourMat;
                minuteRenderer.material = originalMinuteMat;
                secondRenderer.material = originalSecondMat;
            })
            .AppendInterval(0.5f)
            .AppendCallback(StartAnimation);
    }
    
    private Sequence WiggleHands(float duration)
    {
        float strength = 10f;
        
        Sequence wiggle = DOTween.Sequence();

        wiggle.Join(hourHand.DOShakeRotation(duration, strength, vibrato: 10, randomness: 90))
            .Join(minuteHand.DOShakeRotation(duration, strength, vibrato: 10, randomness: 90))
            .Join(secondHand.DOShakeRotation(duration, strength, vibrato: 10, randomness: 90));

        return wiggle;
    }
}
