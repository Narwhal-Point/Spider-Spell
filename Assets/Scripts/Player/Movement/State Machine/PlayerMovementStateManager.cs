namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateManager
    {

        public PlayerMovementBaseState CurrentState { get; private set; }

        public void Initialize(PlayerMovementBaseState startingState)
        {
            CurrentState = startingState;
            CurrentState.EnterState();
        }
    
        public void SwitchState(PlayerMovementBaseState state)
        {
            CurrentState.ExitState();
            CurrentState = state;
            CurrentState.EnterState();
        }
    
    }
}