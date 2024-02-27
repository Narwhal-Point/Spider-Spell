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
        {
            if (player.HorizontalInput != 0 || player.VerticalInput != 0)
            {
                if(Input.GetKey(player.slideKey))
                    player.SwitchState(player.slidingState); // why no work
                else if(Input.GetKeyDown(player.sprintKey))
                    player.SwitchState(player.sprintingState);
                else
                    player.SwitchState(player.walkingState);
            }
            else
                player.SwitchState(player.idleState);
        }
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
    }
}