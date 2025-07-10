using DG.Tweening;
using UnityEngine;

public abstract class JumpPuzzle : MonoBehaviour
{
    public int objectID = 0;              
    [SerializeField] protected GameObject animObj;
    [SerializeField] protected GameObject indicator;
    
    [Header("Animation Settings")]
    [SerializeField] private float animJumpStrength = 0.5f;
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private bool useColorAnim = false;
    
    [Header("Feedback")]
    [SerializeField] protected AudioSource effect;
    [SerializeField] protected AudioSource solvedSFX;
    
    // things for animation
    protected Vector3 originalPos;
    protected Renderer rend;
    protected Color defaultColor;
    
    // Jump on it
    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }
    
    
    // Animate 
    protected void animateSol()
    {
        if(useColorAnim)
            animate(true,true);
        
        //Audio
        effect?.PlayOneShot(solvedSFX.clip);
    }
    
    protected void animate(bool correct, bool timeToShowColor)
    {
        animObj.transform.DOKill(); // Stop ongoing tweens
        animObj.transform.DOMove(originalPos, 0.05f); // Reset position
        
        // Optional color flash
        if (rend && useColorAnim && timeToShowColor)
        {
            Color flashColor = correct ? correctColor : wrongColor;
            rend.material.DOColor(flashColor, animDuration).OnComplete(() =>
                rend.material.DOColor(defaultColor, 0.3f));
        }
        
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
}
