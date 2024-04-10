using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateIdle : PlayerMovementBaseState
    {
        public PlayerMovementStateIdle(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }

        public override void EnterState()
        {
            player.Rb.AddForce(-player.transform.up * 2f, ForceMode.Impulse);
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Idle;
            player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            if (player.InputDirection != Vector2.zero)
            {
                // if(player.IsSprinting)
                //     manager.SwitchState(player.SprintingState);
                // else
                    manager.SwitchState(player.WalkingState);
            }
            // else if(player.IsCrouching)
            //     manager.SwitchState(player.CrouchingState);
            else if(!player.Grounded)
                manager.SwitchState(player.FallingState);
            
        }
    
        
    }
}