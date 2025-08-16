using UnityEngine;

public class Fear : EmotionAbstract
{
    [SerializeField] private GameObject openState, closedState, shadow;


    public bool lockedInFear = false;
    protected override void EmotionalBehaviour()
    {
        base.EmotionalBehaviour();  //does nothing, is commented out in base class
        if (!locked) {
            if (surroundEmotion == Emotion.Fear) {
                closedState.SetActive(false);
                openState.SetActive(true);   
            }
            else {
                closedState.SetActive(true);
                openState.SetActive(false);
            }
        }
        shadow.SetActive(surroundEmotion == Emotion.Fear);
    }

    public override void Bind()
    {
        if (locked) {
            Debug.Log("Unbinding " + gameObject.name + " from fear");
            currentEmotion = surroundEmotion;
            closedState.SetActive(currentEmotion != Emotion.Fear);
            openState.SetActive(currentEmotion == Emotion.Fear);
            shadow.SetActive(currentEmotion == Emotion.Fear);
            locked = false;
            lockedInFear = false;
            if (specialEffect)
            {
                specialEffect.SetActive(false);
            }
        }

        else {
            //Same as with joy, could be locked in any other state as well (just doesn't make a lot of sense)
            locked = true;
            Debug.Log("Locked " + gameObject.name + " in fear");
            if (specialEffect) {
                specialEffect.SetActive(true);
                if (surroundEmotion == Emotion.Joy) {
                    joyVFX.SetActive(true);
                    fearVFX.SetActive(false);
                }
                else {
                    joyVFX.SetActive(false);
                    fearVFX.SetActive(true);
                    lockedInFear = true;
                }
                StartCoroutine(PlayAudioSequentially());
            }
        }
    }
    
    private void OnEnable()
    {
        if (surroundEmotion == Emotion.Fear) {
            closedState.SetActive(false);
            openState.SetActive(true);
        }
        else {
            closedState.SetActive(true);
            openState.SetActive(false);
        }
        shadow.SetActive(surroundEmotion == Emotion.Fear);
    }
}
