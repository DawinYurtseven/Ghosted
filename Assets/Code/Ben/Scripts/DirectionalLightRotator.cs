using UnityEngine;

public class DirectionalLightRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 startRotation = new Vector3(51f, 117f, -38f);
    public float rotationSpeed = 10f; // degrees per second

    void Start()
    {
        transform.rotation = Quaternion.Euler(startRotation);
    }

    void Update()
    {
        // Rotate around local X-axis
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
    }
}