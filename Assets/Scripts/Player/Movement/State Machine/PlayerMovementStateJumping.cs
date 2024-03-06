using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateJumping : PlayerMovementBaseState
    {
        private float _jumpTimer;
        private bool ReadyToJump { get; set; } = true;
        private float _desiredMoveSpeed;
        private float _moveSpeed;

        public PlayerMovementStateJumping(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.Rb.useGravity = true;

            ReadyToJump = false;
            Jump();
            // player.Invoke(nameof(ResetJump), player.jumpCooldown);
            _jumpTimer = player.jumpCooldown;
            player.movementState = PlayerMovement.MovementState.Jumping;
            player.Rb.drag = 0; // no ground drag because we're in the air
        }

        public override void UpdateState()
        {
            _jumpTimer -= Time.deltaTime;

            if (_jumpTimer <= 0)
            {
                if(player.Firing)
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
                if (player.Moving != Vector2.zero)
                {
                    if (player.Sprinting)
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
            player.MoveDirection = player.orientation.forward * player.Moving.y +
                                   player.orientation.right * player.Moving.x;


            player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f * player.airMultiplier),
                ForceMode.Force); // move
        }
    }
}