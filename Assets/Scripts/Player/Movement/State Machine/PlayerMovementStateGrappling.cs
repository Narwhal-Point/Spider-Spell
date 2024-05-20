using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateGrappling : PlayerMovementBaseState
    {
        private readonly PlayerGrappleHandler _grapple;
        private float _startDelay;
        private bool _startedGrappling;

        private float _overshootYAxis = 0f;
        
        
        public PlayerMovementStateGrappling(PlayerMovementStateManager manager, PlayerMovement player, PlayerGrappleHandler grapple) : base(manager, player)
        {
            _grapple = grapple;
        }
        public override void EnterState()
        {
            _startedGrappling = false;
            _startDelay = 0f;
            // player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            // player.DesiredMoveSpeed = player.swingSpeed;
            //
            // // check if there's a big change in desired momentum using magic number.
            // if (Mathf.Abs(player.DesiredMoveSpeed - player.lastDesiredMoveSpeed) > 4f && player.MoveSpeed != 0)
            //     player.ChangeMomentum(4f);
            // else
            //     player.MoveSpeed = player.DesiredMoveSpeed;
            
            player.movementState = PlayerMovement.MovementState.Swinging;
            player.Rb.useGravity = true;
        
            // disable ground drag because otherwise we clamp the y value
            player.Rb.drag = 0f;
            
            StartGrapple();
        }

        public override void ExitState()
        {
            _grapple.IsGrappling = false;
            _grapple.lr.enabled = false;
        }

        public override void UpdateState()
        {
            CheckGrappleDistance();
        }

        private void StartGrapple()
        {
            _grapple.IsGrappling = true;
            _grapple.CurrentGrapplePosition = player.swingOrigin.position;
            _grapple.GrapplePoint = _grapple.predictionHit.point;
            _grapple.lr.enabled = true;

            ExecuteGrapple();
        }
        
        private Vector3 CalculateGrappleVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
        {
            float gravity = Physics.gravity.y;
            float displacementY = endPoint.y - startPoint.y;
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
                                                   + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

            return velocityXZ + velocityY;
        }

        private void GrappleToPosition(Vector3 targetPosition, float trajectoryHeight)
        {
            _startedGrappling = true;
            player.Rb.velocity = CalculateGrappleVelocity(player.transform.position,
                targetPosition, trajectoryHeight);
        }

        private void ExecuteGrapple()
        {
            Vector3 lowestPoint = new Vector3(player.transform.position.x, player.transform.position.y - 1f, player.transform.position.z);

            float grapplePointRelativeYPos = _grapple.GrapplePoint.y - lowestPoint.y;

            if (grapplePointRelativeYPos < 0) 
                grapplePointRelativeYPos = 0f;
            
            GrappleToPosition(_grapple.GrapplePoint, grapplePointRelativeYPos);
        }

        private void CheckGrappleDistance()
        {
            if (Vector3.Distance(player.transform.position, _grapple.GrapplePoint) < 0.01f)
            {
                if(player.Grounded)
                    manager.SwitchState(player.IdleState);
                else
                    manager.SwitchState(player.FallingState);
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
    }
}