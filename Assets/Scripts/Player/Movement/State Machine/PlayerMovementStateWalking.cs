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
            else if (player.InputDirection == Vector2.zero)
                manager.SwitchState(player.IdleState);

            // DetectClimbableSurfaces();
            
            // MovePlayer();
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            if (player.IsSnapping)
                return;
            player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.groundHit);

            // Vector3 newPos = player.transform.position +
            //                  player.MoveDirection.normalized * (_moveSpeed * Time.deltaTime);
            //
            // player.transform.position = newPos;
            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
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
                player.playerObj.forward,
                -player.playerObj.forward,
                player.playerObj.right,
                -player.playerObj.right,
                player.playerObj.forward + player.playerObj.right,
                player.playerObj.forward - player.playerObj.right,
                -player.playerObj.forward + player.playerObj.right,
                -player.playerObj.forward - player.playerObj.right
            };

            // Perform raycasts in all directions to detect climbable surfaces
            RaycastHit hit;
            foreach (Vector3 dir in raycastDirections)
            {
                Debug.DrawRay(player.transform.position - player.playerObj.up, dir * raycastDistance, Color.magenta);
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
            // Vector3 movementDirection = Vector3.ProjectOnPlane(player.playerObj.forward, surfaceNormal);

            // Move the player along the surface *Climb speed*
            Vector3 movementDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.groundHit);

            player.Rb.MovePosition(player.Rb.position + movementDirection * 0.5f * Time.deltaTime);
        }
    }
}