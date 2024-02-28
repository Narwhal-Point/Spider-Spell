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
        SpeedControl(player);
        
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
    
    private bool OnSlope(PlayerMovementStateManager player)
    {
        if (Physics.Raycast(player.transform.position, Vector3.down, out _slopeHit, player.playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < player.maxSlopeAngle && angle != 0;
        }
        return false;
    }
    
    private void SpeedControl(PlayerMovementStateManager player)
    {
        // limit speed on slope
        if (OnSlope(player) && !player.ExitingSlope)
        {
            if ( player.Rb.velocity.magnitude > player.MoveSpeed)
                player.Rb.velocity =  player.Rb.velocity.normalized * player.MoveSpeed;
        }
        else // limit speed on ground
        {
            Vector3 flatVel = new Vector3( player.Rb.velocity.x, 0f,  player.Rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > player.MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * player.MoveSpeed;
                player.Rb.velocity = new Vector3(limitedVel.x,  player.Rb.velocity.y, limitedVel.z);
            }
        }
    }

}