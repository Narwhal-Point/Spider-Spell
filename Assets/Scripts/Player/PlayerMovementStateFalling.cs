using UnityEngine;

public class PlayerMovementStateFalling : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateManager player)
    {
        player.movementState = PlayerMovementStateManager.MovementState.Falling;
        player.Rb.useGravity = true;
        // var velocity = player.Rb.velocity;
        // velocity = new Vector3(velocity.x, -5f, velocity.z);
        // player.Rb.velocity = velocity;
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        if (player.Grounded)
            player.SwitchState(player.idleState);
        Debug.Log(player.Rb.velocity.y);
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
    }
}