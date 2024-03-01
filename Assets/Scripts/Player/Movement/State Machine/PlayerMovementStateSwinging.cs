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
            if (Input.GetKeyUp(player.swingKey))
            {
                StopSwing();
                manager.SwitchState(player.FallingState);
            }
        
            if(player.Swing.joint != null) // currently swinging
                OdmGearMovement();
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
    
        private void OdmGearMovement()
        {
            // right
            if (Input.GetKey(KeyCode.D)) player.Rb.AddForce(player.orientation.right * (player.horizontalThrustForce * Time.deltaTime));
            // left
            if (Input.GetKey(KeyCode.A)) player.Rb.AddForce(-player.orientation.right * (player.horizontalThrustForce * Time.deltaTime));

            // forward
            if (Input.GetKey(KeyCode.W)) player.Rb.AddForce(player.orientation.forward * (player.horizontalThrustForce * Time.deltaTime));

            // shorten cable
            if (Input.GetKey(KeyCode.Space))
            {
                Vector3 directionToPoint = player.Swing.swingPoint - player.transform.position;
                player.Rb.AddForce(directionToPoint.normalized * (player.forwardThrustForce * Time.deltaTime));

                float distanceFromPoint = Vector3.Distance(player.transform.position, player.Swing.swingPoint);

                player.Swing.joint.maxDistance = distanceFromPoint * 0.8f;
                player.Swing.joint.minDistance = distanceFromPoint * 0.25f;
            }
            // extend cable
            if (Input.GetKey(KeyCode.S))
            {
                float extendedDistanceFromPoint = Vector3.Distance(player.transform.position, player.Swing.swingPoint) + player.extendCableSpeed;

                player.Swing.joint.maxDistance = extendedDistanceFromPoint * 0.8f;
                player.Swing.joint.minDistance = extendedDistanceFromPoint * 0.25f;
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