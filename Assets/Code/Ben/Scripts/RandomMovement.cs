using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float radius = 1.0f;            // Max distance for each jerk
    public float speed = 5.0f;             // How quickly it moves along the curve
    public float timeBetweenTargets = 0.5f;

    private Vector3 startPoint;
    private Vector3 controlPoint;
    private Vector3 endPoint;
    private float t = 0f;
    private float timer = 0f;

    void Start()
    {
        startPoint = transform.position;
        GenerateNewTarget();
    }

    void Update()
    {
        t += Time.deltaTime * speed;

        // Quadratic Bezier curve interpolation
        Vector3 a = Vector3.Lerp(startPoint, controlPoint, t);
        Vector3 b = Vector3.Lerp(controlPoint, endPoint, t);
        transform.position = Vector3.Lerp(a, b, t);

        if (t >= 1f)
        {
            startPoint = endPoint;
            GenerateNewTarget();
            t = 0f;
        }
    }

    void GenerateNewTarget()
    {
        // Stay on YZ plane (keep x = constant)
        float newY = transform.position.y + Random.Range(-radius, radius);
        float newZ = transform.position.z + Random.Range(-radius, radius);
        float xFixed = transform.position.x;

        endPoint = new Vector3(xFixed, newY, newZ);

        // Control point between start and end, with slight bend
        float midY = (startPoint.y + newY) / 2f + Random.Range(-radius * 0.5f, radius * 0.5f);
        float midZ = (startPoint.z + newZ) / 2f + Random.Range(-radius * 0.5f, radius * 0.5f);
        controlPoint = new Vector3(xFixed, midY, midZ);
    }
}
