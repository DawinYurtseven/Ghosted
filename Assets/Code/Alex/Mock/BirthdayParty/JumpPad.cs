using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float force = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
