using UnityEngine;
using DG.Tweening;

public class JumpStopHand : MonoBehaviour
{
    [SerializeField] private StopClockManager manager;
    
    [Header("Clock Hand Settings")]
    //[SerializeField] private ClockAnim clockAnim;
    [SerializeField] private ClockHand _hand;
    private bool isHandRunning = true;
    
    [Header("Animation Settings")]
    // For immediate feedback when jumping on the clock
    public GameObject animObj; // Object to animate on interaction
    public GameObject indicator; // Indicator for feedback
    private Vector3 originalPos; // Original position for animation
    private float animJumpStrength = 0.5f;
    private float animDuration = 0.3f;
    
    
    private void Awake()
    {
        if(manager == null)
        {
            manager = FindObjectOfType<StopClockManager>();
        }
        originalPos = animObj.transform.position;
    }

    private void OnEnable()
    {
        manager.PuzzleSolved.AddListener(onPuzzleSolved);
    }
    
    private void OnDisable()
    {
        manager.PuzzleSolved.RemoveListener(onPuzzleSolved);
    }
    
    private void onPuzzleSolved(bool solved)
    {
        if (solved)
        {
            isHandRunning = false;
            animate(true);
        }
        else
        {
            //Debug.Log("Puzzle not solved, hand is running again: " + _hand);
            isHandRunning = true;
        }
    }
    
    void OnTriggerEnter()
    {
        toggleHand();
        animate(false);
        manager.getInput(_hand, isHandRunning);
    }

    private void animate(bool correct)
    {
        animObj.transform.DOKill(); // Stop ongoing tweens
        animObj.transform.DOMove(originalPos, 0.05f); // Reset position

        Debug.Log("Anim on clock hand: " + _hand + ", correct: " + correct);
        
        if (correct)
        {
            // Correct: strong jump up & shake
            Sequence s = DOTween.Sequence();
            s.Append(animObj.transform.DOPunchPosition(Vector3.up * animJumpStrength, animDuration, 10, 0.5f));
            s.Join(animObj.transform.DOShakeRotation(animDuration, new Vector3(8, 0, 8), 10, 90));
        }
        else
        {
            // Wrong: squish + bounce back
            Sequence s = DOTween.Sequence();
            s.Append(animObj.transform.DOScaleY(0.9f, 0.1f));
            s.Append(animObj.transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutElastic));
        }
    }
    
    private void toggleHand()
    {
        if (isHandRunning)
        {
            isHandRunning = false;
            Debug.Log("Stopped hand: " + _hand);
        }
        else
        {
            isHandRunning = true;
            Debug.Log("Started hand: " + _hand);
        }
    }
}
