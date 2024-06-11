using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCinemachineCamera : MonoBehaviour
{
    public FreeLookCinemachineCamera()
    {
        
    }
    private CinemachineFreeLook freeLookCamera;

    // Configurable settings
    [Header("General Settings")]
    public string followTargetTag = "Player";
    public string lookAtTargetTag = "CamLookAt";

    [Header("Axis Control Settings")]
    public float axisSpeed = 300f;
    public bool invertYAxis = false;
    public bool invertXAxis = false;

    [Header("Rig Settings")]
    public float topRigHeight = 6f;
    public float topRigRadius = 4.8f;
    public float middleRigHeight = 3.15f;
    public float middleRigRadius = 8.2f;
    public float bottomRigHeight = -1.31f;
    public float bottomRigRadius = 2.88f;

    private void Start()
    {
        
    }

    public void AddFreeLookCamera()
    {
        freeLookCamera = gameObject.AddComponent<CinemachineFreeLook>();

        // Set targets
        GameObject followTarget = GameObject.FindGameObjectWithTag(followTargetTag);
        GameObject lookAtTarget = GameObject.FindGameObjectWithTag(lookAtTargetTag);

        if (followTarget != null && lookAtTarget != null)
        {
            freeLookCamera.Follow = followTarget.transform;
            freeLookCamera.LookAt = lookAtTarget.transform;
        }
        else
        {
            Debug.LogWarning("Follow or LookAt target not found");
        }

        // Configure Axis Control
        freeLookCamera.m_YAxis.m_MaxSpeed = axisSpeed;
        freeLookCamera.m_XAxis.m_MaxSpeed = axisSpeed;
        freeLookCamera.m_YAxis.m_InvertInput = invertYAxis;
        freeLookCamera.m_XAxis.m_InvertInput = invertXAxis;

        // Configure Rig Settings
        freeLookCamera.m_Orbits[0].m_Height = topRigHeight;
        freeLookCamera.m_Orbits[0].m_Radius = topRigRadius;
        freeLookCamera.m_Orbits[1].m_Height = middleRigHeight;
        freeLookCamera.m_Orbits[1].m_Radius = middleRigRadius;
        freeLookCamera.m_Orbits[2].m_Height = bottomRigHeight;
        freeLookCamera.m_Orbits[2].m_Radius = bottomRigRadius;
    }

    public void DestroyFreeLookCamera()
    {
        if (freeLookCamera != null)
        {
            Destroy(freeLookCamera);
            freeLookCamera = null;
        }
    }
}
