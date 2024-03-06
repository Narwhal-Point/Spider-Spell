using System.Collections;
using System.Collections.Generic;
using Player.Movement;
using UnityEngine;

[CreateAssetMenu(fileName = "Movement-Sprinting", menuName = "Movement Logic/Sprinting Logic")]
public class MovementSprintingLogic : MovementSprintingSOBASE
{
    [SerializeField] private float sprintSpeed = 10f;
    
    private RaycastHit _slopeHit;
    private float _moveSpeed;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        
        player.Rb.useGravity = false;
        player.movementState = PlayerMovement.MovementState.Sprinting;

        _moveSpeed = sprintSpeed; // should probably make this better

        // ground drag
        player.Rb.drag = player.groundDrag;
    }

    public override void DoUpdateLogic()
    {
        base.DoUpdateLogic();
        
        // speed check
        SpeedControl();
    }

    public override void DoFixedUpdateLogic()
    {
        base.DoFixedUpdateLogic();
        
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
            player.MoveDirection = player.orientation.forward * player.Moving.y +
                                   player.orientation.right * player.Moving.x;

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
