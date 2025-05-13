using UnityEngine;

public class JumpPad : Lockable
{
    public float force = 100f;

    State currentState;

    bool _locked = false;

    Collider col;

    public GameObject specialEffect;


    public Material  turnedOff, turnedOn;
    private Renderer renderer;

    private GameObject parent;

    private void OnEnable()
    {
        StateManagerMock.OnStateChanged += changeState;
        col = this.gameObject.GetComponent<BoxCollider>();
        parent = gameObject.transform.parent.gameObject;
        renderer = parent.GetComponent<MeshRenderer>();
        if (!col || ! renderer || !parent) {
            Debug.Log ("Collider missing on JumpPad!");
            this.enabled = false;
        }
    }


    private void OnDisable()
    {
        StateManagerMock.OnStateChanged -= changeState;
    }


    private void changeState(State newState)
    {   if (!_locked) {
            if (newState == State.Joy) {
                col.enabled = true;
                renderer.material = turnedOn;
                parent.layer = LayerMask.NameToLayer("JumpPad");
            }

            else {
                col.enabled = false;
                renderer.material = turnedOff;
                parent.layer = LayerMask.NameToLayer("Default");
            }
        }

        currentState = newState;
    }

    private void OnTriggerEnter (Collider other) {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        
        // to check if object is properly on the jump pad
        if(Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit)) {
            
            var minBoundLocal = col.bounds.min  + new Vector3(0.002f, 0f, 0.002f);
            var maxBoundLocal = col.bounds.max  - new Vector3(0.002f, 0, 0.002f);
            if (hit.point.y <= maxBoundLocal.y && hit.point.x >= minBoundLocal.x && hit.point.x <= maxBoundLocal.x && hit.point.z >= minBoundLocal.z && hit.point.z <= maxBoundLocal.z) {
                Debug.Log("Hit detected!");
            }
        }
        
        Debug.Log("Present!");
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.velocity += (Vector3.up * force);
            Debug.Log("Added force!");
        }
    }

    public override void Lock() {
        Debug.Log("Lock");
        _locked = currentState == State.Joy;
        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
    }

    public override void Unlock()
    {
        col.enabled = currentState == State.Joy;
        renderer.material = (currentState == State.Joy)? turnedOn : turnedOff;
        _locked = false;
        if (specialEffect != null)
        {
            specialEffect.SetActive(false);
        }
        
    }

}
