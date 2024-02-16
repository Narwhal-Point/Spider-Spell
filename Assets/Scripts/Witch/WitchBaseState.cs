using UnityEngine;

public abstract class WitchBaseState
{
    public abstract void EnterState(WitchStateManager witch);

    public abstract void UpdateState(WitchStateManager witch);

    public abstract void onCollisionEnter(WitchStateManager witch);
}
