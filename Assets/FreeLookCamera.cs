using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10.0f;
    public float lookSpeed = 3.0f;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float lerpSpeed = 5.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAtTarget;

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

    void LateUpdate()
    {
        if (target == null) return;

        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -35f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSpeed * Time.deltaTime);

        Vector3 targetPosition = target.position + Vector3.up * offset.y;
        lookAtTarget = Vector3.Lerp(lookAtTarget, targetPosition, lerpSpeed * Time.deltaTime);

        transform.LookAt(lookAtTarget);
    }
}
