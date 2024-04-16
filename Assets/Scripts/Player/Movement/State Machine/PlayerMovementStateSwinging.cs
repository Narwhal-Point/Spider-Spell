using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSwinging : PlayerMovementBaseState
    {
        private float _desiredMoveSpeed;
        private float _moveSpeed;
        private readonly PlayerSwingHandler _swing;
        
        public PlayerMovementStateSwinging(PlayerMovementStateManager manager, PlayerMovement player, PlayerSwingHandler swing) : base(manager, player)
        {
            _swing = swing;
        }
        public override void EnterState()
        {
            _desiredMoveSpeed = player.swingSpeed;
            _moveSpeed = _desiredMoveSpeed;
        
            player.movementState = PlayerMovement.MovementState.Swinging;
            player.Rb.useGravity = true;
        
            // disable ground drag because otherwise we clamp the y value
            // this took hours to figure out...
            player.Rb.drag = 0f;
        
            StartSwing();
        }

        public override void UpdateState()
        {
            SpeedControl();
            if (!player.IsFiring)
            {
                // StopSwing();
                manager.SwitchState(player.FallingState);
            }
        
            if(_swing.Joint != null) // currently swinging
                SwingMovement();
        }
    
        private void StartSwing()
        {
            // return if predictionHit not found
            if (_swing.predictionHit.point == Vector3.zero) 
                return;

            
            _swing.SwingPoint = _swing.predictionHit.point;
            _swing.Joint = player.gameObject.AddComponent<SpringJoint>();
            _swing.Joint.autoConfigureConnectedAnchor = false;
            _swing.Joint.connectedAnchor = _swing.SwingPoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, _swing.SwingPoint);

            // the distance grapple will try to keep from grapple point. 
            float distance = Mathf.Min(distanceFromPoint * 0.8f, _swing.maxSwingDistance);
            
            _swing.Joint.minDistance = distance;

            _swing.Joint.spring = 4.5f;
            _swing.Joint.damper = 7f;
            _swing.Joint.massScale = 4.5f;

            _swing.lr.positionCount = 2;
            _swing.CurrentGrapplePosition = player.swingOrigin.position;
            player.webShootSound.Play();
            player.midAirSound.Play();
        }

        void StopSwing()
        {
            _swing.lr.positionCount = 0;
            _swing.DestroyJoint();
        }
    
        private void SwingMovement()
        {
            if (player.InputDirection.y > 0.6)
            {
                player.Rb.AddForce(player.orientation.forward * (300f * Time.deltaTime));
            }

            if (player.InputDirection.y < -0.6)
                player.Rb.AddForce(-player.orientation.forward * (200f * Time.deltaTime));
            
            if (player.InputDirection.x > 0.6)
            {
                player.Rb.AddForce(player.orientation.right * (200f * Time.deltaTime));
            }

            if (player.InputDirection.x < -0.6)
            {
                player.Rb.AddForce(-player.orientation.right * (300f * Time.deltaTime));
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3( player.Rb.velocity.x, 0f,  player.Rb.velocity.z);

            if (flatVel.magnitude > (_moveSpeed * 1.2f))
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                player.Rb.velocity = new Vector3(limitedVel.x,  player.Rb.velocity.y, limitedVel.z);
            }
        }

        public override void ExitState()
        {
            StopSwing();
            player.midAirSound.Stop();
        }
    }
}