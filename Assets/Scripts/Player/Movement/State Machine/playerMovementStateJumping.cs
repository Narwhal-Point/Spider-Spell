using UnityEngine;

public class playerMovementStateJumping : PlayerMovementBaseState
{
    private float jumpTimer;
    protected bool ReadyToJump { get; set; } = true;
    
    public playerMovementStateJumping(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
    {
    }
    public override void EnterState()
    {
        player.Rb.useGravity = true;

        ReadyToJump = false;
        Jump();
        // player.Invoke(nameof(ResetJump), player.jumpCooldown);
        jumpTimer = player.jumpCooldown;
        player.movementState = PlayerMovement.MovementState.Jumping;
        player.Rb.drag = 0; // no ground drag because we're in the air
    }

    public override void UpdateState()
    {
        jumpTimer -= Time.deltaTime;

        if (jumpTimer <= 0)
        {
            if (Input.GetKeyDown(player.swingKey))
                manager.SwitchState(player.SwingingState);
            ResetJump();
            if (!player.Grounded && player.Rb.velocity.y < 0)
            {
                manager.SwitchState(player.FallingState);
                return;
            }
        }
        
        if (player.Grounded && ReadyToJump)
        {
            if (player.HorizontalInput != 0 || player.VerticalInput != 0)
            {
                if(Input.GetKeyDown(player.slideKey))
                    manager.SwitchState(player.SlidingState);
                else if(Input.GetKeyDown(player.sprintKey))
                    manager.SwitchState(player.SprintingState);
                else
                    manager.SwitchState(player.WalkingState);
            }
            else
                manager.SwitchState(player.IdleState);
        }
    }

    public override void FixedUpdateState()
    {
        MovePlayer();
    }

    private void Jump()
    {
        player.ExitingSlope = true;
        player.Rb.useGravity = true;

        // reset y velocity
        var velocity = player.Rb.velocity;
        velocity = new Vector3(velocity.x, 0f, velocity.z);
        player.Rb.velocity = velocity;
        
        player.Rb.AddForce(player.transform.up * player.jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        ReadyToJump = true;
        player.ExitingSlope = false;
    }

    private void MovePlayer()
    {
        // get the direction to move towards
        player.MoveDirection = player.orientation.forward * player.VerticalInput +
                               player.orientation.right * player.HorizontalInput;


        player.Rb.AddForce(player.MoveDirection.normalized * (MoveSpeed * 10f * player.airMultiplier),
            ForceMode.Force); // move
    }
}