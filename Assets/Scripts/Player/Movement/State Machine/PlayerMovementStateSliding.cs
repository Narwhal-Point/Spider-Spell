using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSliding : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;
    
        private float SlideTimer { get; set; }
        private bool Sliding { get; set; }
    
        public PlayerMovementStateSliding(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
        public override void EnterState()
        {
            player.Rb.useGravity = false;
            StartSlide();
            player.movementState = PlayerMovement.MovementState.Sliding;
            player.Rb.drag = player.groundDrag;
        }

        public override void ExitState()
        {
            StopSlide();
        }
    
        public override void FixedUpdateState()
        {
            SlidingMovement();
        }
    
        private void StartSlide()
        {
            Sliding = true;

            player.transform.localScale = new Vector3(player.transform.localScale.x, player.slideYScale, player.transform.localScale.z);
            player.Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            SlideTimer = player.maxSlideTime;

        }
    
        private void StopSlide()
        {
            Sliding = false;
            player.transform.localScale = new Vector3(player.transform.localScale.x, player.StartYScale, player.transform.localScale.z);
            
        }

        private void SlidingMovement()
        {
            // sliding normally
            Vector3 inputDirection = player.orientation.forward * player.Moving.y +
                                   player.orientation.right * player.Moving.x;

            if (!OnSlope() || player.Rb.velocity.y > -0.1f)
            {
                player.Rb.AddForce(inputDirection.normalized * player.slideForce, ForceMode.Force);

                SlideTimer -= Time.deltaTime;
            }
            else // sliding down a slope
            {
                player.Rb.AddForce(GetSlopeMoveDirection(inputDirection) * player.slideForce, ForceMode.Force);
            }

            if(SlideTimer <= 0)
                StopSlide();
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

        private Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
        }
    }
}
