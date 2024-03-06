using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateWalking : PlayerMovementBaseState
    {

    
        public PlayerMovementStateWalking(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
        public override void EnterState()
        {
            player.MovementWalkingBaseInstace.DoEnterLogic();
        }

        public override void UpdateState()
        {
            player.MovementWalkingBaseInstace.DoUpdateLogic();
            
            // switch to another state
            if(!player.Grounded)
                manager.SwitchState(player.FallingState);
            else if (player.Moving == Vector2.zero)
                manager.SwitchState(player.IdleState);
        }
    
        public override void FixedUpdateState()
        {
            player.MovementWalkingBaseInstace.DoFixedUpdateLogic();
        }
    

    }
}