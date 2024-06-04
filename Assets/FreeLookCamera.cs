using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCamera : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10.0f;
    public float lookSpeed = 3.0f;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float lerpSpeed = 5.0f;
    public float rotationLerpSpeed = 5.0f;
    public float collisionRadius = 0.5f;
    public LayerMask collisionLayers;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAtTarget;

    private Vector3 sphereCastStart;
    private Vector3 sphereCastEnd;
    private PlayerInput _playerInput;
    InputAction lookInput;

    private void Awake()
    {
        //Input Manager
        //_playerInput = GetComponent<PlayerInput>();
        //lookInput = _playerInput.actions["Look"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;       

        if (target != null)
        {
            lookAtTarget = target.position + Vector3.up * offset.y;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        //Vector2 lookValue = lookInput.ReadValue<Vector2>();

        /* yaw += lookSpeed * lookValue.x;
         pitch -= lookSpeed * lookValue.y;*/

        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -35f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        // sphere cast
        sphereCastStart = target.position;
        sphereCastEnd = desiredPosition;

        // Collision detection and adjustment
        RaycastHit hit;
        if (Physics.SphereCast(sphereCastStart, collisionRadius, desiredPosition - sphereCastStart, out hit, offset.magnitude, collisionLayers))
        {
            desiredPosition = target.position + (hit.point - target.position).normalized * (hit.distance - collisionRadius);

            sphereCastEnd = desiredPosition;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSpeed * Time.deltaTime);

        //Snooth lookAt
        Vector3 targetPosition = target.position + Vector3.up * offset.y;
        lookAtTarget = Vector3.Lerp(lookAtTarget, targetPosition, lerpSpeed * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(lookAtTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationLerpSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (target == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sphereCastStart, collisionRadius);
        Gizmos.DrawLine(sphereCastStart, sphereCastEnd);
        Gizmos.DrawWireSphere(sphereCastEnd, collisionRadius);
    }
}
