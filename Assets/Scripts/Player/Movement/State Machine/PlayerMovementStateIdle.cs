using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateIdle : PlayerMovementBaseState
    {
        private float _moveSpeed;
    
        private RaycastHit _slopeHit;
    
        public PlayerMovementStateIdle(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }

        public override void EnterState()
        {
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Idle;
            player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            SpeedControl();

            if (player.Moving != Vector2.zero)
            {
                if(player.Sprinting)
                    manager.SwitchState(player.SprintingState);
                else
                    manager.SwitchState(player.WalkingState);
            }
            else if(player.Crouching)
                manager.SwitchState(player.CrouchingState);
            else if(!player.Grounded)
                manager.SwitchState(player.FallingState);
            
        }

        public override void FixedUpdateState()
        {
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
    
        private void SpeedControl()
        {
            // limit speed on slope
            if (OnSlope() && !player.ExitingSlope)
            {
                if ( player.Rb.velocity.magnitude > MoveSpeed)
                    player.Rb.velocity =  player.Rb.velocity.normalized * MoveSpeed;
            }
            else // limit speed on ground
            {
                Vector3 flatVel = new Vector3( player.Rb.velocity.x, 0f,  player.Rb.velocity.z);

                // limit velocity if needed
                if (flatVel.magnitude > MoveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * MoveSpeed;
                    player.Rb.velocity = new Vector3(limitedVel.x,  player.Rb.velocity.y, limitedVel.z);
                }
            }
        }
    }
}