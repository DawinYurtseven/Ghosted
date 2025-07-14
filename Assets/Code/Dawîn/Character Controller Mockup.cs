using System;
using System.Collections;
using System.Collections.Generic;
using Ghosted.Dialogue;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControllerMockup : MonoBehaviour
{
    //TODO: ADD fmod and animations 
    
    
    private static readonly int Grounded = Animator.StringToHash("grounded");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jumping = Animator.StringToHash("jump");
    private static readonly int Throw = Animator.StringToHash("Throw Talisman");
    private static readonly int Call = Animator.StringToHash("call");

    [Header("base values")]
    private Rigidbody _rb;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject characterObject;

    [SerializeField] private PlayerConversant conversant;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rb = GetComponent<Rigidbody>();
        conversant = GetComponent<PlayerConversant>();
    }

    private void Start()
    {
        animator.SetBool(Grounded, true);
        EmotionSingletonMock.Instance.CurrentTarget.Subscribe(talisman =>
        {
            /*if (talisman == null && target != null && lockOn && mockTransform != null)
            {
                lockOn = false;
                StartCoroutine(LerpBackFocus());
                StartCoroutine(LerpActionShotLockInput(60f, CameraShoulderOffset, 2.5f));
            }*/

            target = talisman;
        });
    }

    private void Update()
    {
        //CameraControl
        CameraUpdate();
        //interactable check
        //CheckForInteractables();
    }

    public void FixedUpdate()
    {
        //Movement
        MovementUpdate();

        //jumpControl
        RegulateJump();

        //check for slope
        Slope();
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
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Move");
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
            var velocity = _rb.transform.InverseTransformDirection(_rb.velocity);
            currentSpeed = Mathf.Lerp(currentSpeed, 0, timer / deaccelerationTime);
            _rb.velocity = Vector3.Lerp(velocity, new Vector3(0, 0, 0) + lookAtTarget.up * velocity.y, timer / 0.5f);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void MovementUpdate()
    {
        if (moveVector != Vector2.zero)
        {
            characterObject.transform.localRotation = lookAtPivot.transform.localRotation;
            //rotate with direction
            characterObject.transform.localRotation *=
                Quaternion.LookRotation(new Vector3(moveVector.x, 0, moveVector.y));

            var right = lookAtTarget.right;
            var forward = lookAtTarget.forward;
            currentSpeed += Time.fixedDeltaTime * acceleration;

            //reduces movement limit during shot lock over time so player won't instantly stop at place


            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            var localVelocity = _rb.transform.InverseTransformDirection(_rb.velocity);
            Vector3 relativeMove = right * (moveVector.x * currentSpeed) +
                                   forward * (currentSpeed * moveVector.y)
                                   + lookAtTarget.up * localVelocity.y;
            _rb.velocity = relativeMove;
        }

        animator.SetFloat(Speed, currentSpeed / maxSpeed);
    }

    #endregion

    #region Camera

    [Header("Camera")] 
    [SerializeField] private GameObject cameraPivot;
    [SerializeField] private GameObject lookAtPivot;
    [SerializeField] private Vector2 cameraDirection;
    [SerializeField] private float cameraSpeed;

    private float _xAxisAngle, _yAxisAngle;

    [SerializeField] private float xAxisMin, xAxisMax;

    public void Camera_Move(InputAction.CallbackContext context)
    {
        if (context.performed)
            cameraDirection = context.ReadValue<Vector2>();
        else if (context.canceled)
            cameraDirection = new Vector2(0, 0);
    }

    private void CameraUpdate()
    {
        _xAxisAngle += -cameraDirection.y * cameraSpeed * Time.fixedDeltaTime;
        _yAxisAngle += cameraDirection.x * cameraSpeed * Time.fixedDeltaTime;
        _xAxisAngle = Mathf.Clamp(_xAxisAngle, xAxisMin, xAxisMax);

        cameraPivot.transform.localRotation = Quaternion.Euler(_xAxisAngle, _yAxisAngle, 0f);
        lookAtPivot.transform.localRotation = Quaternion.Euler(0f, _yAxisAngle, 0f);
    }

    #endregion

    #region Jump

    /*
     * Now each variable explained in order:
     * jumpStrength: How strong the jump is, how high the player can jump.
     * FallStrength: How strong the player falls, how fast the player falls. this also dictates how quick he starts to fall from a jump
     * groundCheckDistance: How far the player checks for ground below him, this is used to check if the player is grounded or not.
     * coyoteTime: How long the player can jump after leaving the ground, this is used to allow the player to jump after leaving the ground.
     * the two booleans coyoteJumped and isGrounded are used to check if the player is grounded or not and dictate coyote time.
     */
    [Header("Jump")] 
    [SerializeField] public float jumpStrength;
    [SerializeField] public float fallStrength;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private bool coyoteJumped, isGrounded = true, jumpPressed;

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && (Physics.SphereCast(transform.position, 0.3f, -transform.up, out var hit,
                                    groundCheckDistance, ground) ||
                                !coyoteJumped))
        {
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Jump");
            if (Vector3.Angle(hit.normal, transform.up) > 45f)
                return;
            animator.SetTrigger(Jumping);
            coyoteJumped = true;
            animator.SetBool(Grounded, false);
            var up = transform.up;
            _rb.velocity += up * jumpStrength;
            var right = lookAtTarget.right;
            var forward = lookAtTarget.forward;
            _rb.AddForce(right * moveVector.x * 200f +
                        forward * moveVector.y * 200f);
            print("jumped");
        }
    }

    public void Jump()
    {
        animator.SetBool(Grounded, false);
        var up = transform.up;
        _rb.velocity += up * jumpStrength;
        //rb.AddForce(up * jumpStrength, ForceMode.Force);
    }

    IEnumerator CoyoteJump()
    {
        float timer = 0f;
        while (timer < coyoteTime)
        {
            if (Physics.SphereCast(transform.position, 0.3f, -transform.up, out _, groundCheckDistance, ground))
            {
                isGrounded = true;
                animator.SetBool(Grounded, true);
                yield break;
            }

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        coyoteJumped = true;
    }

    private void RegulateJump()
    {
        Debug.DrawLine(transform.position, transform.position - transform.up * groundCheckDistance, Color.red, 0.5f);
        if (!Physics.SphereCast(transform.position, 0.3f, -transform.up, out _, groundCheckDistance, ground))
        {
            animator.SetBool(Grounded, false);
            if (isGrounded && !coyoteJumped)
            {
                isGrounded = false;
                coyoteJumped = false;
                StartCoroutine(CoyoteJump());
            }
        }

        _rb.AddForce(-transform.up * fallStrength, ForceMode.Acceleration);
    }

    #endregion


    #region Target System

    //If merge conflict -> change to private, used for Mock for level design
    [Header("Targeting")]
    [SerializeField] public TalismanTargetMock target;

    /*
    [SerializeField] private int lockOnRange;
    [SerializeField] private new CinemachineVirtualCamera camera;
    [SerializeField] private float cameraZoomSpeed;


    //TODO: Lock on raus und mehr am ui arbeiten
    private bool lockOn;
    private Vector3 targetPosition;



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
    */

    #endregion

    #region Talismans
    
    [Header("Talismans")]
    [SerializeField] private TalismanTargetMock currentTalismanBind, previousTargetTalismanObject;
    [SerializeField] private talismanMode tMode;
    [SerializeField] private Emotion talismanEmotion;

    [SerializeField] private TextMeshProUGUI talismanModetext, talismanEmotionText;


    [SerializeField] private GameObject talismanPrefab;

    private GameObject _thrownTalisman;


    //Check this 
    public int maxTalismans = 3;
    private int _curTalismans;
    [SerializeField] TextMeshProUGUI talismansUsed;
    [SerializeField] private List<TalismanTargetMock> lockedObjects = new List<TalismanTargetMock>();

    public void ThrowTalisman(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (EmotionSingletonMock.Instance.disableAll) return;
            if (!target || _thrownTalisman) return;
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Shoot");
            //If the object is already bounded, recall talisman
            if (lockedObjects.Contains(target))
            {
                lockedObjects.Remove(target);
                _curTalismans--;
                target.Bind();
                _thrownTalisman = Instantiate(talismanPrefab, target.gameObject.transform.position,
                    Quaternion.LookRotation((transform.position - gameObject.transform.position).normalized));
                StartCoroutine(_thrownTalisman.GetComponent<Talisman>().MoveTowardsPlayer(this));
                animator.SetTrigger(Call);
                talismansUsed.text = "Talismans used: " + _curTalismans + " / " + maxTalismans;
                animator.SetTrigger(Call);
                //thrownTalisman.GetComponent<Talisman>().Initialize(tMode, talismanEmotion);
                StartCoroutine(_thrownTalisman.GetComponent<Talisman>().MoveTowardsPlayer(this));
                talismansUsed.text = maxTalismans- _curTalismans + " / " + maxTalismans;
            }

            //Throw talisman
            else
            {
                if (_curTalismans == maxTalismans) return;
                _curTalismans++;
                talismansUsed.text = maxTalismans- _curTalismans + " / " + maxTalismans;
                animator.SetTrigger(Throw);
                Vector3 lookPosition = new Vector3 (target.transform.position.x, characterObject.transform.position.y, target.transform.position.z);
                characterObject.transform.LookAt(lookPosition);
            }
            
        }
    }

    public void ThrowTalismanAnim()
    {
        //curTalismans++;
        lockedObjects.Add(target);
        _thrownTalisman = Instantiate(talismanPrefab, gameObject.transform.position,
            Quaternion.LookRotation((target.transform.position - transform.position).normalized));
        _thrownTalisman.GetComponent<Talisman>().Initialize(tMode, talismanEmotion);
        StartCoroutine(_thrownTalisman.GetComponent<Talisman>().MoveTowards(target));
        //talismansUsed.text = "Talismans used: " + curTalismans + " / " + maxTalismans;
    }

    #endregion

    #region Interactions

    [Header("Interactions")]
    private TalismanTargetMock _tempTar;
    public AltarMock tempAltar;

    // For Cutscene
    public static event Action FirstUsageAltar;
    public bool usedAltar;

    [SerializeField] private int interactionRange = 10;

    [SerializeField] private float interactDistanceAltar = 3f;
    [SerializeField] private Transform checkFrom;

    public void SetAltar(AltarMock altar)
    {
        if (tempAltar != null)
        {
            tempAltar.turnOffHintAltar();
        }
        tempAltar = altar;
        altar.turnOnHintAltar();
    }

    public void LeaveAltar()
    {
        if (tempAltar != null)
        {
            tempAltar.turnOffHintAltar();
            tempAltar = null;
        }
    }
    

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("E performed");
            // if (tempTar == null && tempAltar == null) return;
            if (tempAltar)
            {
                Debug.Log("Altar found");
                if (!usedAltar)
                {
                    usedAltar = true;
                    FirstUsageAltar?.Invoke();
                }

                tempAltar.ChangeEmotion(talismanEmotion);
            }
            else
            {
                conversant.InteractDialogue();
            }
        }
    }

    public void RecallTalismans(InputAction.CallbackContext context)
    {
        if (tempAltar && context.performed)
        {
            Debug.Log("Recalling talismans");
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Recall");
            
            foreach (TalismanTargetMock lockedObject in lockedObjects)
            {
                lockedObject.Bind();
            }

            lockedObjects.Clear();
            _curTalismans = 0;
            talismansUsed.text = maxTalismans- _curTalismans + " / " + maxTalismans;
        }
    }

    #endregion

    #region Position and Rotation
    
    [Header("Position and Rotation")]

    private bool _isOnCurvedGround;
    [SerializeField] private float maxSlopeAngle = 45f, slopeAdjustment = 1.2f;

    private void Slope()
    {
        if (Physics.SphereCast(transform.position, 0.3f, -transform.up, out var hit, groundCheckDistance, ground))
        {
            float slopeAngle = Vector3.Angle(hit.normal, transform.up);
            if (moveVector == Vector2.zero && slopeAngle >= 0 && slopeAngle <= maxSlopeAngle && !coyoteJumped)
            {
                _rb.velocity = new Vector3(0, 0, 0);
                _rb.AddForce(transform.up * fallStrength, ForceMode.Acceleration);
            }
            else if (moveVector != Vector2.zero && slopeAngle > maxSlopeAngle)
            {
                // If the slope is too steep, apply a force to keep falling, no matter how hard the player tries to move
                _rb.velocity = new Vector3(_rb.velocity.x, -10, _rb.velocity.z);
            }
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & ground) != 0)
        {
            float slopeAngle = Vector3.Angle(other.contacts[0].normal, transform.up);
            // Apply friction on a specific angle 
            if (slopeAngle >= 0 && slopeAngle <= maxSlopeAngle)
            {
                isGrounded = true;
                coyoteJumped = false;
                animator.SetBool(Grounded, true);
                animator.ResetTrigger(Jumping);
            }
        }
    }

    #endregion
}