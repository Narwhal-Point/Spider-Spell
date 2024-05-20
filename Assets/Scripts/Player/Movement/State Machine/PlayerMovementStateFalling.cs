using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateFalling : PlayerMovementBaseState
    {
        private float _moveSpeed;
        public PlayerMovementStateFalling(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.movementState = PlayerMovement.MovementState.Falling;
            player.Rb.useGravity = true;

            // disable ground drag because otherwise we clamp the y value
            // this took hours to figure out...
            player.Rb.drag = 0f;
            
            player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            // using swingSpeed for now.
            player.DesiredMoveSpeed = player.swingSpeed;
            
            if (Mathf.Abs(player.DesiredMoveSpeed - player.lastDesiredMoveSpeed) > 4f && player.MoveSpeed != 0)
                player.ChangeMomentum(2f);
            else
                player.MoveSpeed = player.DesiredMoveSpeed;
        }

        public override void UpdateState()
        {
            Movement();
            
            if (player.Grounded)
            {
                if (player.InputDirection != Vector2.zero)
                {
                    if (player.IsSliding)
                        manager.SwitchState(player.SlidingState);
                    // else if (player.IsSprinting)
                    //     manager.SwitchState(player.SprintingState);
                    else
                        manager.SwitchState(player.WalkingState);
                }
                else
                    manager.SwitchState(player.IdleState);
            }
        }
        

        public override void ExitState()
        {
            if (player.Grounded)
            {
                player.audioManager.PlaySFX(player.audioManager.landing);
                player.PlayLandVFX();
            }
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