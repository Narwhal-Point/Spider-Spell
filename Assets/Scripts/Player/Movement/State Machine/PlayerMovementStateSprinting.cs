using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSprinting : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;
        private float _moveSpeed;

        public PlayerMovementStateSprinting(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            // player.walkingSound.Play();
            player.audioManager.PlayLoopSFX(player.audioManager.walking);
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Sprinting;

            player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            player.DesiredMoveSpeed = player.sprintSpeed;

            // if (Mathf.Abs(player.DesiredMoveSpeed - player.lastDesiredMoveSpeed) > 4f && player.MoveSpeed != 0)
            //     player.ChangeMomentum(2f);
            // else
            player.MoveSpeed = player.DesiredMoveSpeed;
            

            player.Rb.drag = player.groundDrag;
        }

        public override void UpdateState()
        {
            if (!player.Grounded)
            {
                manager.SwitchState(player.FallingState);
                return;
            }
            if (player.InputDirection == Vector2.zero)
            {
                manager.SwitchState(player.IdleState);
                return;
            }

            SpeedControl();
        }

        public override void FixedUpdateState()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 forward = player.movementForward.normalized;
            Vector3 right = player.movementRight.normalized;

            Vector3 forwardRelativeInput = player.InputDirection.y * forward;
            Vector3 rightRelativeInput = player.InputDirection.x * right;

            Vector3 planeRelativeMovement = forwardRelativeInput + rightRelativeInput;
            planeRelativeMovement = Vector3.ProjectOnPlane(planeRelativeMovement, player.groundHit.normal).normalized;
            Vector3 movementWithSpeed = planeRelativeMovement * player.MoveSpeed * Time.deltaTime;
            player.MoveDirection = movementWithSpeed;

            player.Rb.AddForce(-player.transform.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f), ForceMode.Force);
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
            player.audioManager.StopSFX(player.audioManager.walking);
        }
    }
}