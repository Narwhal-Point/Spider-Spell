using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public Transform target; // The target object to follow
    public float followSpeed = 10.0f; // Speed of following the target
    public float lookSpeed = 3.0f; // Speed of camera rotation
    public Vector3 offset = new Vector3(0, 5, -10); // Offset from the target

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize yaw and pitch based on current rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // Ensure the target is assigned
        if (target == null)
        {
            Debug.LogError("Target not assigned. Please assign a target object for the camera to follow.");
        }
    }

    void LateUpdate()
    {
        // Ensure the target is assigned
        if (target == null) return;

        // Get mouse inputs for free look
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -35f, 60f); // Clamp pitch to avoid flipping and unrealistic angles

        // Calculate rotation based on mouse input
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);

        // Calculate the desired position behind the target
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Always look at the target
        transform.LookAt(target.position + Vector3.up * offset.y); // Adjust look at to the target's height
    }
}
