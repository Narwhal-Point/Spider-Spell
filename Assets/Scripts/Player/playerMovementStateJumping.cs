using UnityEngine;

public class playerMovementStateJumping : PlayerMovementBaseState
{
    private float jumpTimer;
    public override void EnterState(PlayerMovementStateManager player)
    {
        player.Rb.useGravity = true;

        player.ReadyToJump = false;
        Jump(player);
        // player.Invoke(nameof(ResetJump), player.jumpCooldown);
        jumpTimer = player.jumpCooldown;
        player.movementState = PlayerMovementStateManager.MovementState.Jumping;
        player.Rb.drag = 0; // no ground drag because we're in the air
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        if (player.Grounded && player.ReadyToJump)
        {
            if (player.HorizontalInput != 0 || player.VerticalInput != 0)
            {
                if(Input.GetKeyDown(player.sprintKey))
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
        MovePlayer(player);

        jumpTimer -= Time.deltaTime;

        if (jumpTimer <= 0)
        {
            ResetJump(player);
            if(!player.Grounded && player.Rb.velocity.y < 0)
                player.SwitchState(player.fallingState);
        }
    }

    private void Jump(PlayerMovementStateManager player)
    {
        player.ExitingSlope = true;
        player.Rb.useGravity = true;

        // reset y velocity
        var velocity = player.Rb.velocity;
        velocity = new Vector3(velocity.x, 0f, velocity.z);
        player.Rb.velocity = velocity;
        
        player.Rb.AddForce(player.transform.up * player.jumpForce, ForceMode.Impulse);
    }

    private void ResetJump(PlayerMovementStateManager player)
    {
        player.ReadyToJump = true;
        player.ExitingSlope = false;
    }

    private void MovePlayer(PlayerMovementStateManager player)
    {
        // get the direction to move towards
        player.MoveDirection = player.orientation.forward * player.VerticalInput +
                               player.orientation.right * player.HorizontalInput;


        player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f * player.airMultiplier),
            ForceMode.Force); // move
    }
}