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
            _jumpTimer = player.jumpCooldown;
            player.movementState = PlayerMovement.MovementState.Jumping;
            player.Rb.drag = 0; // no ground drag because we're in the air

            // player.jumpingSound.Play();
            player.audioManager.PlaySFX(player.audioManager.jumping);
            Jump();
        }

        public override void UpdateState()
        {
            Movement();
            _jumpTimer -= Time.deltaTime;

            if (_jumpTimer <= 0)
            {
                ResetJump();
                if (!player.Grounded && player.Rb.velocity.y < 0)
                {
                    manager.SwitchState(player.FallingState);
                    return;
                }
            }

            if (player.Grounded && ReadyToJump)
            {
                if (player.InputDirection != Vector2.zero)
                {
                    // if (player.IsSprinting)
                    //     manager.SwitchState(player.SprintingState);
                    // else
                    manager.SwitchState(player.WalkingState);
                }
                else
                    manager.SwitchState(player.IdleState);
            }
        }

        private void Jump()
        {
            player.Rb.useGravity = true;

            // reset y velocity
            var velocity = player.Rb.velocity;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
            player.Rb.velocity = velocity;
            player.PlayLandVFX();
            player.Rb.AddForce(player.playerObj.transform.up * player.jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            ReadyToJump = true;
        }

        private void Movement()
        {
            if (player.InputDirection.y > 0.6)
            {
                player.Rb.AddForce(player.orientation.forward * (player.airSpeed * 100f * Time.deltaTime));
            }

            if (player.InputDirection.y < -0.6)
                player.Rb.AddForce(-player.orientation.forward * (player.airSpeed * 100f * Time.deltaTime));

            if (player.InputDirection.x > 0.6)
            {
                player.Rb.AddForce(player.orientation.right * (player.airSpeed * 100f * Time.deltaTime));
            }

            if (player.InputDirection.x < -0.6)
            {
                player.Rb.AddForce(-player.orientation.right * (player.airSpeed * 100f * Time.deltaTime));
            }
        }
    }
}