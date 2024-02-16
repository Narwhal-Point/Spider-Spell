using UnityEngine;

public class WitchSearchingState : WitchBaseState
{
    private float _timer = 10f;
    private WitchMovement move;
    
    public override void EnterState(WitchStateManager witch)
    {
        Debug.Log("Hello from Searching state");
        move = witch.GetComponent<WitchMovement>();
    }

    public override void UpdateState(WitchStateManager witch)
    {
        move.moving = true;
        witch.transform.Rotate(Vector3.up, witch.idleRotationSpeed * 10f * Time.deltaTime);

        if (!witch.playerDetection.canSeePlayer)
        {
            if (_timer <= 0f)
            {
                _timer = witch.searchingTime;
                move.moving = false;
                witch.SwitchState(witch.IdleState);
            }
            else
            {
                _timer -= Time.deltaTime;
                // Debug.Log("Countdown: " + _timer);
            }
        }
        else
        {
            move.moving = false;
            _timer = witch.searchingTime;
            witch.SwitchState(witch.ChasingState);
        }

    }

    public override void onCollisionEnter(WitchStateManager witch)
    {
        
    }
}
