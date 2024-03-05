using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateSwinging : PlayerMovementBaseState
    {
        public PlayerMovementStateSwinging(PlayerMovementStateManager manager, PlayerMovement player) : base(manager, player)
        {
        }
        public override void EnterState()
        {
            DesiredMoveSpeed = player.swingSpeed;
            MoveSpeed = DesiredMoveSpeed;
        
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
        
            if(player.Swing.joint != null) // currently swinging
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

            player.Swing.swingPoint = player.Swing.predictionHit.point;
            player.Swing.joint = player.gameObject.AddComponent<SpringJoint>();
            player.Swing.joint.autoConfigureConnectedAnchor = false;
            player.Swing.joint.connectedAnchor = player.Swing.swingPoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, player.Swing.swingPoint);

            // the distance grapple will try to keep from grapple point. 
            player.Swing.joint.maxDistance = distanceFromPoint * 0.8f;
            player.Swing.joint.minDistance = distanceFromPoint * 0.25f;

            // customize values as you like
            player.Swing.joint.spring = 4.5f;
            player.Swing.joint.damper = 7f;
            player.Swing.joint.massScale = 4.5f;

            player.Swing.lr.positionCount = 2;
            player.Swing.currentGrapplePosition = player.swingOrigin.position;
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
                player.Rb.AddForce(player.orientation.forward * (player.horizontalThrustForce * Time.deltaTime));
            }

            if (player.Moving.x > 0.6)
            {
                player.Rb.AddForce(player.orientation.right * (player.horizontalThrustForce * Time.deltaTime));
            }

            if (player.Moving.x < -0.6)
            {
                player.Rb.AddForce(-player.orientation.right * (player.horizontalThrustForce * Time.deltaTime));
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3( player.Rb.velocity.x, 0f,  player.Rb.velocity.z);

            if (flatVel.magnitude > (MoveSpeed * 1.2f))
            {
                Vector3 limitedVel = flatVel.normalized * MoveSpeed;
                player.Rb.velocity = new Vector3(limitedVel.x,  player.Rb.velocity.y, limitedVel.z);
            }
        }
    }
}