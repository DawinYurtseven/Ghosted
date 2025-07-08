using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : EmotionAbstract
{
    [SerializeField] private GameObject openState, closedState, shadow;


    public bool lockedInFear = false;
    protected override void EmotionalBehaviour()
    {
        base.EmotionalBehaviour();
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
}
