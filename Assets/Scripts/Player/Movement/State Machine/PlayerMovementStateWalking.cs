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

            //player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            SpeedControl();
            /*if (!player.Grounded)
                manager.SwitchState(player.FallingState);*/
           if (player.Moving == Vector2.zero)
                manager.SwitchState(player.IdleState);

            DetectClimbableSurfaces();
            MovePlayer();
        }

        public override void FixedUpdateState()
        {
           
        }

        private void MovePlayer()
        {
            Vector3 moveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.currentHit);
            Vector3 newPosition = player.transform.position + moveDirection.normalized * (_moveSpeed * Time.deltaTime);

            // Move the player's position directly
            player.transform.position = newPosition;
            /*Vector3 moveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.currentHit);

            player.Rb.AddForce(moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);*/
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
            /*//WOrking sphere casting
            RaycastHit[] hits = Physics.SphereCastAll(player.transform.position, 0.8f, Vector3.down, 0.8f);
            Debug.DrawRay(player.transform.position, Vector3.down * 0.8f, Color.blue);

            foreach (RaycastHit hit in hits)
            {
                Vector3 surfaceNormal = hit.normal;

                if (Mathf.Abs(surfaceNormal.y) < 0.1f)
                {
                    ClimbSurface(surfaceNormal);
                    return;
                    // Exit the loop after finding a climbable surface
                }
            }*/
            Vector3 bottomPosition = player.transform.position - Vector3.up * (player.GetComponent<Collider>().bounds.extents.y);
            // Array of raycast directions covering different angles around the character
            Vector3[] raycastDirections = {
                  player.transform.forward,
                  -player.transform.forward,
                  player.transform.right,
                  -player.transform.right,
                  player.transform.forward + player.transform.right,
                  player.transform.forward - player.transform.right,
                  -player.transform.forward + player.transform.right,
                  -player.transform.forward - player.transform.right,
                  -player.transform.up
              };
            RaycastHit hit;
            foreach (Vector3 dir in raycastDirections)
            {
                if (Physics.Raycast(bottomPosition, dir, out hit, this.raycastDistance))
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

            // Calculate the new position based on the movement direction
            Vector3 newPosition = player.transform.position + movementDirection * 0.5f * Time.deltaTime;

            // Move the player's position directly
            player.transform.position = newPosition;
            /* Vector3 movementDirection = Vector3.ProjectOnPlane(player.transform.forward, surfaceNormal);

             // Move the player along the surface *Climb speed*
             player.Rb.MovePosition(player.Rb.position + movementDirection * 0.5f * Time.deltaTime);*/
        }
    }
}