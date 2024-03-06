using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateIdle : PlayerMovementBaseState
    {
        public PlayerMovementStateIdle(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.MovementIdleBaseInstance.DoEnterLogic();
        }

        public override void UpdateState()
        {
            if (player.Moving != Vector2.zero)
                manager.SwitchState(player.WalkingState);
            else if (player.Crouching)
                manager.SwitchState(player.CrouchingState);
            else if (!player.Grounded)
                manager.SwitchState(player.FallingState);
        }
    }
}