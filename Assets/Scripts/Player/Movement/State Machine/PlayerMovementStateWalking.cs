using UnityEngine;
using UnityEngine.InputSystem;

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
            player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.groundHit);

            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
            int distance = 2;            
            RaycastHit hit;
            bool raycastHit = Physics.Raycast(player.transform.position, -player.playerObj.up, out hit, distance);
/*
            // Draw the raycast for visualization
            Debug.DrawRay(player.transform.position, player.MoveDirection * distance, Color.green);
            if (raycastHit && hit.collider.CompareTag("Wall"))
            {
                Debug.Log("on wall");
                // If hitting a wall, move along the wall's surface
                Vector3 wallNormal = hit.normal;
                Vector3 inputDirection = new Vector3(player.InputDirection.x, 0f, player.InputDirection.y).normalized;
                Vector3 moveDirection = Vector3.Cross(wallNormal, inputDirection).normalized;
                player.Rb.AddForce(inputDirection * (_moveSpeed * 10f), ForceMode.Force);
                return;
            }
            else
            {
                Debug.Log("on ground");
                player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.groundHit);
                // Otherwise, move as usual
                player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
                player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
            }*/
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