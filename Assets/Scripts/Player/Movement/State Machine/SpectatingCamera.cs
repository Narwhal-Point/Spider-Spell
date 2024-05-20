using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatingCamera : CameraAbstract
{
    private Vector3 lastPosition;

    protected override void Awake()
    {
        base.Awake();
        lastPosition = observedObject.position;
    }

    protected override void Update()
    {
        base.Update();
        updateCameraTarget();
    }

    private void updateCameraTarget()
    {

        // Position
        Vector3 translation = observedObject.position - lastPosition;
        camTarget.position += translation;
        lastPosition = observedObject.position;

        //Rotation
        Vector3 newForward = Vector3.ProjectOnPlane(observedObject.position - camTarget.position, Vector3.up);
        if (newForward != Vector3.zero)
            camTarget.rotation = Quaternion.LookRotation(observedObject.position - camTarget.position, Vector3.up);
    }

    protected override Vector3 getHorizontalRotationAxis()
    {
        return Vector3.up;
    }
    protected override Vector3 getVerticalRotationAxis()
    {
        return camTarget.right;
    }
}
