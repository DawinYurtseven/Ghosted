using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingCherry : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Degrees per second

    public float pushForce = 3f;

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
{
    
    Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
    if (playerRb != null)
    {
        Vector3 pushDirection = collision.contacts[0].normal * -1f;
        playerRb.AddForce(pushForce * pushDirection, ForceMode.Impulse);
    }
    
}
}
