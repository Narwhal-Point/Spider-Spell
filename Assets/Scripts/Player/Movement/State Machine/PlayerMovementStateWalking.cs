using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
            Vector3 direction = new Vector3(player.InputDirection.x, 0f, player.InputDirection.y).normalized;
            if (direction.magnitude >= 0.1f)
            {
                MovePlayer();
                TurnPlayer();
            }
            TransitionPlayer();
        }
        private void TurnPlayer()
        {
            Vector3 forward = player.movementForward.normalized;
            Vector3 right = player.movementRight.normalized;

            Vector3 forwardRelativeInput = player.InputDirection.y * forward;
            Vector3 rightRelativeInput = player.InputDirection.x * right;
            // Calculate the combined movement direction relative to the camera
            Vector3 combinedMovement = forwardRelativeInput + rightRelativeInput;

            // Project the combined movement onto the horizontal plane
            combinedMovement = Vector3.ProjectOnPlane(combinedMovement, player.transform.up).normalized;

            // Check if movement is negligible or zero
            if (combinedMovement == Vector3.zero || Vector3.Angle(combinedMovement, player.transform.forward) < Mathf.Epsilon)
            {
                return;
            }

            // Project the combined movement onto the horizontal plane again (not normalized this time)
            combinedMovement = Vector3.ProjectOnPlane(combinedMovement, player.transform.up);

            // Rotate towards the combined movement direction
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, Quaternion.LookRotation(combinedMovement, player.transform.up), 15f);           
        }

        private void TransitionPlayer()
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
                if (Physics.Raycast(player.transform.position, dir, out hit, 1f))
                {
                    Vector3 surfaceNormal = hit.normal;

                    if (Mathf.Abs(surfaceNormal.y) < 0.1f)
                    {
                        ClimbSurface(surfaceNormal);
                        return; // Exit the loop if climbing is initiated
                    }
                }
            }
        }
        private void ClimbSurface(Vector3 surfaceNormal)
        {
            //climbingSurface = true;
            player.transform.up = surfaceNormal;

            Vector3 inputDir = new Vector3(player.InputDirection.x, 0f, player.InputDirection.y).normalized;

            Vector3 movementDir = Vector3.ProjectOnPlane(inputDir, surfaceNormal);
            player.MoveDirection = movementDir;
            //playerController.Move(movementDir * climbSpeed * Time.deltaTime);
        }

        private void MovePlayer()
        {
            Vector3 forward = player.movementForward.normalized;
            Vector3 right = player.movementRight.normalized;

            Vector3 forwardRelativeInput = player.InputDirection.y * forward;
            Vector3 rightRelativeInput = player.InputDirection.x * right;

            Vector3 planeRelativeMovement = forwardRelativeInput + rightRelativeInput;
            planeRelativeMovement = Vector3.ProjectOnPlane(planeRelativeMovement, player.transform.up).normalized;
            Vector3 movementWithSpeed = planeRelativeMovement * _moveSpeed * Time.deltaTime;
            //player.transform.position += movementWithSpeed;
            //Stutter issue with transform.translate
            //player.transform.Translate(planeRelativeMovement * Time.deltaTime, Space.World);
            player.MoveDirection = movementWithSpeed;

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

        public override void ExitState()
        {
            player.walkingSound.Stop();
        }
    }
}