using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : EmotionAbstract
{
    [SerializeField] private GameObject openState, closedState, shadow;

    protected override void EmotionalBehaivour()
    {
        base.EmotionalBehaivour();
        if (!locked) {
            if (currentEmotion == Emotion.Fear) {
                closedState.SetActive(false);
                openState.SetActive(true);   
            }
            else {
                closedState.SetActive(true);
                openState.SetActive(false);
            }
        }
        shadow.SetActive(currentEmotion == Emotion.Fear);
    }

    public override void Bind()
    {
        if (locked) {
            currentEmotion = surroundEmotion;
            closedState.SetActive(currentEmotion == Emotion.Joy);
            openState.SetActive(currentEmotion != Emotion.Joy);
            locked = false;
        }

        else {
            locked = currentEmotion != Emotion.Joy;
        }
    }
}
