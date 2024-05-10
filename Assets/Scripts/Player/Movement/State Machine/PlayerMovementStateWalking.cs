using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateWalking : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;
        private Vector3 surfaceNormal;
        public PlayerMovementStateWalking(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.walkingSound.Play();
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Walking;

            player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            player.DesiredMoveSpeed = player.walkSpeed;

            if (Mathf.Abs(player.DesiredMoveSpeed - player.lastDesiredMoveSpeed) > 4f && player.MoveSpeed != 0)
                player.ChangeMomentum(2f);
            else
                player.MoveSpeed = player.DesiredMoveSpeed;
            

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
            //HandleVectorRotation();
            if (direction.magnitude >= 0.1f)
            {
                MovePlayer();
                TurnPlayer();
            }
        }

        private void MovePlayer()
        {
            Vector3 forward = player.movementForward.normalized;
            Vector3 right = player.movementRight.normalized;

            Vector3 forwardRelativeInput = player.InputDirection.y * forward;
            Vector3 rightRelativeInput = player.InputDirection.x * right;

            Vector3 planeRelativeMovement = forwardRelativeInput + rightRelativeInput;
            planeRelativeMovement = Vector3.ProjectOnPlane(planeRelativeMovement, surfaceNormal).normalized;
            Vector3 movementWithSpeed = planeRelativeMovement * player.MoveSpeed * Time.deltaTime;
            player.MoveDirection = movementWithSpeed;

            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f), ForceMode.Force);
            /*player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.groundHit);

            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f), ForceMode.Force);*/
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


        private void SpeedControl()
        {
            Vector3 flatVel = player.Rb.velocity;

            // limit velocity if needed
            if (flatVel.magnitude > player.MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * player.MoveSpeed;
                player.Rb.velocity = limitedVel;
            }
        }

        public override void ExitState()
        {
            player.walkingSound.Stop();
        }
    }
}