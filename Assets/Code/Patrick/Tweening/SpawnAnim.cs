using UnityEngine;
using DG.Tweening;

public class SpawnAnim : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float animDuration = 0.5f;
    private CanvasGroup cg;
    
    // Set all required Attributes
    private void Awake()
    {
        if (!obj)
            obj = this.gameObject;
        if (!spawnPoint)
            spawnPoint = this.gameObject.transform;
        
        if (obj)
        {
            // Add CanvasGroup for fade if not already present
            cg = obj.GetComponent<CanvasGroup>();
            if (!cg) cg = obj.AddComponent<CanvasGroup>();
        }
    }
    
    // handles the animation
    public void Spawn(bool state)
    {
        if (!obj)
        {
            Debug.LogWarning("No gameobject to spawn!");
            return;
        }
        
        Transform t = obj.transform;
        obj.SetActive(true);
        Debug.Log("Got: " + state);
        if (state)
        {
            /// Show + animate in
            cg.alpha = 0;

            t.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack);
            cg.DOFade(1, animDuration);
            
            Debug.Log("Showing obj");
        }
        else
        {
            // Animate out
            t.DOScale(Vector3.zero, animDuration).SetEase(Ease.InBack);
            cg.DOFade(0, animDuration).OnComplete(() =>
            {
                obj.SetActive(false);
            });

            Debug.Log("Deactivating obj");
        }
    }
}

