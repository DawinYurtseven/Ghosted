using System.Collections;
using Cinemachine;
using TMPro;
using UniRx;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControllerMockup : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private LayerMask ground;

    [SerializeField] private AnimationCurve animCurve;

    [SerializeField] private float timer;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        EmotionSingletonMock.Instance.CurrentTarget.Subscribe(talisman => { target = talisman; });
        talismanModetext.text = tMode.ToString();
        if (tMode == talismanMode.bind)
        {
            talismanEmotionText.enabled = false;
        }
        else
        {
            talismanEmotionText.enabled = true;
            talismanEmotionText.text = talismanEmotion.ToString();
        }
    }

    private void Update()
    {
        //CameraControl
        CameraUpdate();
        //interactable check
        CheckForInteractables();
    }

    public void FixedUpdate()
    {
        //Movement
        MovementUpdate();

        //jumpControl
        RegulateJump();
    }

    #region Speed

    [Header("Movement")] [SerializeField] private Vector2 moveVector;
    [SerializeField] public float maxSpeed, acceleration, currentSpeed, shotLockLimit, deaccelerationTime;
    [SerializeField] private Transform lookAtTarget;

    //TODO: affect Direction change in air
    public void Move_2D(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var lastMoveVector = moveVector;
            moveVector = context.ReadValue<Vector2>();
            currentSpeed -= ((lastMoveVector - moveVector).magnitude * currentSpeed / 2) * 0.5f;
            //todo: let player slow down before changing direction drastically
        }
        else if (context.canceled)
        {
            moveVector = new Vector2(0, 0);
            StartCoroutine(Deacceleration());
        }
    }

    private IEnumerator Deacceleration()
    {
        float timer = 0f;
        while (timer < deaccelerationTime)
        {
            if (moveVector != Vector2.zero) yield break;
            var velocity = rb.transform.InverseTransformDirection(rb.velocity);
            currentSpeed = Mathf.Lerp(currentSpeed, 0, timer / deaccelerationTime);
            rb.velocity = Vector3.Lerp(velocity, new Vector3(0, 0, 0) + lookAtTarget.up * velocity.y, timer / 0.5f);
            timer += Time.fixedDeltaTime;
            yield return null;
        }
    }

    private void MovementUpdate()
    {
        if (!dashedCooldown && moveVector != Vector2.zero)
        {
            var right = lookAtTarget.right;
            var forward = lookAtTarget.forward;
            lastInput = right * moveVector.x + forward * moveVector.y;
            var magnitude = lastInput.magnitude;
            lastInput *= 1 / magnitude;
            currentSpeed += Time.fixedDeltaTime * acceleration;

            //reduces movement limit during shot lock over time so player won't instantly stop at place
            shotLockLimit = lockOn ? Mathf.Lerp(shotLockLimit, 10f, Time.fixedDeltaTime) : maxSpeed;

            currentSpeed = Mathf.Clamp(currentSpeed, 0, lockOn ? shotLockLimit : maxSpeed);

            var localVelocity = rb.transform.InverseTransformDirection(rb.velocity);
            Vector3 relativeMove = right * (moveVector.x * currentSpeed) +
                                   forward * (currentSpeed * moveVector.y)
                                   + lookAtTarget.up * localVelocity.y;
            rb.velocity = relativeMove;
        }
    }

    #endregion

    #region Camera

    [Header("Camera")] [SerializeField] private GameObject cameraPivot;
    [SerializeField] private GameObject lookAtPivot;
    [SerializeField] private Vector2 cameraDirection;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private Vector3 CameraShoulderOffset = new Vector3(2,0,0), CameraShoulderLockOnOffset = new Vector3(2,2,0);

    [SerializeField] private bool strifing;

    private float xAxisAngle, yAxisAngle;

    [SerializeField] private float xAxisMin, xAxisMax, xAxisMinLock, xAxisMaxLock;

    public void Camera_Move(InputAction.CallbackContext context)
    {
        if (context.performed)
            cameraDirection = context.ReadValue<Vector2>();
        else if (context.canceled)
            cameraDirection = new Vector2(0, 0);
    }

    private GameObject mockTransform;

    private void CameraUpdate()
    {
        if (inCameraTransition) return;
        if (lockOn && target != null)
        {
            mockTransform.transform.position = targetPosition;
            cameraPivot.transform.LookAt(targetPosition);
            xAxisAngle = cameraPivot.transform.localRotation.eulerAngles.x - 360;
            yAxisAngle = cameraPivot.transform.localRotation.eulerAngles.y - 360;
            xAxisAngle = Mathf.Clamp(xAxisAngle, xAxisMinLock, xAxisMaxLock);
            cameraPivot.transform.eulerAngles = new Vector3(
                xAxisAngle,
                yAxisAngle,
                0
            );
            if (strifing) lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);
        }
        else
        {
            xAxisAngle += -cameraDirection.y * cameraSpeed * Time.fixedDeltaTime;
            yAxisAngle += cameraDirection.x * cameraSpeed * Time.fixedDeltaTime;
            xAxisAngle = Mathf.Clamp(xAxisAngle, xAxisMin, xAxisMax);

            cameraPivot.transform.localRotation = Quaternion.Euler(xAxisAngle, yAxisAngle, 0f);
            lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);
        }
    }

    #endregion

    #region Jump

    [Header("Jump")] [SerializeField] public float jumpStrength;
    [SerializeField] public float fallStrength;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask allowJumpOn;
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && Physics.Raycast(transform.position, -transform.up, 1.1f, allowJumpOn))
        {
            var up = transform.up;
            rb.velocity += up * jumpStrength;
            //rb.AddForce(up * jumpStrength, ForceMode.Force);
        }
    }

    private void RegulateJump()
    {
        if (!Physics.Raycast(transform.position, -transform.up, 1.1f))
            rb.AddForce(-transform.up * fallStrength, ForceMode.Acceleration);
    }

    #endregion

    #region Dash

    [Header("Dash")] [SerializeField] private float dashSpeed;
    [SerializeField] private Vector3 lastInput;
    [SerializeField] private bool dashed, dashedCooldown;


    //this function will use the moveVector from the Speed region 
    public void Dash(InputAction.CallbackContext context)
    {
        if (lockOn) return;
        if (context.started)
        {
            if (Physics.Raycast(transform.position, -transform.up, 1.1f))
                dashed = false;

            if (dashedCooldown || dashed) return;
            dashed = true;
            //uses same logic as in Speed region to move person
            Vector3 relativeDash = lastInput * dashSpeed;
            rb.velocity = relativeDash;

            //dash adds speed to current speed
            currentSpeed += maxSpeed * 0.8f;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCooldown()
    {
        dashedCooldown = true;
        yield return new WaitForSeconds(0.5f);
        dashedCooldown = false;
    }

    #endregion

    #region Target System

    [SerializeField] private int lockOnRange;
    [SerializeField] private new CinemachineVirtualCamera camera;
    [SerializeField] private float cameraZoomSpeed;

    private bool lockOn;
    private Vector3 targetPosition;

    //If merge conflict -> change to private, used for Mock for level design
    [SerializeField] public TalismanTargetMock target;

    public void ToggleLockOn(InputAction.CallbackContext context)
    {
        var fov = 60f;
        var newVal = 2.5f;
        if (context.performed && target != null && !lockOn)
        {
            lockOn = true;
            if (mockTransform == null) mockTransform = new GameObject();
            mockTransform.transform.position = lookAtTarget.transform.position;
            camera.m_LookAt = mockTransform.transform;
            fov = 30f;
            newVal = 0f;


            targetPosition = lookAtTarget.transform.position;


            StartCoroutine(LerpTargetPosition());
            StartCoroutine(LerpActionShotLockInput(fov, CameraShoulderLockOnOffset, newVal));
        }

        if (context.canceled && target != null && lockOn && mockTransform != null)
        {
            lockOn = false;
            StartCoroutine(LerpBackFocus());
            StartCoroutine(LerpActionShotLockInput(fov, CameraShoulderOffset, newVal));
        }
    }

    private IEnumerator LerpActionShotLockInput(float newFOV, Vector3 newOffset, float newVal)
    {
        bool firstState = lockOn;
        float lerpTimer = 0f;
        while (lerpTimer < cameraZoomSpeed)
        {
            if (!firstState.Equals(lockOn)) yield break;

            var fov = camera.m_Lens.FieldOfView;
            var lerpFloat = Mathf.Lerp(fov, newFOV, lerpTimer / cameraZoomSpeed);
            camera.m_Lens.FieldOfView = lerpFloat;

            var cine3RdPerson = camera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            var offset = cine3RdPerson.ShoulderOffset;
            var lerpVector = Vector3.Lerp(offset, newOffset, lerpTimer / cameraZoomSpeed);
            camera.GetComponentInChildren<Cinemachine3rdPersonFollow>().ShoulderOffset = lerpVector;

            var vertArmLength = cine3RdPerson.VerticalArmLength;
            var lerpLength = Mathf.Lerp(vertArmLength, newVal, lerpTimer / cameraZoomSpeed);
            camera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().VerticalArmLength = lerpLength;

            lerpTimer += Time.deltaTime;
            yield return null;
        }

        camera.m_Lens.FieldOfView = newFOV;
        camera.GetComponentInChildren<Cinemachine3rdPersonFollow>().ShoulderOffset = newOffset;
    }

    private bool inCameraTransition;

    private IEnumerator LerpTargetPosition()
    {
        inCameraTransition = true;
        var targetDirection = target.transform.transform.position - cameraPivot.transform.position;
        var _lookRot = Quaternion.LookRotation(targetDirection);
        var lerpTimer = 0f;
        while (lerpTimer < cameraZoomSpeed)
        {
            if (!lockOn)
            {
                yield break;
            }

            mockTransform.transform.position = targetPosition;

            cameraPivot.transform.rotation =
                Quaternion.Slerp(cameraPivot.transform.rotation, _lookRot, lerpTimer / cameraZoomSpeed);

            targetPosition = Vector3.Lerp(targetPosition, target.transform.position, lerpTimer / cameraZoomSpeed);
            lerpTimer += Time.deltaTime;
            yield return null;
        }

        inCameraTransition = false;
        targetPosition = target.transform.position;
    }

    private IEnumerator LerpBackFocus()
    {
        inCameraTransition = true;
        var lerpTimer = 0f;
        while (lerpTimer < cameraZoomSpeed)
        {
            if (lockOn)
            {
                yield break;
            }

            mockTransform.transform.position = targetPosition;

            xAxisAngle += -cameraDirection.y * cameraSpeed * Time.fixedDeltaTime;
            yAxisAngle += cameraDirection.x * cameraSpeed * Time.fixedDeltaTime;
            xAxisAngle = Mathf.Clamp(xAxisAngle, -15f, 65f);


            cameraPivot.transform.localRotation = Quaternion.Euler(xAxisAngle, yAxisAngle, 0f);
            lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);

            targetPosition = Vector3.Lerp(targetPosition, lookAtTarget.transform.position, lerpTimer / cameraZoomSpeed);
            lerpTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        targetPosition = lookAtTarget.transform.position;
        camera.m_LookAt = lookAtTarget.transform;
        inCameraTransition = false;
        Destroy(mockTransform);
    }

    #endregion

    #region Talismans

    [SerializeField] private TalismanTargetMock currentTalismanBind, previousTargetTalismanObject;
    [SerializeField] private talismanMode tMode;
    [SerializeField] private Emotion talismanEmotion;

    [SerializeField] private TextMeshProUGUI talismanModetext, talismanEmotionText;

    public void ChangeTalismanMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tMode = tMode == talismanMode.bind ? talismanMode.emotions : talismanMode.bind;
            if (tMode == talismanMode.bind)
            {
                talismanModetext.text = "Bind";
                talismanEmotionText.enabled = false;
            }
            else
            {
                talismanEmotionText.enabled = true;
                talismanModetext.text = "Emotions";
                talismanEmotionText.text = talismanEmotion.ToString();
            }
        }
    }

    public void ChangeEmotionTalisman(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            talismanEmotion = talismanEmotion == Emotion.Joy ? Emotion.Lonely : Emotion.Joy;
            talismanEmotionText.text = talismanEmotion.ToString();
        }
    }

    [SerializeField] private GameObject TalismanPrefab;

    private GameObject thrownTalisman;

    public void ThrowTalisman(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (target == null || thrownTalisman != null) return;
            if (previousTargetTalismanObject != null) previousTargetTalismanObject.ResetObject();

            thrownTalisman = Instantiate(TalismanPrefab, gameObject.transform.position,
                Quaternion.LookRotation((target.transform.position - transform.position).normalized));
            thrownTalisman.GetComponent<Talisman>().Initialize(tMode, talismanEmotion);
            StartCoroutine(thrownTalisman.GetComponent<Talisman>().MoveTowards(target));
            previousTargetTalismanObject = target;
        }
    }

    private TalismanTargetMock tempTar;
    private AltarMock tempAltar;

    private void CheckForInteractables()
    {
        if (Physics.SphereCast(transform.position, 1f, lookAtPivot.transform.forward, out var hit, 10))
        {
            if (hit.collider.gameObject.TryGetComponent(typeof(TalismanTargetMock), out var tar))
            {
                tempTar = (TalismanTargetMock)tar;
                tempTar.HighlightInteract();
            }
            else if (hit.collider.gameObject.TryGetComponent(typeof(AltarMock), out var altar))
            {
                print("altar");
                tempAltar = (AltarMock)altar;
            }
            else
            {
                tempTar = null;
                tempAltar = null;
            }
        }
        else
        {
            if (tempTar != null)
            {
                tempTar.UnHighlight();
                tempTar = null;
            }
        }
    }

    public void PlaceTalisman(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (tempTar == null && tempAltar == null) return;
            if (tempAltar != null)
            {
                print("sup");
                tempAltar.ChangeEmotion(talismanEmotion);
                return;
            }

            if (previousTargetTalismanObject != null) previousTargetTalismanObject.ResetObject();
            switch (tMode)
            {
                case talismanMode.bind:
                    tempTar.Bind();
                    break;
                case talismanMode.emotions:
                    tempTar.EvokeEmotion(talismanEmotion);
                    break;
            }

            previousTargetTalismanObject = tempTar;
            print(previousTargetTalismanObject);
        }
    }

    #endregion

    #region Position and Rotation

    /*
     * TODO: Overwrite this to make it slope calculation and allow the player to have friction at slopes.
     * This should work in tandem with the custom gravity and thus needs probably a raycast to check the slope normal
     * with the characters up direction and thus calculate custom friction.
     */

    private bool _isOnCurvedGround;
    [SerializeField] private float maxSlopeAngle = 45f;

    public void OnCollisionStay(Collision collisionInfo)
    {
        //set the ground which the player should be able to move on to Ground if possible. 
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            //create friction only when not intending to move
            if (moveVector == Vector2.zero)
            {
                RaycastHit hit; //check for ground below the player and get the angle
                if (Physics.Raycast(transform.position, -transform.up, out hit, ground))
                {
                    // Calculate the slope angle
                    float slopeAngle = Vector3.Angle(hit.normal, transform.up);

                    // Apply friction on a specific angle 
                    if (slopeAngle >= 0 && slopeAngle <= maxSlopeAngle)
                    {
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
                }
            }
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("CurvedGround"))
        {
            _isOnCurvedGround = false;
            StartCoroutine(ReturnRotation());
        }
    }

    private IEnumerator ReturnRotation()
    {
        yield return new WaitForSeconds(2f);

        while (!transform.rotation.Equals(Quaternion.Euler(0, 0, 0)))
        {
            if (_isOnCurvedGround) yield break;
            var transform1 = transform;
            var reference = Quaternion.Lerp(transform1.rotation, Quaternion.Euler(0, 0, 0),
                animCurve.Evaluate(timer));
            transform.localRotation = Quaternion.Euler(reference.eulerAngles.x, reference.eulerAngles.y,
                reference.eulerAngles.z);
            yield return null;
        }
    }

    #endregion
}