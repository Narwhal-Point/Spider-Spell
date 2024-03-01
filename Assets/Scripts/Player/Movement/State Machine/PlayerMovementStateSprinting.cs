using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSprinting : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;

        public PlayerMovementStateSprinting(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
        public override void EnterState()
        {
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Sprinting;
            DesiredMoveSpeed = player.sprintSpeed;

            MoveSpeed = DesiredMoveSpeed; // should probably make this better

            // ground drag
            player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            // speed check
            SpeedControl();
        
            // switch to another state
            if (Input.GetKeyDown(player.slideKey))
                manager.SwitchState(player.SlidingState);
            else if (Input.GetKeyDown(player.jumpKey))
                manager.SwitchState(player.JumpingState);
            else if (Input.GetKeyUp(player.sprintKey))
                manager.SwitchState(player.WalkingState);
            else if(!player.Grounded)
                manager.SwitchState(player.FallingState);
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
                player.Rb.AddForce(GetSlopeMoveDirection(player.MoveDirection) * (MoveSpeed * 20f), ForceMode.Force);

                if (player.Rb.velocity.y > 0)
                    player.Rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

            // player on the ground
            else if (player.Grounded)
            {
                player.Rb.AddForce(player.MoveDirection.normalized * (MoveSpeed * 10f),
                    ForceMode.Force); // move
            }
        }

        private void SpeedControl()
        {
            // limit speed on slope
            if (OnSlope() && !player.ExitingSlope)
            {
                if (player.Rb.velocity.magnitude > MoveSpeed)
                    player.Rb.velocity = player.Rb.velocity.normalized * MoveSpeed;
            }
            else // limit speed on ground
            {
                Vector3 flatVel = new Vector3(player.Rb.velocity.x, 0f, player.Rb.velocity.z);

                // limit velocity if needed
                if (flatVel.magnitude > MoveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * MoveSpeed;
                    player.Rb.velocity = new Vector3(limitedVel.x, player.Rb.velocity.y, limitedVel.z);
                }
            }
        }
    }
}