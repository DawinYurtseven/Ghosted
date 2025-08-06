using UnityEngine;
using DG.Tweening;

public class JumpStopHand : MonoBehaviour
{
    [SerializeField] private StopClockManager manager;
    
    [Header("Clock Hand Settings")]
    //[SerializeField] private ClockAnim clockAnim;
    [SerializeField] private ClockHand _hand;
    private bool isHandRunning = true;
    private bool puzzleActive = true;
    
    [Header("Animation Settings")]
    // For immediate feedback when jumping on the clock
    public GameObject animObj; // Object to animate on interaction
    public GameObject indicator; // Indicator for feedback
    private Vector3 originalPos; // Original position for animation
    [SerializeField] private float animJumpStrength = 0.5f;
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private float animDelay = 2f;
    public Material pressedMaterial; // Material to indicate pressed state
    
    [Header("Audio Settings")]
    public FMODUnity.EventReference feedbackSound; // Sound to play when jumping on the clock hand
    public FMODUnity.EventReference releaseSFX;
    public FMODUnity.EventReference solvedSFX;
    
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
            puzzleActive = false;
        }
        else
        {
            if (manager.lastJumpedHand == _hand)
            {
                Debug.Log("Puzzle not solved, starting hand again: " + _hand);
                isHandRunning = true;
                manager.clockAnim.onPuzzleAnimationComplete = (b) =>
                {
                    manager.clockAnim.startHand(_hand);
                    manager.clockAnim.onPuzzleAnimationComplete = null; // remove listener after use
                };
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!puzzleActive || !other.CompareTag("Player"))
            return;

        Debug.Log("Player jumped on clock hand: " + _hand);
        
        toggleHand();
        animate(false);
        manager.getInput(_hand, isHandRunning);
    }

    // Animate the clock hand when the player jumps on it
    private void animate(bool correct)
    {
        animObj.transform.DOKill(); // Stop ongoing tweens
        animObj.transform.DOMove(originalPos, 0.05f); 
        indicator.transform.DOKill();
        
        Debug.Log("Anim on clock hand: " + _hand + ", correct: " + correct);
        
        animatePlate(animDelay); // Animate the plate for feedback
        if (correct)
        {
            // Correct: strong jump up & shake
            Sequence s = DOTween.Sequence();
            s.Append(animObj.transform.DOPunchPosition(Vector3.up * animJumpStrength, animDuration, 10, 0.5f));
            s.Join(animObj.transform.DOShakeRotation(animDuration, new Vector3(8, 0, 8), 10, 90));
            
            if(!solvedSFX.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(solvedSFX, transform.position);
        }
        else
        {
            // Wrong: squish + bounce back
            Sequence s = DOTween.Sequence();
            s.Append(animObj.transform.DOScaleY(0.9f, 0.1f));
            s.Append(animObj.transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutElastic));
        }
    }

    private void animatePlate(float delay = 0)
    {
        Material tempMaterial = indicator.GetComponent<Renderer>()?.material;
        setMaterial(pressedMaterial);
        
        // Wrong: squish + bounce back
        Sequence s = DOTween.Sequence();
        s.Append(indicator.transform.DOScaleY(animJumpStrength, 0.1f)).SetEase(Ease.OutBack);
        s.AppendInterval(delay);
        s.Append(indicator.transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack));
        s.AppendCallback(() =>
        {
            setMaterial(tempMaterial);
            if (!releaseSFX.IsNull)
            {
                FMODUnity.RuntimeManager.PlayOneShot(releaseSFX, transform.position);;
            }
        });
        
        playFeedbackSound();
    }
    
    private void setMaterial(Material mat)
    {
        if (indicator.TryGetComponent<Renderer>(out Renderer rend))
        {
            rend.material = mat;
        }
        else
        {
            Debug.LogWarning("No Renderer found on animObj to set material.");
        }
    }
    
    // just toggles the isHandRunning bool
    private void toggleHand()
    {
        if(isHandRunning)
        {
            StopHand();
        }
        else
        {
            StartHand();
        }
    }

    private void StopHand()
    {
        if (!isHandRunning)
        {
            Debug.Log("Hand is already stopped: " + _hand);
            return;
        }
        
        isHandRunning = false;
        Debug.Log("Stopped hand: " + _hand);
    }
    
    private void StartHand()
    {
        if (isHandRunning)
        {
            Debug.Log("Hand is already running: " + _hand);
            return;
        }
        
        isHandRunning = true;
        Debug.Log("Started hand: " + _hand);
    }
    
    private void playFeedbackSound()
    {
        if (!feedbackSound.IsNull)
        {
            FMODUnity.RuntimeManager.PlayOneShot(feedbackSound, transform.position);;
        }
    }
    
    public ClockHand getClockHand()
    {
        return _hand;
    }
    
}
