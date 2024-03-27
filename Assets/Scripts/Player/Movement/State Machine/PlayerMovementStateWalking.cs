using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateWalking : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;
        private float _moveSpeed;

        public PlayerMovementStateWalking(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Walking;
            _moveSpeed = player.walkSpeed;

            player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            SpeedControl();

            // switch to another state
            if (!player.Grounded)
                manager.SwitchState(player.FallingState);
            else if (player.Moving == Vector2.zero)
                manager.SwitchState(player.IdleState);
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
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