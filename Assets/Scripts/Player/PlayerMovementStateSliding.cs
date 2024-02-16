using UnityEngine;

public class PlayerMovementStateSliding : PlayerMovementBaseState
{
    private RaycastHit _slopeHit;
    
    public override void EnterState(PlayerMovementStateManager player)
    {
        StartSlide(player);
        player.movementState = PlayerMovementStateManager.MovementState.Sliding;
        player.Rb.drag = player.groundDrag;
    }

    public override void UpdateState(PlayerMovementStateManager player)
    {

        if (Input.GetKeyUp(player.slideKey) && player.Sliding)
        {
            StopSlide(player);
        }
    }
    
    public override void FixedUpdateState(PlayerMovementStateManager player)
    {
        SlidingMovement(player);
    }
    
    private void StartSlide(PlayerMovementStateManager player)
    {
        player.Sliding = true;

        player.transform.localScale = new Vector3(player.transform.localScale.x, player.slideYScale, player.transform.localScale.z);
        player.Rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        player.SlideTimer = player.maxSlideTime;

    }
    
    private void StopSlide(PlayerMovementStateManager player)
    {
        player.Sliding = false;
        player.transform.localScale = new Vector3(player.transform.localScale.x, player.StartYScale, player.transform.localScale.z);
        
        if(player.Grounded && player.HorizontalInput == 0 && player.VerticalInput == 0 && player.Rb.velocity.magnitude < 0.1f)
            player.SwitchState(player.idleState);
        else if (player.HorizontalInput != 0 || player.VerticalInput != 0)
        {
            if (Input.GetKeyDown(player.sprintKey))
                player.SwitchState(player.sprintingState);
            else
                player.SwitchState(player.walkingState);
        }
    }

    private void SlidingMovement(PlayerMovementStateManager player)
    {
        // sliding normally
        Vector3 inputDirection = player.orientation.forward * player.VerticalInput + player.orientation.right * player.HorizontalInput;

        if (!OnSlope(player) || player.Rb.velocity.y > -0.1f)
        {
            player.Rb.AddForce(inputDirection.normalized * player.slideForce, ForceMode.Force);

            player.SlideTimer -= Time.deltaTime;
        }
        else // sliding down a slope
        {
            player.Rb.AddForce(GetSlopeMoveDirection(inputDirection) * player.slideForce, ForceMode.Force);
        }

        if(player.SlideTimer <= 0)
            StopSlide(player);
    }
    
    private bool OnSlope(PlayerMovementStateManager player)
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
