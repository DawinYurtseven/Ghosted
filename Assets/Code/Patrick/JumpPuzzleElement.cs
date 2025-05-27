using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class JumpPuzzleElement : MonoBehaviour
{
    public int objectID = 0;              // 0,1,2
    public PuzzleMock manager;

    [FormerlySerializedAs("jumpStrength")]
    [SerializeField] private GameObject animObj;
    [Header("Animation Settings")]
    [SerializeField] private float animJumpStrength = 0.5f;
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private bool useColorAnim = false;
    [SerializeField] private GameObject indicator;
    
    private Vector3 originalPos;
    private Renderer rend;
    private Color defaultColor;
    
    // Setup
    void Awake()
    {
        manager = manager ? manager : FindObjectOfType<PuzzleMock>();
        originalPos = animObj.transform.position;
        
        rend = indicator? indicator.GetComponent<Renderer>() : animObj.GetComponent<Renderer>();
        if (rend) defaultColor = rend.material.color;
    }

    //Subscribe for win animation
    private void OnEnable()
    {
        manager?.puzzleSolved.AddListener(animateSol);
    }

    private void OnDisable()
    {
        manager?.puzzleSolved.RemoveListener(animateSol);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Logic
            manager.registerStep(objectID);
            
            //Animation
            if(!manager.WasSolved() || manager.isRepeatable)
                animate(manager.isSolved(), manager.isSolution());

        }
    }

    private void animateSol()
    {
        if(useColorAnim)
            animate(true,true);
    }
    
    private void animate(bool correct, bool timeToShowColor)
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
