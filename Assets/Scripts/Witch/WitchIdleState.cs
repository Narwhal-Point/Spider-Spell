using System;
using Unity.VisualScripting;
using UnityEngine;

public class WitchIdleState : WitchBaseState
{
    public override void EnterState(WitchStateManager witch)
    {
        Debug.Log("Hello from idle state");
    }

    public override void UpdateState(WitchStateManager witch)    
    {
        witch.transform.Rotate(Vector3.up, witch.idleRotationSpeed * Time.deltaTime);
        if (witch.playerDetection.canSeePlayer)
        {
            // Debug.Log("player found!");
            witch.SwitchState(witch.ChasingState);
        }
    }

    public override void onCollisionEnter(WitchStateManager witch)    
    {
        
    }
}
