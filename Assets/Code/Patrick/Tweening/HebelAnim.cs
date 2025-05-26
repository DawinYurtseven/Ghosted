using UnityEngine;
using DG.Tweening;

public class HebelAnim : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationAngle = -45f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease easing = Ease.OutBack;

    [Header("Optional Position Change")]
    [SerializeField] private bool moveOnToggle = false;
    [SerializeField] private Vector3 onPositionOffset = Vector3.zero;

    private bool isOn = false;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private void Awake()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    public void Toggle(bool state)
    {
        isOn = state;

        // Animate rotation
        float targetAngle = isOn ? rotationAngle : initialRotation.eulerAngles.z;
        transform.DOLocalRotate(new Vector3(0f, 0f, targetAngle), duration).SetEase(easing);

        // Animate position if enabled
        if (moveOnToggle)
        {
            Vector3 targetPos = isOn ? initialPosition + onPositionOffset : initialPosition;
            transform.DOLocalMove(targetPos, duration).SetEase(Ease.OutSine);
        }
    }
}


