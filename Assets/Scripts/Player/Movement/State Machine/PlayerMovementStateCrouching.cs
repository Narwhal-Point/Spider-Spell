namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateCrouching : PlayerMovementBaseState
    {
        

        public PlayerMovementStateCrouching(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
    
        public override void EnterState()
        {
            player.MovementCrouchingBaseInstance.DoEnterLogic();
        }

        public override void ExitState()
        {
            player.MovementCrouchingBaseInstance.DoExitLogic();

        }

        public override void UpdateState()
        {
            player.MovementCrouchingBaseInstance.DoUpdateLogic();

            
            if (!player.Grounded)
            {
                manager.SwitchState(player.FallingState);
            }
        }

        public override void FixedUpdateState()
        {
            player.MovementCrouchingBaseInstance.DoFixedUpdateLogic();

        }
    
        
    }
}