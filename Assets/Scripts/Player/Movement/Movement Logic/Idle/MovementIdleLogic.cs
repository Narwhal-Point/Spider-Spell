using UnityEngine;

namespace Player.Movement.Movement_Logic.Idle
{
    [CreateAssetMenu(fileName = "Movement-Idle", menuName = "Movement Logic/Idle Logic")]
    public class MovementIdleLogic : MovementIdleSOBASE
    {
        private float _moveSpeed;
    
        private RaycastHit _slopeHit;

        public override void DoEnterLogic()
        {
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Idle;
            player.Rb.drag = player.groundDrag;
        }
    
        private bool OnSlope()
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out _slopeHit, player.playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < player.maxSlopeAngle && angle != 0;
            }
            return false;
        }
    }
}
