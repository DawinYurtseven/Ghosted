using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ExplosionAnim : MonoBehaviour
{
    [Header("Explosion Animation Settings")]
    [SerializeField] private GameObject[] explosionParts;
    [SerializeField] private Vector3 trainCrashForce = new Vector3(1, 0.5f, 0);
    [SerializeField] private GameObject[] partsInTheWay;
    [SerializeField] private GameObject explodingObject;
    [SerializeField] private bool withCamShake = false;
    [SerializeField] private CinemachineVirtualCamera cam2shake;
    
    [Header("Cleanup Settings")]
    [SerializeField] private bool cleanUp = false;
    [SerializeField] private float cleanupDelay;
    
    private bool isTrainCrashed = false;
    
    public void OnTrainCrash(float delay = 0f)
    {
        if(isTrainCrashed)
            return;
        
        //ChangeObstacleLayer(LayerMask.NameToLayer("NoCollision"));
        
        isTrainCrashed = true;
        Debug.Log("Simulating train crash...");
        
        if(explodingObject)
            explodingObject.SetActive(true);
        
        // Play the train animation with a delay
        DOVirtual.DelayedCall(delay, () =>
        {
            explodeParts();
        });
    }
    
    public void explodeParts()
    {
        if (explosionParts == null || explosionParts.Length == 0)
        {
            Debug.LogWarning("No wall parts set in ClockTutManager.");
            return;
        }
        
        // remove the object that is to be exploded 
        if (explodingObject != null)
        {
            explodingObject.SetActive(false);
        }
        
        if(withCamShake)
            StartCoroutine(SpawnAnim.shakeVirtualCamera(cam2shake));
        
        // explode the parts
        foreach (GameObject part in explosionParts)
        {
            part.SetActive(true);
            SpawnAnim.simulatePhysics(part.transform, trainCrashForce, 3f);
        }
        
        // Clean up the exploded parts
        if (cleanUp)
        {
            if(cleanupDelay > 0)
                DOVirtual.DelayedCall(cleanupDelay, despawnParts);
            else
                despawnParts();
        }
    }
    
    private void ChangeObstacleLayer(int layer = -1)
    {
        if (partsInTheWay == null || partsInTheWay.Length == 0)
        {
            Debug.LogWarning("No bridge parts set in ClockTutManager.");
            return;
        }
        
        if(layer< 0)
        {
            layer = LayerMask.NameToLayer("CameraCollision");
        }
        
        foreach (GameObject part in partsInTheWay)
        {
            part.layer = layer;
        }
    }
    
    private void despawnParts()
    {
        if (explosionParts == null || explosionParts.Length == 0)
        {
            Debug.LogWarning("No wall parts set in ClockTutManager.");
            return;
        }
        
        foreach (GameObject part in explosionParts)
        {
            SpawnAnim.Despawn(part);
        }
    }
    
    // draw for the first wall part the force vector in simulatePhysics for reference in the editor
    // private void OnDrawGizmos()
    // {
    //     if (explosionParts != null && explosionParts.Length > 0)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawLine(explosionParts[0].transform.position, explosionParts[0].transform.position + trainCrashForce);
    //     }
    //     else
    //     {
    //         Debug.Log("No wall parts set in ClockTutManager.");
    //     }
    // }
}
