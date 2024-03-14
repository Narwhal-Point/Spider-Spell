using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSwinging : PlayerMovementBaseState
    {
        private float _desiredMoveSpeed;
        private float _moveSpeed;
        
        public PlayerMovementStateSwinging(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
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
            if (!player.Firing)
            {
                StopSwing();
                manager.SwitchState(player.FallingState);
            }
        
            if(player.Swing.Joint != null) // currently swinging
                SwingMovement();
        }

        public override void FixedUpdateState()
        {
        }
    
        private void StartSwing()
        {
            // return if predictionHit not found
            if (player.Swing.predictionHit.point == Vector3.zero) 
                return;

            player.Swing.SwingPoint = player.Swing.predictionHit.point;
            player.Swing.Joint = player.gameObject.AddComponent<SpringJoint>();
            player.Swing.Joint.autoConfigureConnectedAnchor = false;
            player.Swing.Joint.connectedAnchor = player.Swing.SwingPoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, player.Swing.SwingPoint);

            // the distance grapple will try to keep from grapple point. 
            player.Swing.Joint.maxDistance = distanceFromPoint * 0.8f;
            player.Swing.Joint.minDistance = distanceFromPoint * 0.25f;

            player.Swing.Joint.spring = 4.5f;
            player.Swing.Joint.damper = 7f;
            player.Swing.Joint.massScale = 4.5f;

            player.Swing.lr.positionCount = 2;
            player.Swing.CurrentGrapplePosition = player.swingOrigin.position;
        }

        void StopSwing()
        {
            player.Swing.lr.positionCount = 0;
            player.DestroyJoint();
        }
    
        private void SwingMovement()
        {
            if (player.Moving.y > 0.6)
            {
                player.Rb.AddForce(player.orientation.forward * (300f * Time.deltaTime));
            }

            if (player.Moving.y < -0.6)
                player.Rb.AddForce(-player.orientation.forward * (200f * Time.deltaTime));
            
            if (player.Moving.x > 0.6)
            {
                player.Rb.AddForce(player.orientation.right * (200f * Time.deltaTime));
            }

            if (player.Moving.x < -0.6)
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
    }
}