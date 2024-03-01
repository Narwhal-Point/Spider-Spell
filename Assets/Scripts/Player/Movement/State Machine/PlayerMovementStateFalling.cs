using UnityEngine;

public class PlayerMovementStateFalling : PlayerMovementBaseState
{
    public PlayerMovementStateFalling(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
    {
    }
    
    public override void EnterState()
    {
        player.movementState = PlayerMovement.MovementState.Falling;
        player.Rb.useGravity = true;
        
        // disable ground drag because otherwise we clamp the y value
        // this took hours to figure out...
        player.Rb.drag = 0f;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(player.swingKey))
            manager.SwitchState(player.SwingingState);
        if (player.Grounded)
        {
            if (player.HorizontalInput != 0 || player.VerticalInput != 0)
            {
                if(Input.GetKey(player.slideKey))
                    manager.SwitchState(player.SlidingState); // why no work
                else if(Input.GetKeyDown(player.sprintKey))
                    manager.SwitchState(player.SprintingState);
                else
                    manager.SwitchState(player.WalkingState);
            }
            else
                manager.SwitchState(player.IdleState);
        }
    }
}