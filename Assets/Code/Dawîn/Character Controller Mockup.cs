using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Ghosted.Dialogue;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControllerMockup : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject characterObject;

    [SerializeField] private PlayerConversant conversant;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        conversant = GetComponent<PlayerConversant>();
    }

    private void Start()
    {
        animator.SetBool("grounded", true);
        EmotionSingletonMock.Instance.CurrentTarget.Subscribe(talisman =>
        {
            if (talisman == null && target != null && lockOn && mockTransform != null)
            {
                lockOn = false;
                StartCoroutine(LerpBackFocus());
                StartCoroutine(LerpActionShotLockInput(60f, CameraShoulderOffset, 2.5f));
            }

            target = talisman;
        });
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
            var velocity = rb.transform.InverseTransformDirection(rb.velocity);
            currentSpeed = Mathf.Lerp(currentSpeed, 0, timer / deaccelerationTime);
            rb.velocity = Vector3.Lerp(velocity, new Vector3(0, 0, 0) + lookAtTarget.up * velocity.y, timer / 0.5f);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void MovementUpdate()
    {
        if (!dashedCooldown && moveVector != Vector2.zero)
        {
            characterObject.transform.localRotation = lookAtPivot.transform.localRotation;
            //rotate with direction
            characterObject.transform.localRotation *=
                Quaternion.LookRotation(new Vector3(moveVector.x, 0, moveVector.y));

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

        animator.SetFloat("Speed", currentSpeed / maxSpeed);
    }

    #endregion

    #region Camera

    [Header("Camera")] [SerializeField] private GameObject cameraPivot;
    [SerializeField] private GameObject lookAtPivot;
    [SerializeField] private Vector2 cameraDirection;
    [SerializeField] private float cameraSpeed;

    [SerializeField] private Vector3 CameraShoulderOffset = new Vector3(2, 0, 0),
        CameraShoulderLockOnOffset = new Vector3(2, 2, 0);

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
            xAxisAngle += -cameraDirection.y * cameraSpeed * Time.fixedDeltaTime * PlayerPrefs.GetInt("invert", 1) * PlayerPrefs.GetFloat("sensitivity", 1);
            yAxisAngle += cameraDirection.x * cameraSpeed * Time.fixedDeltaTime * PlayerPrefs.GetFloat("sensitivity", 1);
            xAxisAngle = Mathf.Clamp(xAxisAngle, xAxisMin, xAxisMax);

            cameraPivot.transform.localRotation = Quaternion.Euler(xAxisAngle, yAxisAngle, 0f);
            lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);
        }
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
    [Header("Jump")] [SerializeField] public float jumpStrength;
    [SerializeField] public float fallStrength;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private bool coyoteJumped, isGrounded = true, jumpPressed = false;

    public void Jump(InputAction.CallbackContext context)
    {
        //print(coyoteJumped);
        if (context.started &&
            (Physics.SphereCast(transform.position, 0.3f, -transform.up, out var hit, groundCheckDistance, ground) ||
             !coyoteJumped))
        {
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Jump");
            float angle = Vector3.Angle(hit.normal, transform.up);
            if (Vector3.Angle(hit.normal, transform.up) > 45f)
                return;
            /*var up = transform.up;
            rb.velocity += up * jumpStrength;
            //rb.AddForce(up * jumpStrength, ForceMode.Force);
            coyoteJumped = true;*/
            animator.SetTrigger("jump");
            coyoteJumped = true;
            animator.SetBool("grounded", false);
            var up = transform.up; 
            rb.velocity += up * jumpStrength;
        }
    }

    public void Jump()
    {
        animator.SetBool("grounded", false);
        var up = transform.up;
        rb.velocity += up * jumpStrength;
        //rb.AddForce(up * jumpStrength, ForceMode.Force);
    }

    IEnumerator CoyoteJump()
    {
        float timer = 0f;
        while (timer < coyoteTime)
        {
            if (Physics.SphereCast(transform.position, 0.3f, -transform.up, out var hit, groundCheckDistance, ground))
            {
                isGrounded = true;
                coyoteJumped = false;
                animator.SetBool("grounded", true);
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
        if (!Physics.SphereCast(transform.position, 0.3f, -transform.up, out var hit, groundCheckDistance, ground))
        {
            animator.SetBool("grounded", false);
            if (isGrounded && !coyoteJumped)
            {
                isGrounded = false;
                coyoteJumped = false;
                StartCoroutine(CoyoteJump());
            }
        }

        rb.AddForce(-transform.up * fallStrength, ForceMode.Acceleration);
    }

    //TODO: sliding problem 
    //TODO: 

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


    //TODO: Lock on raus und mehr am ui arbeiten
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


    [SerializeField] private GameObject TalismanPrefab;

    private GameObject thrownTalisman;


    //Check this 
    public int maxTalismans = 3;
    private int curTalismans = 0;
    [SerializeField] TextMeshProUGUI talismansUsed;
    [SerializeField] private List<TalismanTargetMock> lockedObjects = new List<TalismanTargetMock>();

    public void ThrowTalisman(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (EmotionSingletonMock.Instance.disableAll) return;
            if (target == null || thrownTalisman != null) return;
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Shoot");
            //If the object is already bounded, recall talisman
            if (lockedObjects.Contains(target))
            {
                lockedObjects.Remove(target);
                curTalismans--;
                target.Bind();
                thrownTalisman = Instantiate(TalismanPrefab, target.gameObject.transform.position,
                    Quaternion.LookRotation((transform.position - gameObject.transform.position).normalized));
                //thrownTalisman.GetComponent<Talisman>().Initialize(tMode, talismanEmotion);
                StartCoroutine(thrownTalisman.GetComponent<Talisman>().MoveTowardsPlayer(this));
                talismansUsed.text = maxTalismans- curTalismans + " / " + maxTalismans;
            }

            //Throw talisman
            else
            {
                if (curTalismans == maxTalismans) return;
                curTalismans++;
                talismansUsed.text = maxTalismans- curTalismans + " / " + maxTalismans;
                animator.SetTrigger("Throw Talisman");
                Vector3 lookPosition = new Vector3 (target.transform.position.x, characterObject.transform.position.y, target.transform.position.z);
                characterObject.transform.LookAt(lookPosition);
                /*lockedObjects.Add(target);
                thrownTalisman = Instantiate(TalismanPrefab, gameObject.transform.position,
                    Quaternion.LookRotation((target.transform.position - transform.position).normalized));
                thrownTalisman.GetComponent<Talisman>().Initialize(tMode, talismanEmotion);
                StartCoroutine(thrownTalisman.GetComponent<Talisman>().MoveTowards(target));*/
            }


            //previousTargetTalismanObject = target;
        }
    }

    public void ThrowTalismanAnim()
    {
        //curTalismans++;
        lockedObjects.Add(target);
        thrownTalisman = Instantiate(TalismanPrefab, gameObject.transform.position,
            Quaternion.LookRotation((target.transform.position - transform.position).normalized));
        thrownTalisman.GetComponent<Talisman>().Initialize(tMode, talismanEmotion);
        StartCoroutine(thrownTalisman.GetComponent<Talisman>().MoveTowards(target));
        //talismansUsed.text = "Talismans used: " + curTalismans + " / " + maxTalismans;
    }
    
    
    #endregion

    #region Interactions
    
    private TalismanTargetMock tempTar;
    public AltarMock tempAltar;
    
    // For Cutscene
    public static event Action firstUsageAltar;
    public bool usedAltar = false;

    [SerializeField] private int interactionRange = 10;

    public float interactDistanceAltar = 3f;
    public Transform checkFrom;

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
    
    
    private void CheckForInteractables()
    {
        
        if (tempAltar != null)
        {
            tempAltar.turnOffHintAltar();
            tempAltar = null;
        }
        
        //Same as for dialogues 
        Ray ray = new Ray(checkFrom.position, checkFrom.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistanceAltar))
        {
            //TalismanTargetMock tar = hit.collider.GetComponent<TalismanTargetMock>();
            AltarMock altar = hit.collider.GetComponentInParent<AltarMock>();
            if (altar && hit.collider.CompareTag("Altar") && altar != tempAltar)
            {
                if (tempAltar != null)
                {
                    tempAltar.turnOffHintAltar();
                }
                //print("altar");
                tempAltar = (AltarMock)altar;
                tempAltar.turnOnHintAltar();
            }
            else if (tempAltar != null)
            {
                tempAltar.turnOffHintAltar();
                tempAltar = null;
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("E performed");
            // if (tempTar == null && tempAltar == null) return;
            if (tempAltar != null)
            {
                Debug.Log("Altar found");
                if (!usedAltar)
                {
                    usedAltar = true;
                    firstUsageAltar?.Invoke();
                }
                
                tempAltar.InteractAltar();
            }
            else
            {
                conversant.InteractDialogue();
            }

            // if (previousTargetTalismanObject != null) previousTargetTalismanObject.ResetObject();
            // tempTar.Bind();
            //
            // previousTargetTalismanObject = tempTar;
            // print(previousTargetTalismanObject);
        }
    }

    public void RecallTalismans(InputAction.CallbackContext context)
    {
        if (tempAltar != null && context.performed)
        {
            Debug.Log("Recalling talismans");
            if (UIHintShow.Instance) UIHintShow.Instance.NotifyActionPerformed("Recall");
            
            foreach (TalismanTargetMock lockedObject in lockedObjects)
            {
                lockedObject.Bind();
            }

            lockedObjects.Clear();
            curTalismans = 0;
            talismansUsed.text = maxTalismans- curTalismans + " / " + maxTalismans;
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
    [SerializeField] private float maxSlopeAngle = 45f, slopeAdjustment = 1.2f;

    private void Slope()
    {
        if (Physics.SphereCast(transform.position, 0.3f, -transform.up, out var hit, groundCheckDistance, ground))
        {
            float slopeAngle = Vector3.Angle(hit.normal, transform.up);
            if (moveVector == Vector2.zero && slopeAngle >= 0 && slopeAngle <= maxSlopeAngle && !coyoteJumped)
            {
                rb.velocity = new Vector3(0, 0, 0);
                rb.AddForce(transform.up * fallStrength, ForceMode.Acceleration);
            }
            else if(moveVector != Vector2.zero && slopeAngle > maxSlopeAngle)
            {
                // If the slope is too steep, apply a force to keep falling, no matter how hard the player tries to move
                rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
            }
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("CurvedGround"))
        {
            _isOnCurvedGround = false;
            //StartCoroutine(ReturnRotation());
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
                animator.SetBool("grounded", true);
                animator.ResetTrigger("jump");
            }
        }
    }

    /*private IEnumerator ReturnRotation()
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
    }*/

    #endregion
}