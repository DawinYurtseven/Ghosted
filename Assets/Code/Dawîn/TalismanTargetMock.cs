using System.Collections;
using TMPro;
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
    Image lockOnImageComponent;
    
    private void Start()
    {
        cam = Camera.main;
        if (!cam)
        {
            Debug.LogError("No main camera found. Please assign a camera with the 'MainCamera' tag.");
            return;
        }
        
        objCollider = GetComponent<Collider>();
        if (!objCollider)
        {
            Debug.LogError("No collider found on the TalismanTargetMock"+ this.name + " object.");
            return;
        }

        lockOnImageComponent = lockOnImage.GetComponent<Image>();
        
        // Start checking visibility
        StartCoroutine(CheckAvailability());
        
        //Emotion subscribe
        EmotionSingletonMock.Instance.EmotionSubject
            .Subscribe(emotion =>
            {
                if (!locked) currentEmotion = emotion;
                surroundEmotion = emotion;
                EmotionalBehaviour();
            });
        EmotionalBehaviour();
    }
    
    private void OnEnable()
    {
        lockOnImage = lockOnImage.GetComponent<Image>();
        emotionText.text = currentEmotion.ToString();
        
        //copied from start but test first cam = Camera.main;
        objCollider =  GetComponent<Collider>();
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

    public void turnOff()
    {
        interact = false;
        lockOnImage.enabled = false;
    }
    
    public void turnOn()
    {
        lockOnImage.enabled = true;
    }

    #endregion
    
    #region Emotion
    
    /*
     * This is the emotion areas. this will only have the logic of what to do when it is in an emotion
     * the emotion itself will be declared in the singleton
     * 
     */
    protected Emotion currentEmotion;
    protected Emotion surroundEmotion;
    protected bool locked;
    [SerializeField] private TextMeshProUGUI emotionText;
    
    public bool GetLocked()
    {
        return locked;
    }
    private void ResetEmotion()
    {
        //work on fear properly with ignore tags maybe? or just cheat like now if objects other than player should fall through
        //Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<Collider>(), GetComponent<Collider>(), false);
        locked = false;
    }

    public void ResetObject()
    {
        currentEmotion = surroundEmotion;
        EmotionalBehaviour();
    }

    private void EmotionalBehaivour()
    {
        Debug.Log("Doing smth?");
    }

    protected virtual void EmotionalBehaviour()
    {
        //ResetEmotion();
        /*switch (currentEmotion)
        {
            case Emotion.Fear:
                gameObject.AddComponent<Fear>();
                Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<Collider>(), GetComponent<Collider>(), true );
                emotionText.text = "Lonely";
                //copy code for seethrough material
                break;
            case Emotion.Joy:
                gameObject.AddComponent<Joy>();
                emotionText.text = "Joy";
                break;
            case Emotion.Fear:
                emotionText.text = "Fear";
                break;
            default:
                emotionText.text = "";
                break;
        }*/
    }

    public virtual void Bind()
    {
        print("Target bind");
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

    #endregion
    
    
}
