namespace Player.Movement.State_Machine
{
    public class PlayerMovementBaseState
    {
        protected PlayerMovementStateManager manager;
        protected PlayerMovement player; 
    
        // variables needed to be accessed by all states
        protected float MoveSpeed { get; set; }
        protected float DesiredMoveSpeed { get; set; }
        protected float LastDesiredMoveSpeed { get; set; }
        public PlayerMovementBaseState(PlayerMovementStateManager manager, PlayerMovement player)
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