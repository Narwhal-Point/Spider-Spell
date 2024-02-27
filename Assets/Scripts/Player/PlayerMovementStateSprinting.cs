using UnityEngine;

public class PlayerMovementStateSprinting : PlayerMovementBaseState
{

    private RaycastHit _slopeHit;

    public override void EnterState(PlayerMovementStateManager player)
    {
        player.Rb.useGravity = false;
        player.movementState = PlayerMovementStateManager.MovementState.Sprinting;
        player.DesiredMoveSpeed = player.sprintSpeed;

        player.MoveSpeed = player.DesiredMoveSpeed; // should probably make this better

        // ground drag
        player.Rb.drag = player.groundDrag;
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        // speed check
        SpeedControl(player);
        
        // switch to another state
        if (Input.GetKeyDown(player.slideKey))
            player.SwitchState(player.slidingState);
        else if (Input.GetKeyDown(player.jumpKey))
            player.SwitchState(player.jumpingState);
        else if (Input.GetKeyUp(player.sprintKey))
            player.SwitchState(player.walkingState);
        else if(!player.Grounded)
            player.SwitchState(player.fallingState);
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
        MovePlayer(player);
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

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
    }

    private void MovePlayer(PlayerMovementStateManager player)
    {
        // get the direction to move towards
        player.MoveDirection = player.orientation.forward * player.VerticalInput +
                               player.orientation.right * player.HorizontalInput;

        // player is on a slope
        if (OnSlope(player) && !player.ExitingSlope)
        {
            player.Rb.AddForce(GetSlopeMoveDirection(player.MoveDirection) * player.MoveSpeed * 20f, ForceMode.Force);

            if (player.Rb.velocity.y > 0)
                player.Rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // player on the ground
        else if (player.Grounded)
        {
            player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f),
                ForceMode.Force); // move
        }
    }

    private void SpeedControl(PlayerMovementStateManager player)
    {
        // limit speed on slope
        if (OnSlope(player) && !player.ExitingSlope)
        {
            if (player.Rb.velocity.magnitude > player.MoveSpeed)
                player.Rb.velocity = player.Rb.velocity.normalized * player.MoveSpeed;
        }
        else // limit speed on ground
        {
            Vector3 flatVel = new Vector3(player.Rb.velocity.x, 0f, player.Rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > player.MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * player.MoveSpeed;
                player.Rb.velocity = new Vector3(limitedVel.x, player.Rb.velocity.y, limitedVel.z);
            }
        }
    }
}