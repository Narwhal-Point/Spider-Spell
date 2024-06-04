using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponentsAdjuster : MonoBehaviour
{

    public delegate void Transitioned();
    public event Transitioned OntransitionCompleted;

    [SerializeField] GameObject cameraObject;

    //freelook camera script 
    private FreeLookCamera freelookCamera;

    //private CinemachineFreeLook freeLookCam;

    //private CinemachineBrain camBrain;

    private SmoothCamera smoothMovementScript;

    private Transform cachedTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {

    }
    private void Start()
    {

        this.freelookCamera = GetComponent<FreeLookCamera>();
        this.smoothMovementScript = GetComponent<SmoothCamera>();
        cachedTransform = transform;
    }

    public void DelayMethod(Action action, float delay)
    {
        if (action != null)
        {
            StartCoroutine(InvokeActionWithDelay(action, delay));
        }        
    }

    private IEnumerator InvokeActionWithDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public void FreeLook()
    {
        ActivateFreeLook();
        CashedPositionRotation();
        DeactivateFollowCam();
    }

    public void FollowPlayer()
    {
        DeactivateFreeLook();
        OriginalPositionRotation();
        ActivateFollowCam();       
    }

    private void ActivateFreeLook()
    {
        freelookCamera.enabled = true;
        //freeLookCam.enabled = true;
    }

    private void DeactivateFreeLook()
    {
        freelookCamera.enabled = false;
        //freeLookCam.enabled = false;
    }

    private void ActivateFollowCam()
    {
        smoothMovementScript.enabled = true;
    }
    private void DeactivateFollowCam() 
    {
        smoothMovementScript.enabled = false;
    }
    private void CashedPositionRotation()
    {
        cachedTransform.position = originalPosition;
        cachedTransform.rotation = originalRotation;
    }
    private void OriginalPositionRotation()
    {
        originalPosition = cachedTransform.position;
        originalRotation = cachedTransform.rotation;
    }
}
