using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum talismanMode
{
    emotions,
    bind
}
[RequireComponent(typeof(Collider))]
public class Talisman : MonoBehaviour
{
    private talismanMode mode;
    private Emotion emotion;

    [SerializeField] private float speed = 50f;
    
    public void Initialize(talismanMode mode, Emotion emotion)
    {
        this.mode = mode;
        this.emotion = emotion;
    }
    
    public IEnumerator MoveTowards(TalismanTargetMock target)
    {

        while (Vector3.Distance(transform.position, target.transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }
        switch (mode)
        {
            case talismanMode.bind:
                target.Bind();
                break;
            case talismanMode.emotions:
                target.EvokeEmotion(emotion);
                break;
        }
        
        Destroy(gameObject);
    }
}
