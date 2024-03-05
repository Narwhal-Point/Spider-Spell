using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateFalling : PlayerMovementBaseState
    {
        public PlayerMovementStateFalling(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
    
        public override void EnterState()
        {
            player.movementState = PlayerMovement.MovementState.Falling;
            player.Rb.useGravity = true;
        
            // disable ground drag because otherwise we clamp the y value
            // this took hours to figure out...
            player.Rb.drag = 0f;
        }

        public override void UpdateState()
        {
            if(player.Firing) // start swinging
                manager.SwitchState(player.SwingingState);
            else if(player.Grounded)
            {
                if (player.Moving != Vector2.zero)
                {
                    if(player.Sliding)
                        manager.SwitchState(player.SlidingState);
                    else if(player.Sprinting)
                        manager.SwitchState(player.SprintingState);
                    else
                        manager.SwitchState(player.WalkingState);
                }
                else
                    manager.SwitchState(player.IdleState);
            }
        }
    }
}