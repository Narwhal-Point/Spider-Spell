using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovewithCamera : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public Transform targetObject;
    public float heightOffset = 2.0f; // Adjust this to set the desired height above the ground

    private void Awake()
    {
        targetObject.position = freeLookCamera.transform.position;
    }
    private void Update()
    {
        if (freeLookCamera != null && targetObject != null)
        {
            // Get the camera's forward direction without vertical component
            Vector3 cameraForward = Vector3.ProjectOnPlane(freeLookCamera.transform.forward, Vector3.up).normalized;

            // Calculate the target position by adding the horizontal movement of the camera and the height offset
            Vector3 targetPosition = freeLookCamera.transform.position + cameraForward + Vector3.up * heightOffset;

            // Move the target object to the calculated position
            targetObject.position = targetPosition;
        }
    }
}
