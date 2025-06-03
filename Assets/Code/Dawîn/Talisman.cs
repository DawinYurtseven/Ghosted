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

    [SerializeField] private float timer = 1f;

    public void Initialize(talismanMode mode, Emotion emotion)
    {
        this.mode = mode;
        this.emotion = emotion;
    }

    public IEnumerator MoveTowards(TalismanTargetMock target)
    {
        //Should we probably do this with DoTween or so?
        var startPos = transform.position;
        var clock = 0f;
        while (clock < timer)
        {
            clock += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, target.transform.position, clock / timer);
            yield return null;
        }
        
        target.Bind();
        
        Destroy(gameObject);
    }
    
    public IEnumerator MoveTowardsPlayer (CharacterControllerMockup player)
    {
        //Should we probably do this with DoTween or so?
        var startPos = transform.position;
        var clock = 0f;
        while (clock < timer)
        {
            clock += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, player.transform.position, clock / timer);
            yield return null;
        }
        Destroy(gameObject);
    }
}