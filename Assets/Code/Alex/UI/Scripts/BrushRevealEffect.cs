using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class BrushRevealEffect : MonoBehaviour
{
    [SerializeField] private Material brushMaterial; 
    public float duration = 3f;

    public bool playReveal = false;
    
    void Update()
    {
        if (playReveal)
        {
            playReveal = false;
            DOTween.To(
                () => brushMaterial.GetFloat("_Progress"),     
                value => brushMaterial.SetFloat("_Progress", value),  
                1f,        
                duration      
            ).SetEase(Ease.OutQuad);
        }
    }
    
    
    
    private void Start()
    {
        // Reset progress
        brushMaterial.SetFloat("_Progress", 0f);

        
    }
}
