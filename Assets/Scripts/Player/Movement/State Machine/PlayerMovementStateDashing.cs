using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateDashing : PlayerMovementBaseState
    {
        private float _dashDuration;
        private float _dashForce;
        private float _dashUpwardForce;
        private float _dashCooldown;

        private float _dashTimer;
        

        private float lastDashTime = 2f;
        public PlayerMovementStateDashing(PlayerMovementStateManager manager, PlayerMovement player, float dashDuration, float dashForce, float dashCooldown, float dashUpwardForce) : base(manager,
            player)
        {
            _dashDuration = dashDuration;
            _dashForce = dashForce;
            _dashCooldown = dashCooldown;
            _dashUpwardForce = dashUpwardForce;
        }


        public override void EnterState()
        {
            Debug.Log(Time.time - lastDashTime);
            if (Time.time - lastDashTime <= _dashCooldown)
            {
                manager.SwitchState(player.FallingState);
                return;
            }
            
            player.Rb.drag = 0f;
            player.Rb.useGravity = true;
            player.IsDashing = true;
            _dashTimer = _dashDuration;
            lastDashTime = Time.time;
            Vector3 forceToApply = player.orientation.forward * _dashForce + player.orientation.up * _dashUpwardForce;

            player.Rb.AddForce(forceToApply, ForceMode.Impulse);
            
        }

        private void ResetDash()
        {
            player.IsDashing = false;
            // player.Rb.velocity = Vector3.zero;
        }


        public override void UpdateState()
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                ResetDash();
                manager.SwitchState(player.FallingState);
            }
        }
    }
}