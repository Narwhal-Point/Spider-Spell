using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateDashing : PlayerMovementBaseState
    {
        private float _dashTimer;
        
        public PlayerMovementStateDashing(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }


        public override void EnterState()
        {
            _dashTimer = player.dashDuration;
            player.Rb.AddForce(player.playerObj.transform.forward * player.dashForce, ForceMode.Impulse);
        }

        public override void UpdateState()
        {
            Debug.Log(player.Rb.drag);
            _dashTimer -= Time.deltaTime;
            
            if(_dashTimer <= 0f)
                manager.SwitchState(player.IdleState);
        }
    }
}