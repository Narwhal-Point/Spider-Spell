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

    private FreeLookCinemachineCamera freeLookCam;

    private CinemachineBrain camBrain;

    private SmoothCamera smoothMovementScript;

    private void Awake()
    {

    }
    private void Start()
    {
        this.freeLookCam = cameraObject.GetComponent<FreeLookCinemachineCamera>();
        this.camBrain = cameraObject.GetComponent<CinemachineBrain>();
        this.smoothMovementScript = cameraObject.GetComponent<SmoothCamera>();
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
        DeactivateFollowCam();
    }

    public void FollowPlayer()
    {
        DeactivateFreeLook();
        ActivateFollowCam();
    }

    private void ActivateFreeLook()
    {
        freeLookCam.enabled = true;
    }

    private void DeactivateFreeLook()
    {
        freeLookCam.enabled = false;
    }

    private void ActivateFollowCam()
    {
        smoothMovementScript.enabled = true;
    }
    private void DeactivateFollowCam() 
    {
        smoothMovementScript.enabled = false;
    }
}
