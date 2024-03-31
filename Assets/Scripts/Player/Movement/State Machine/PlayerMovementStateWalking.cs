using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateWalking : PlayerMovementBaseState
    {
         private RaycastHit _slopeHit;
         private float _moveSpeed;
        public float raycastDistance = 1.0f;

        public PlayerMovementStateWalking(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
             player)
         {
         }
        /*
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
     }*/
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
            if (!player.Grounded)
                manager.SwitchState(player.FallingState);
            else if (player.Moving == Vector2.zero)
                manager.SwitchState(player.IdleState);

            DetectClimbableSurfaces();
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 moveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.currentHit);

            player.Rb.AddForce(moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
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

        private void DetectClimbableSurfaces()
        {
            // Array of raycast directions covering different angles around the character
            Vector3[] raycastDirections = {
                player.transform.forward,
                -player.transform.forward,
                player.transform.right,
                -player.transform.right,
                player.transform.forward + player.transform.right,
                player.transform.forward - player.transform.right,
                -player.transform.forward + player.transform.right,
                -player.transform.forward - player.transform.right
            };

            // Perform raycasts in all directions to detect climbable surfaces
            RaycastHit hit;
            foreach (Vector3 dir in raycastDirections)
            {
                if (Physics.Raycast(player.transform.position, dir, out hit, this.raycastDistance))
                {
                    Vector3 surfaceNormal = hit.normal;

                    if (Mathf.Abs(surfaceNormal.y) < 0.1f)
                    {
                        ClimbSurface(surfaceNormal);
                        return; 
                    }
                }
            }
        }
        private void ClimbSurface(Vector3 surfaceNormal)
        {
            Vector3 movementDirection = Vector3.ProjectOnPlane(player.transform.forward, surfaceNormal);

            // Move the player along the surface *Climb speed*
            player.Rb.MovePosition(player.Rb.position + movementDirection * 0.5f * Time.deltaTime);
        }
    }
}