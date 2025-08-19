using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;
    
    [Header("Teleport Targets")]
    [Tooltip("List of teleport targets. Maximum 10 targets can be set.")]
    public List<Transform> teleportTargets = new List<Transform>();

    private void Start()
    {
        if (teleportTargets.Count <= 0)
        {
            Debug.LogWarning("No teleport targets set. Please assign at least one target in the inspector.");
        }
        if(teleportTargets.Count > 10)
        {
            Debug.LogWarning("More than 10 teleport targets set. Only the first 10 will be used.");
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject not found. Please assign it in the inspector or ensure it has the 'Player' tag.");
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < teleportTargets.Count && i < 10; i++)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl))
            {
                KeyCode numberKey = KeyCode.Alpha0 + i;
                if (Input.GetKeyDown(numberKey))
                {
                    if (player != null && teleportTargets[i] != null)
                    {
                        player.transform.position = teleportTargets[i].position;
                        player.transform.rotation = teleportTargets[i].rotation;
                    }
                }
            }
        }
    }
}
