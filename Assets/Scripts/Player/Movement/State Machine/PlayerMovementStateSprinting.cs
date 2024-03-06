namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSprinting : PlayerMovementBaseState
    {
        public PlayerMovementStateSprinting(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
        public override void EnterState()
        {
            player.MovementSprintingBaseInstance.DoEnterLogic();
        }

        public override void UpdateState()
        {
            player.MovementSprintingBaseInstance.DoUpdateLogic();

        
            // switch to another state
            if(!player.Grounded)
                manager.SwitchState(player.FallingState);
            
        }

        public override void FixedUpdateState()
        {
            player.MovementSprintingBaseInstance.DoFixedUpdateLogic();

        }
    }
}