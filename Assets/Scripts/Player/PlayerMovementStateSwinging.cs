using UnityEngine;

public class PlayerMovementStateSwinging : PlayerMovementBaseState
{
    // private Vector3 swingPoint;
    // private Vector3 currentGrapplePosition;
    
    [Header("OdmGear")]
    private float horizontalThrustForce = 200f;
    private float forwardThrustForce = 300f;
    private float extendCableSpeed = 20f;
    
    public override void EnterState(PlayerMovementStateManager player)
    {
        player.DesiredMoveSpeed = player.swingSpeed;
        player.MoveSpeed = player.DesiredMoveSpeed;
        
        player.movementState = PlayerMovementStateManager.MovementState.swinging;
        player.Rb.useGravity = true;
        
        // disable ground drag because otherwise we clamp the y value
        // this took hours to figure out...
        player.Rb.drag = 0f;
        
        StartSwing(player);
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {
        SpeedControl(player);
        if (Input.GetKeyUp(player.swingKey))
        {
            StopSwing(player);
            player.SwitchState(player.fallingState);
        }
        
        if(player.swing.joint != null) // currently swinging
            OdmGearMovement(player);
    }

    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
    }
    
    private void StartSwing(PlayerMovementStateManager player)
    {
        // return if predictionHit not found
        if (player.swing.predictionHit.point == Vector3.zero) 
            return;

        player.swing.swingPoint = player.swing.predictionHit.point;
        player.swing.joint = player.play.gameObject.AddComponent<SpringJoint>();
        player.swing.joint.autoConfigureConnectedAnchor = false;
        player.swing.joint.connectedAnchor = player.swing.swingPoint;

        float distanceFromPoint = Vector3.Distance(player.play.position, player.swing.swingPoint);

        // the distance grapple will try to keep from grapple point. 
        player.swing.joint.maxDistance = distanceFromPoint * 0.8f;
        player.swing.joint.minDistance = distanceFromPoint * 0.25f;

        // customize values as you like
        player.swing.joint.spring = 4.5f;
        player.swing.joint.damper = 7f;
        player.swing.joint.massScale = 4.5f;

        player.lr.positionCount = 2;
        player.swing.currentGrapplePosition = player.swingOrigin.position;
    }

    void StopSwing(PlayerMovementStateManager player)
    {
        player.lr.positionCount = 0;
        player.DestroyJoint();
    }
    
    private void OdmGearMovement(PlayerMovementStateManager player)
    {
        // right
        if (Input.GetKey(KeyCode.D)) player.Rb.AddForce(player.orientation.right * horizontalThrustForce * Time.deltaTime);
        // left
        if (Input.GetKey(KeyCode.A)) player.Rb.AddForce(-player.orientation.right * horizontalThrustForce * Time.deltaTime);

        // forward
        if (Input.GetKey(KeyCode.W)) player.Rb.AddForce(player.orientation.forward * horizontalThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = player.swing.swingPoint - player.transform.position;
            player.Rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(player.transform.position, player.swing.swingPoint);

            player.swing.joint.maxDistance = distanceFromPoint * 0.8f;
            player.swing.joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(player.transform.position, player.swing.swingPoint) + extendCableSpeed;

            player.swing.joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            player.swing.joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    private void SpeedControl(PlayerMovementStateManager player)
    {
        Vector3 flatVel = new Vector3( player.Rb.velocity.x, 0f,  player.Rb.velocity.z);

        if (flatVel.magnitude > (player.MoveSpeed * 1.2f))
        {
            Vector3 limitedVel = flatVel.normalized * player.MoveSpeed;
            player.Rb.velocity = new Vector3(limitedVel.x,  player.Rb.velocity.y, limitedVel.z);
        }
    }
    
}