using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateCrouching : PlayerMovementBaseState
    {
        private float _moveSpeed;
        private RaycastHit _slopeHit;

        public PlayerMovementStateCrouching(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
    
        public override void EnterState()
        {
            player.crouchSound.Play();   
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Crouching;

            DesiredMoveSpeed = player.crouchSpeed;

            _moveSpeed = DesiredMoveSpeed;
            player.Rb.drag = player.groundDrag;


            player.transform.localScale = new Vector3(player.transform.localScale.x, player.crouchYScale,
                player.transform.localScale.z);
            player.Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        public override void ExitState()
        {
            player.uncrouchSound.Play();
        }

        public override void UpdateState()
        {
            SpeedControl();

            if (Input.GetKeyUp(player.crouchKey))
            {
                player.transform.localScale = new Vector3(player.transform.localScale.x, player.StartYScale,
                    player.transform.localScale.z);

                if (player.HorizontalInput != 0 || player.VerticalInput != 0)
                {
                    manager.SwitchState(player.WalkingState);
                }
                else
                {
                    manager.SwitchState(player.IdleState);
                }
            }

            if (!player.Grounded)
            {
                manager.SwitchState(player.FallingState);
            }
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
        }
    
        private bool OnSlope()
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out _slopeHit, player.playerHeight * 0.5f + 0.3f))
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
            // get the direction to move towards
            player.MoveDirection = player.orientation.forward * player.VerticalInput +
                                   player.orientation.right * player.HorizontalInput;

            // player is on a slope
            if (OnSlope() && !player.ExitingSlope)
            {
                player.Rb.AddForce(GetSlopeMoveDirection(player.MoveDirection) * (_moveSpeed * 20f), ForceMode.Force);

                if (player.Rb.velocity.y > 0)
                    player.Rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

            // player on the ground
            else if (player.Grounded)
            {
                player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f),
                    ForceMode.Force); // move
            }
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
                Vector3 flatVel = new Vector3(player.Rb.velocity.x, 0f, player.Rb.velocity.z);

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