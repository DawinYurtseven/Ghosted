using UnityEngine;
using DG.Tweening;

public class Canon : MonoBehaviour
{
    [Header("Canon")] 
    [SerializeField] private float canonForce;
    [SerializeField] private GameObject openCanon;
    [SerializeField] private GameObject endPoint;

    public Vector3 shootingDir;
    public GameObject goalPos;
    
    [Header("Confetti")]
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private AudioSource confettiSound;

    //private bool canFire = false;
    private EmotionSingletonMock emoSing;

    private void Awake()
    {
        emoSing = FindObjectOfType<EmotionSingletonMock>();
        if (emoSing == null)
        {
            Debug.LogWarning("No EmotionSingletonMock found! Cannot check emotions for canon firing.");
        }
    }

    private void Start()
    {
        if(shootingDir == Vector3.zero)
        {
            // Set default shooting direction if not set
            shootingDir = transform.up;
            Debug.Log("Setting Canon dir to upwards");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // shoot player out of the canon
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Debug.Log("FIRE IN THE HOLE!");
                
                //other.transform.DOMove(endPoint.transform.position, 0.21f).SetEase(Ease.InOutSine)
                    //.OnComplete(() => FirePlayer(playerRb));
                
                // Check if current emotion is joy
                
                // check if openCanon is active and if the current emotion is Joy
                //if (emoSing.getCurrentEmotion() == Emotion.Joy && openCanon.activeSelf)
                    // Tween player to endPoint and to goal point as shooting
                    playerRb.transform.DOMove(endPoint.transform.position, 0.5f).SetEase(Ease.InOutSine)
                    .OnComplete(() => FirePlayer(playerRb));
            }
        }
    }
    
    private void FirePlayer(Rigidbody playerRb)
    {
        // playerRb.transform.TransformDirection(shootingDir);
        // playerRb.AddForce(shootingDir * canonForce, ForceMode.Impulse);
        //playerRb.velocity = transform.up * canonForce;
        
        TriggerConfetti();
        
        // use DoTween to smoothly move the player
        playerRb.transform.DOMove(goalPos.transform.position, 0.5f).SetEase(Ease.InOutSine);
    }
    
    private void TriggerConfetti()
    {
        if (confetti != null)
        {
            confetti.Play();
            if(confettiSound) confettiSound?.Play();
        }
    }
    
    // show fire direction in editor
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(transform.position, transform.position + shootingDir * 10);
    // }
}
