// using UnityEngine;
//
// namespace Player.Movement.State_Machine
// {
//     public class PlayerMovementStateCrouching : PlayerMovementBaseState
//     {
//         private float _moveSpeed;
//         private RaycastHit _slopeHit;
//
//         public PlayerMovementStateCrouching(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
//             player)
//         {
//         }
//
//         public override void EnterState()
//         {
//             player.crouchSound.Play();
//             player.Rb.useGravity = false;
//             player.movementState = PlayerMovement.MovementState.Crouching;
//
//             _moveSpeed = player.crouchSpeed;
//
//             player.Rb.drag = player.groundDrag;
//
//
//             player.transform.localScale = new Vector3(player.transform.localScale.x, player.crouchYScale,
//                 player.transform.localScale.z);
//             player.Rb.AddForce(-player.playerObj.transform.up * 5f, ForceMode.Impulse);
//         }
//
//         public override void ExitState()
//         {
//             player.uncrouchSound.Play();
//             player.transform.localScale = new Vector3(player.transform.localScale.x, player.StartYScale,
//                 player.transform.localScale.z);
//         }
//
//         public override void UpdateState()
//         {
//             SpeedControl();
//
//             if (!player.Grounded)
//             {
//                 manager.SwitchState(player.FallingState);
//             }
//         }
//
//         public override void FixedUpdateState()
//         {
//             MovePlayer();
//         }
//
//         private void MovePlayer()
//         {
//             if (player.InputDirection != Vector2.zero)
//             {
//                 player.MoveDirection = player.CalculateMoveDirection(player.facingAngles.Item1, player.currentHit);
//                 player.Rb.AddForce(player.MoveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force); // move
//             }
//         }
//
//         private void SpeedControl()
//         {
//             Vector3 flatVel = player.Rb.velocity;
//
//             // limit velocity if needed
//             if (flatVel.magnitude > _moveSpeed)
//             {
//                 Vector3 limitedVel = flatVel.normalized * _moveSpeed;
//                 player.Rb.velocity = limitedVel;
//             }
//         }
//     }
// }