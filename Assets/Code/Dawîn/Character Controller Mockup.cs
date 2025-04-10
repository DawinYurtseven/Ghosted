using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControllerMockup : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private LayerMask ground;

    [SerializeField] private AnimationCurve animCurve;

    [SerializeField] private float timer;

    [SerializeField] private Camera mainCamera;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        EmotionSingletonMock.Instance.CurrentTarget.Subscribe(talisman =>
        {
            target = talisman;
        });
    }

    private void Update()
    {
        //CameraControl
        CameraUpdate();
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
    [SerializeField] private float maxSpeed, acceleration, currentSpeed, shotLockLimit, deaccelerationTime;
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
        while (timer <deaccelerationTime)
        {
            if(moveVector != Vector2.zero) yield break;
            var velocity = rb.velocity;
            currentSpeed = velocity.magnitude;
            rb.velocity = Vector3.Lerp(velocity, new Vector3(0, 0, 0), timer /0.5f);
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

    private float xAxisAngle, yAxisAngle;

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
        if (inCameraTransition)return;
        if (lockOn && target != null)
        {
            mockTransform.transform.position = targetPosition;
            cameraPivot.transform.LookAt(targetPosition);
            xAxisAngle = cameraPivot.transform.localRotation.eulerAngles.x - 360;
            yAxisAngle = cameraPivot.transform.localRotation.eulerAngles.y - 360;
            xAxisAngle = Mathf.Clamp(xAxisAngle, -15f, 65f);
            cameraPivot.transform.eulerAngles = new Vector3(
                xAxisAngle,
                yAxisAngle,
                0
            );
            lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);
        }
        else
        {
            xAxisAngle += -cameraDirection.y * cameraSpeed * Time.fixedDeltaTime;
            yAxisAngle += cameraDirection.x * cameraSpeed * Time.fixedDeltaTime;
            xAxisAngle = Mathf.Clamp(xAxisAngle, -15f, 65f);


            cameraPivot.transform.localRotation = Quaternion.Euler(xAxisAngle, yAxisAngle, 0f);
            lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);
        }
        
        
        
    }

    #endregion

    #region Jump

    [Header("Jump")] [SerializeField] private float jumpStrength;
    [SerializeField] private float fallStrength;
    [SerializeField] private float gravity;
    [SerializeField] private float groundCheckDistance;


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && Physics.Raycast(transform.position, -transform.up, 1.1f))
        {
            var up = transform.up;
            rb.velocity += up * jumpStrength;
        }
    }

    private void RegulateJump()
    {
        rb.AddForce(-transform.up * gravity, ForceMode.Acceleration);
        if (rb.velocity.y != 0)
        {
            rb.AddForce(-transform.up * fallStrength, ForceMode.Acceleration);
        }
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
    [SerializeField] private TalismanTargetMock target;

    public void ToggleLockOn(InputAction.CallbackContext context)
    {
        var fov = 60f;
        var offset = new Vector3(2, 0, 0);
        var newVal = 2.5f;
        if (context.performed && target != null && !lockOn)
        {
            lockOn = true;
            if(mockTransform == null) mockTransform = new GameObject();
            mockTransform.transform.position = lookAtTarget.transform.position;
            camera.m_LookAt = mockTransform.transform;
            fov = 30f;
            offset = new Vector3(2, 2, 0);
            newVal = 0f;
            

            targetPosition = lookAtTarget.transform.position;
            
            
            StartCoroutine(LerpTargetPosition());
            StartCoroutine(LerpActionShotLockInput(fov, offset, newVal));
        }

        if (context.canceled && target != null && lockOn && mockTransform != null)
        {
            lockOn = false;
            StartCoroutine(LerpBackFocus());
            StartCoroutine(LerpActionShotLockInput(fov, offset, newVal));
            
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
            var lerpFloat = Mathf.Lerp(fov, newFOV, lerpTimer/cameraZoomSpeed);
            camera.m_Lens.FieldOfView = lerpFloat;

            var cine3RdPerson = camera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            var offset = cine3RdPerson.ShoulderOffset;
            var lerpVector = Vector3.Lerp(offset, newOffset, lerpTimer/cameraZoomSpeed);
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
        print("target");
        inCameraTransition = true;
        var targetDirection = target.transform.transform.position - cameraPivot.transform.position;
        var _lookRot = Quaternion.LookRotation(targetDirection);
        var lerpTimer = 0f;
        while (lerpTimer < cameraZoomSpeed)
        {
            print($"lockon = {lockOn}");
            if (!lockOn) 
            {
                yield break;
            }
            mockTransform.transform.position = targetPosition;
            
            cameraPivot.transform.rotation = Quaternion.Slerp(cameraPivot.transform.rotation, _lookRot, lerpTimer/cameraZoomSpeed);
            
            targetPosition = Vector3.Lerp(targetPosition, target.transform.position, lerpTimer/cameraZoomSpeed);
            lerpTimer += Time.deltaTime;
            yield return null;
        }

        inCameraTransition = false;
        targetPosition = target.transform.position;
    }

    private IEnumerator LerpBackFocus()
    {
        print("Back");
        inCameraTransition = true;
        var lerpTimer = 0f;
        while (lerpTimer < cameraZoomSpeed)
        {
            if (lockOn)
            {
                print("I am still here");
                yield break;
            }
            mockTransform.transform.position = targetPosition;
            
            xAxisAngle += -cameraDirection.y * cameraSpeed * Time.fixedDeltaTime;
            yAxisAngle += cameraDirection.x * cameraSpeed * Time.fixedDeltaTime;
            xAxisAngle = Mathf.Clamp(xAxisAngle, -15f, 65f);


            cameraPivot.transform.localRotation = Quaternion.Euler(xAxisAngle, yAxisAngle, 0f);
            lookAtPivot.transform.localRotation = Quaternion.Euler(0f, yAxisAngle, 0f);
            
            targetPosition = Vector3.Lerp(targetPosition, lookAtTarget.transform.position, lerpTimer/cameraZoomSpeed);
            lerpTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        targetPosition = lookAtTarget.transform.position;
        camera.m_LookAt = lookAtTarget.transform;
        inCameraTransition = false;
        Destroy(mockTransform);
    }

    #endregion

    #region Position and Rotation

    private bool _isOnCurvedGround;

    public void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("CurvedGround"))
        {
            _isOnCurvedGround = true;
            var transform1 = transform;
            Ray ray = new Ray(transform1.position, -transform1.up);
            if (Physics.Raycast(ray, out var hit, ground))
            {
                var rotationRef = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up,
                    hit.normal), animCurve.Evaluate(timer));
                transform.localRotation = Quaternion.Euler(rotationRef.eulerAngles.x, rotationRef.eulerAngles.y,
                    rotationRef.eulerAngles.z);
            }
        }
        else if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            _isOnCurvedGround = false;
            var transform1 = transform;
            var reference = Quaternion.Lerp(transform1.rotation, Quaternion.Euler(0, 0, 0),
                animCurve.Evaluate(timer));
            transform.localRotation = Quaternion.Euler(reference.eulerAngles.x, reference.eulerAngles.y,
                reference.eulerAngles.z);
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