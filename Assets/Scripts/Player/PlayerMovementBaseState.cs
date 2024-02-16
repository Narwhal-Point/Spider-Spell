public abstract class PlayerMovementBaseState
{
    public abstract void EnterState(PlayerMovementStateManager player);

    public abstract void UpdateState(PlayerMovementStateManager player);

    public abstract void FixedUpdateState(PlayerMovementStateManager player);
}