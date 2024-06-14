using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateWalking : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;

        public PlayerMovementStateWalking(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            // player.walkingSound.Play();
            player.audioManager.PlayLoopSFX(player.audioManager.walking);
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Walking;

            player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            player.DesiredMoveSpeed = player.walkSpeed;

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
            player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.groundHit);

            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f * player.InputDirection.magnitude), ForceMode.Force);
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