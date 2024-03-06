namespace Player.Movement.State_Machine
{
    public class PlayerMovementBaseState
    {
        protected readonly PlayerMovementStateManager manager;
        protected readonly PlayerMovement player; 
        protected PlayerMovementBaseState(PlayerMovementStateManager manager, PlayerMovement player)
        {
            this.manager = manager;
            this.player = player;
        }
        public virtual void EnterState() {}
    
        public virtual void ExitState() {}

        public virtual void UpdateState() {}

        public virtual void FixedUpdateState() {}
    }
}