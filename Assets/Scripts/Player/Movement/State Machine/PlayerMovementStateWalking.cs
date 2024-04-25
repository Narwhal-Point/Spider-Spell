using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
            player.walkingSound.Play();
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
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
        }
        private void MovePlayer()
        {
            Vector3 inputDirection = new Vector3(player.InputDirection.x, 0f, player.InputDirection.y);

            // Check if there is any input (not zero vector)
            if (inputDirection != Vector3.zero)
            {
                Debug.Log("Y: " + player.InputDirection.y + "/// Z: " + inputDirection.z);
                // If moving forward, use camera direction
                if (inputDirection.z > 0)
                {
                    // Get the camera's forward direction projected onto the XZ plane
                    Vector3 cameraForward = Vector3.Scale(player.cam.transform.forward, new Vector3(1, 0, 1)).normalized;
                    inputDirection = Quaternion.LookRotation(cameraForward) * inputDirection;
                }
                else if (inputDirection.z < 0)
                {
                    Vector3 cameraBackward = Vector3.Scale(-player.cam.transform.forward, new Vector3(1, 0, -1)).normalized;
                    inputDirection = Quaternion.LookRotation(cameraBackward) * inputDirection;
                    // If moving backward, do not adjust input direction
                    //inputDirection = new Vector3(player.InputDirection.x, 0f, player.InputDirection.y);
                }
                Debug.Log(inputDirection);
                // Calculate move direction using input direction
                player.MoveDirection = CalculateMoveDirection(inputDirection, player.groundHit);
            }
            RotatePlayer(inputDirection);

            // Apply forces
            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
        }
        public Vector3 CalculateMoveDirection(Vector3 direction, RaycastHit hit)
        {
            // Calculate facing rotation based on input direction
            Quaternion facingRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Get surface rotation from the ground hit normal
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Combine facing and surface rotations
            Quaternion combinedRotation = surfaceRotation * facingRotation;

            // Calculate move direction
            Vector3 moveDirection = combinedRotation * Vector3.forward;        

            return moveDirection;
        }

        private void RotatePlayer(Vector3 inputDirection)
        {
            // If there is any input (not zero vector)
            if (inputDirection != Vector3.zero)
            {
                // Calculate the rotation angle based on the input direction
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

                // Smoothly rotate towards the target angle
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                player.playerObj.rotation = Quaternion.Slerp(player.playerObj.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        private void HandleRotation()
        {
            float cos70 = Mathf.Cos(70 * Mathf.Deg2Rad);

            // get the dot product of the ground normal and the angleHit normal to check the angle between them.
            float dotProduct = Vector3.Dot(player.groundHit.normal.normalized, player.angleHit.normal.normalized);

            if (player._manager.CurrentState == player.JumpingState)
                return;

            player.facingAngles = (2f, 2f);

            if (player.WallInFront && player.InputDirection != Vector2.zero && player._manager.CurrentState != player.SwingingState)
            {
               
            }
            else if (player.WallInFrontLow && player.InputDirection != Vector2.zero && player._manager.CurrentState != player.SwingingState)
            {
               
            }
            // if an edge is found and the angle between the normals is 90 degrees or more align the player with the new surface
            else if (player.EdgeFound && player.InputDirection != Vector2.zero && dotProduct <= cos70 && player._manager.CurrentState != player.SwingingState)
            {
              
            }
            // TODO: Change camera player rotation
            else if (player.Grounded && player.InputDirection != Vector2.zero || player._manager.CurrentState == player.SwingingState)
            {
               
            }
            else if (player.IsHeadHit && player._manager.CurrentState != player.SwingingState)
            {
              
            }
            else if (player.InputDirection != Vector2.zero)
            {
                
            }
        }
        private void RotatePlayer()
        {

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

        public override void ExitState()
        {
            player.walkingSound.Stop();
        }
    }
}