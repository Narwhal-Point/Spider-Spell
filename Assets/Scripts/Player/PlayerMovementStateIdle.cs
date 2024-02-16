using UnityEngine;

public class PlayerMovementStateIdle : PlayerMovementBaseState
{
    private float _moveSpeed;
    
    private RaycastHit _slopeHit;
    
    public override void EnterState(PlayerMovementStateManager player)
    {
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
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
    }

}