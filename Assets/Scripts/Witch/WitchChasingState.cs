using UnityEngine;

public class WitchChasingState : WitchBaseState
{
    private WitchMovement movement;

    public override void EnterState(WitchStateManager witch)
    {
        movement = witch.GetComponent<WitchMovement>();
        Debug.Log("Hello from Chasing state");
    }

    public override void UpdateState(WitchStateManager witch)
    {
        if (witch.playerDetection.canSeePlayer)
        {
            witch.transform.LookAt(witch.playerDetection.targetCoords, Vector3.up);
            movement.moving = true;

            // Commented out code should smooth out rotation, but does not work correctly :(

            // Calculate the rotation needed to look at the target
            // Quaternion targetRotation = Quaternion.LookRotation(witch.playerDetection.targetCoords, Vector3.up);

            // Smoothly interpolate between the current rotation and the target rotation
            // witch.transform.rotation = Quaternion.RotateTowards(witch.transform.rotation, targetRotation, 1f * Time.deltaTime * Mathf.Rad2Deg);
        }
        else
        {
            movement.moving = false;
            witch.SwitchState(witch.SearchingState);
        }
    }

    public override void onCollisionEnter(WitchStateManager witch)
    {
        
    }
}
