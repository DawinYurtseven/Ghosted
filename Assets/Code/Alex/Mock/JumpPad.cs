using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float force = 100f;

    bool canJump = true;

    Collider col;

    void OnEnable()
    {
        StateManagerMock.OnStateChanged += changeState;
        col = this.gameObject.GetComponent<BoxCollider>();

        if (!col) {
            Debug.Log ("Collider missing on JumpPad!");
            this.enabled = false;
        }
    }


    void OnDisable()
    {
        StateManagerMock.OnStateChanged -= changeState;
    }


    private void changeState(State newState)
    { 
        if (newState == State.Joy) {
            col.enabled = true;
        }

        else {
            col.enabled = false;
        }
    }

    private void OnTriggerEnter (Collider other) {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        Debug.Log("Present!");
        if (rb != null) {
            rb.AddForce(Vector3.up * force);
            Debug.Log("Added force!");
        }
    }
}
