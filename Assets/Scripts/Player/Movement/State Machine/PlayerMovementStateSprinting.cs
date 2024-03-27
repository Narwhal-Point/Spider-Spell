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

        private bool OnSlope()
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out _slopeHit,
                    player.playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < player.maxSlopeAngle && angle != 0;
            }

            return false;
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
            // limit speed on slope
            if (OnSlope() && !player.ExitingSlope)
            {
                if (player.Rb.velocity.magnitude > _moveSpeed)
                    player.Rb.velocity = player.Rb.velocity.normalized * _moveSpeed;
            }
            else // limit speed on ground
            {
                var rbVelocity = player.Rb.velocity;
                Vector3 flatVel = new Vector3(rbVelocity.x, 0f, rbVelocity.z);

                // limit velocity if needed
                if (flatVel.magnitude > _moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                    player.Rb.velocity = new Vector3(limitedVel.x, player.Rb.velocity.y, limitedVel.z);
                }
            }
        }
    }
}