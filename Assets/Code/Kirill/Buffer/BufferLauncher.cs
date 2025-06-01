using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferLauncher : MonoBehaviour
{
    public Rigidbody projectile;
    public Vector3 offset = Vector3.forward;
    [Range(0, 100)] public float velocity = 10;

    [ContextMenu("Fire")]
    public void Fire()
    {
        var body = Instantiate(
            projectile, 
            transform.TransformPoint(offset), 
            transform.rotation);
        body.velocity = Vector3.forward * velocity;
    }
}
