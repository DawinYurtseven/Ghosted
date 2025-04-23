using System;
using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TalismanTargetMock : MonoBehaviour
{
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image lockOnImage;
    // Start is called before the first frame update
    Camera cam;
    Collider objCollider;
    Image lockOnImageComponent;

    void Start()
    {
        cam = Camera.main;
        objCollider =  GetComponent<Collider>();
        StartCoroutine(CheckAvailability());
        
        //Emotion subscribe
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                if (!locked) currentEmotion = emotion;
                surroundEmotion = emotion;
                EmotionalBehaivour();
            });
    }
    
    private void OnEnable()
    {
        lockOnImage = lockOnImage.GetComponent<Image>();
        emotionText.text = currentEmotion.ToString();
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
        if(interact) return;
        lockOnImage.color = Color.red;
    }

    private bool interact;
    public void HighlightInteract()
    {
        interact = true;
        lockOnImage.color = Color.yellow;
    }
    
    public void UnHighlight()
    {
        interact = false;
        lockOnImage.color = Color.white;
    }

    #endregion
    
    #region Emotion
    
    /*
     * This is the emotion areas. this will only have the logic of what to do when it is in an emotion
     * the emotion itself will be declared in the singleton
     * 
     */
    private Emotion currentEmotion, surroundEmotion;
    private bool locked;
    [SerializeField] private TextMeshProUGUI emotionText;

    private void ResetEmotion()
    {
        Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<Collider>(), GetComponent<Collider>(), false);
        locked = false;
    }

    public void ResetObject()
    {
        currentEmotion = surroundEmotion;
        EmotionalBehaivour();
    }
    
    private void EmotionalBehaivour()
    {
        ResetEmotion();
        switch (currentEmotion)
        {
            case Emotion.Lonely:
                Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<Collider>(), GetComponent<Collider>(), true );
                emotionText.text = "Lonely";
                //copy code for seethrough material
                break;
            case Emotion.Joy:
                emotionText.text = "Joy";
                break;
            default:
                emotionText.text = "";
                break;
        }
    }

    public void Bind()
    {
        print("bind");
        if (locked)
        {
            currentEmotion = surroundEmotion;
            locked = false;
        }
        else
        {
            locked = true;
        }
    }

    public void EvokeEmotion(Emotion emotion)
    {
        currentEmotion = emotion;
        EmotionalBehaivour();
    }

    #endregion

}
