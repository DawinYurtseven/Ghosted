using UnityEngine;

public class Confetti : MonoBehaviour
{
    public GameObject confettiPrefab; // Prefab for the confetti particles
    public Vector3 spawnOffset;
    public Vector3 spawnRotation;
    
    public void fireConfetti()
    {
        // Check if the confetti prefab is assigned
        if (confettiPrefab == null)
        {
            Debug.LogWarning("Confetti prefab is not assigned!");
            return;
        }

        // Instantiate the confetti at the current position with an offset
        Vector3 spawnPosition = transform.position + spawnOffset;
        Quaternion spawnQuaternion = Quaternion.Euler(spawnRotation);
        GameObject confettiInstance = Instantiate(confettiPrefab, spawnPosition, spawnQuaternion);

        // Optionally, you can set the parent of the confetti instance to this object
        confettiInstance.transform.SetParent(transform);
        
        confettiInstance.GetComponent<ParticleSystem>().Play();
        
        // destroy the confetti instance after its duration
        Destroy(confettiInstance, confettiInstance.GetComponent<ParticleSystem>().main.duration + 1f);
    }
}
