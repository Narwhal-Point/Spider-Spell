using UnityEngine;

public class PlayerMovementStateFalling : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        player.movementState = PlayerMovementStateManager.MovementState.Falling;
        player.Rb.useGravity = true;
        
        // disable ground drag because otherwise we clamp the y value
        // this took hours to figure out...
        player.Rb.drag = 0f;
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        if (player.Grounded)
            player.SwitchState(player.idleState);
        Debug.Log(player.Rb.velocity.y);
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
    }
}