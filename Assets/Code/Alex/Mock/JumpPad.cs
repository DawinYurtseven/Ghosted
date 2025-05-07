using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float force = 100f;

    State currentState;

    bool locked = false;

    Collider col;


    public Material  turnedOff, turnedOn;
    private Renderer renderer;

    private void OnEnable()
    {
        StateManagerMock.OnStateChanged += changeState;
        col = this.gameObject.GetComponent<BoxCollider>();
        renderer = this.gameObject.GetComponentInParent<MeshRenderer>();

        if (!col || ! renderer) {
            Debug.Log ("Collider missing on JumpPad!");
            this.enabled = false;
        }
    }


    private void OnDisable()
    {
        StateManagerMock.OnStateChanged -= changeState;
    }


    private void changeState(State newState)
    {   if (!locked) {

            if (newState == State.Joy) {
                col.enabled = true;
                renderer.material = turnedOn;
            }

            else {
                col.enabled = false;
                renderer.material = turnedOff;
            }
        }

        currentState = newState;
    }

    private void OnTriggerEnter (Collider other) {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        Debug.Log("Present!");
        if (rb != null) {
            rb.velocity += (Vector3.up * force);
            Debug.Log("Added force!");
        }
    }

    public void Lock() {
        Debug.Log("Lock");
        if (locked) {
            col.enabled = currentState == State.Joy;
            renderer.material = (currentState == State.Joy)? turnedOn : turnedOff;
            locked = false;
             
        }

        else {
            locked = currentState == State.Joy;
        }
    }

}
