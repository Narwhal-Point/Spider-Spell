using UnityEngine;

public class PlayerMovementStateIdle : PlayerMovementBaseState
{
    private float _moveSpeed;
    
    private RaycastHit _slopeHit;
    
    public override void EnterState(PlayerMovementStateManager player)
    {
        player.Rb.useGravity = false;
        player.movementState = PlayerMovementStateManager.MovementState.Idle;
        player.Rb.drag = player.groundDrag;
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        if (player.HorizontalInput != 0 || player.VerticalInput != 0 && player.Grounded)
        {
                player.SwitchState(player.walkingState);
        }
        else if(Input.GetKeyDown(player.crouchKey))
            player.SwitchState(player.crouchingState);
        else if (Input.GetKeyDown(player.jumpKey))
        {
            player.SwitchState(player.jumpingState);
        }
        else if(!player.Grounded)
            player.SwitchState(player.fallingState);
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
    }

}