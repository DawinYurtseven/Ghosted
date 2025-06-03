using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joy : EmotionAbstract
{
    
    [SerializeField] private float force = 100f;
    [SerializeField] private Material turnedOff, turnedOn;
    private Renderer _renderer;
    private Collider _col;
    private GameObject parent;


    private void OnEnable()
    {
        //TODO: Rework later a bit to not be put on the collider object and instead on the parent object
        _col = gameObject.GetComponent<Collider>();
        _renderer = gameObject.GetComponentInParent<MeshRenderer>();
        parent = gameObject.transform.parent.gameObject;
        if (!_col || !_renderer) {
            Debug.Log ("Collider missing on JumpPad!");
            enabled = false;
        }
    }
    
    private void OnTriggerEnter (Collider other) {
        // if(other.gameObject.TryGetComponent(typeof(CharacterControllerMockup), out var it)) {
        //     Debug.Log("Player entered the Joy trigger!");
        //     var controller = (CharacterControllerMockup)it;
        //     //I have done it earlier just with the fact, that Jumppad is not on "ground" layer and therefore
        //     //player cannot jump on it at all when in joy, because rn I can think of something like 
        //     //"Player hits "Jump" and it is saved as trigger before setInAir is called" or so
        //     controller.SetInAir();
        // }
        
        // to check if object is properly on the jump pad
        if(Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit)) {
            
            var minBoundLocal = _col.bounds.min  + new Vector3(0.002f, 0f, 0.002f);
            var maxBoundLocal = _col.bounds.max  - new Vector3(0.002f, 0, 0.002f);
            if (hit.point.y <= maxBoundLocal.y && hit.point.x >= minBoundLocal.x && hit.point.x <= maxBoundLocal.x && hit.point.z >= minBoundLocal.z && hit.point.z <= maxBoundLocal.z) {
                Debug.Log("Hit detected!");
            }
        }
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        Debug.Log("Present!");
        if (rb != null) {
            rb.velocity = Vector3.zero;
            rb.velocity += (Vector3.up * force);
            Debug.Log("Added force!");
        }
    }
    //This is as far as I copy for now. I am sleepy and will work properly tomorrow. if you guys want to say anything, 
    //beat my sorry ass first so I listen!
    
    
    // or maybe just get me a coffee... (-_- )
    
    protected override void EmotionalBehaviour()
    {
        // Why would you want to Reset Emotion?
        base.EmotionalBehaviour();
        if (!locked) {
            if (surroundEmotion == Emotion.Joy) {
                _col.enabled = true;
                _renderer.material = turnedOn;
                parent.layer = LayerMask.NameToLayer("JumpPad");
            }
            else {
                _col.enabled = false;
                _renderer.material = turnedOff;
                parent.layer = LayerMask.NameToLayer("Default");
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
            
            if (specialEffect)
            {
                specialEffect.SetActive(false);
            }
        }

        else {
            //basically it can be locked in the other state as well
            //locked = currentEmotion == Emotion.Joy;
            locked = true;
            if (specialEffect)
            {
                specialEffect.SetActive(true);
            }
        }
    }
}
