using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joy : EmotionAbstract
{
    
    [SerializeField] private float force = 100f;
    [SerializeField] private Material turnedOff, turnedOn;
    private Renderer _renderer;
    private Collider _col;

    private void OnEnable()
    {
        //TODO: Rework later a bit to not be put on the collider object and instead on the parent object
        _col = gameObject.GetComponent<Collider>();
        _renderer = gameObject.GetComponentInParent<MeshRenderer>();
        if (!_col || !_renderer) {
            Debug.Log ("Collider missing on JumpPad!");
            enabled = false;
        }
    }
    
    private void OnTriggerEnter (Collider other) {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        Debug.Log("Present!");
        if (rb != null) {
            rb.velocity += (Vector3.up * force);
            Debug.Log("Added force!");
        }
    }
    //This is as far as I copy for now. I am sleepy and will work properly tomorrow. if you guys want to say anything, 
    //beat my sorry ass first so I listen!
    
    
    // or maybe just get me a coffee... (-_- )
    
    protected override void EmotionalBehaviour()
    {
        base.EmotionalBehaviour();
        if (!locked) {
            if (currentEmotion == Emotion.Joy) {
                _col.enabled = true;
                _renderer.material = turnedOn;
            }
            else {
                _col.enabled = false;
                _renderer.material = turnedOff;
            }
        }
    }

    public override void Bind()
    {
        if (locked) {
            currentEmotion = surroundEmotion;
            _col.enabled = currentEmotion == Emotion.Joy;
            _renderer.material = currentEmotion == Emotion.Joy ? turnedOn : turnedOff;
            locked = false;
        }

        else {
            locked = currentEmotion == Emotion.Joy;
        }
    }
}
