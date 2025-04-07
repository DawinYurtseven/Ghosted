using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TalismanTargetKirillMock : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image lockOnImage;
    // Start is called before the first frame update
    Camera cam;
    Collider objCollider;
    private EmotionEnumKirillMock curEmotion;

    void Start()
    {
        curEmotion = EmotionEnumKirillMock.JOY;
        ApplyNewEmotion();
        cam = Camera.main;
        objCollider =  GetComponent<Collider>();
        StartCoroutine(CheckAvailability());
    }
    
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
            yield return null;
        }
        isVisible = !isVisible;
        if (isVisible)
        {
            EmotionSingletonKirillMock.Instance.AddTarget(this);
        }
        else
        {
            EmotionSingletonKirillMock.Instance.RemoveTarget(this);
        }
        StartCoroutine(CheckAvailability());
    }

    public void Highlight()
    {
        print("yes");
        lockOnImage.GetComponent<Image>().color = Color.red;
    }
    
    public void UnHighlight()
    {
        lockOnImage.GetComponent<Image>().color = Color.white;
    }

    private void ChangeEmotion (EmotionEnumKirillMock emotion) {
        if (curEmotion != emotion) {
            curEmotion = emotion;
            ApplyNewEmotion();
        }
    }

    private void ApplyNewEmotion() {
        switch (curEmotion) {
            case EmotionEnumKirillMock.JOY:
                lockOnImage.color = Color.green;
                Debug.Log("Bring Joy to the world!");
                break;
            case EmotionEnumKirillMock.LONELINESS:
                lockOnImage.color = Color.gray;
                Debug.Log("Bring Loneliness to the world!");
                break;
            case EmotionEnumKirillMock.FEAR:
                lockOnImage.color = Color.white;
                Debug.Log("Bring Fear to the world!");
                break;
            case EmotionEnumKirillMock.LOVE:
                lockOnImage.color = Color.magenta;
                Debug.Log("Bring Love to the world!");
                break;
        }
    }
}
