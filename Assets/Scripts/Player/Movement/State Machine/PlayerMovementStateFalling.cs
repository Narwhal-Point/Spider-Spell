﻿using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateFalling : PlayerMovementBaseState
    {
        public PlayerMovementStateFalling(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.movementState = PlayerMovement.MovementState.Falling;
            player.Rb.useGravity = true;

            // disable ground drag because otherwise we clamp the y value
            // this took hours to figure out...
            player.Rb.drag = 0f;
            
            player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            // using swingSpeed for now.
            player.DesiredMoveSpeed = player.swingSpeed;
            
            if (Mathf.Abs(player.DesiredMoveSpeed - player.lastDesiredMoveSpeed) > 4f && player.MoveSpeed != 0)
                player.ChangeMomentum(4f);
            else
                player.MoveSpeed = player.DesiredMoveSpeed;
        }

        public override void UpdateState()
        {
            if(player.IsFiring && player.camScript.CurrentCamera == PlayerCam.CameraStyle.Aiming) // start swinging
                manager.SwitchState(player.SwingingState);
            else if (player.Grounded)
            {
                if (player.InputDirection != Vector2.zero)
                {
                    if (player.IsSliding)
                        manager.SwitchState(player.SlidingState);
                    // else if (player.IsSprinting)
                    //     manager.SwitchState(player.SprintingState);
                    else
                        manager.SwitchState(player.WalkingState);
                }
                else
                    manager.SwitchState(player.IdleState);
            }
        }

        public override void ExitState()
        {
            if (player.Grounded)
            {
                player.landingSound.Play();
                player.PlayLandVFX();
            }
        }
    }
}