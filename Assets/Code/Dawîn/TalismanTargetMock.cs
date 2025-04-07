using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TalismanTargetMock : MonoBehaviour
{
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image lockOnImage;
    // Start is called before the first frame update
    Camera cam;
    Collider objCollider;

    void Start()
    {
        cam = Camera.main;
        objCollider =  GetComponent<Collider>();
        StartCoroutine(CheckAvailability());
    }


    #region Target

    private bool CheckIfInFrustum()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, objCollider.bounds);
    }

    private bool isVisible = false;

    private IEnumerator CheckAvailability()
    {
        while(CheckIfInFrustum() == isVisible)
        {
            canvas.transform.LookAt(cam.transform);
            yield return null;
        }
        isVisible = !isVisible;
        if (isVisible)
        {
            EmotionSingletonMock.Instance.AddTarget(this);
        }
        else
        {
            EmotionSingletonMock.Instance.RemoveTarget(this);
        }
        StartCoroutine(CheckAvailability());
    }

    public void Highlight()
    {
        lockOnImage.GetComponent<Image>().color = Color.red;
    }
    
    public void UnHighlight()
    {
        lockOnImage.GetComponent<Image>().color = Color.white;
    }

    #endregion
    
    #region Emotion
    
    /*
     * This is the emotion areas. this will only have the logic of what to do when it is in an emotion
     * the emotion itself will be declared in the singleton
     * 
     */
    
    [SerializeField] private Emotion emotion;

    private void EmotionalBehaivour()
    {
        
    }
    
    #endregion
    
}
