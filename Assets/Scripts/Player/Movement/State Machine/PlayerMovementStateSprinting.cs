using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSprinting : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;
        private float _moveSpeed;

        public PlayerMovementStateSprinting(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Sprinting;
            _moveSpeed = player.sprintSpeed;


            // ground drag
            player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            // speed check
            SpeedControl();

            // switch to another state
            if (!player.Grounded)
                manager.SwitchState(player.FallingState);
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
        }

        private Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
        }

        private void MovePlayer()
        {
            player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.currentHit);
            player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force); // move
        }

        private void SpeedControl()
        {
            Vector3 flatVel = player.Rb.velocity;

            // limit velocity if needed
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                player.Rb.velocity = limitedVel;
            }
        }
    }
}