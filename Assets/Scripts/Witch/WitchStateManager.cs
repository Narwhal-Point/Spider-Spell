using UnityEngine;

public class WitchStateManager : MonoBehaviour
{
    private WitchBaseState _currentState;
    public WitchIdleState IdleState = new WitchIdleState();
    public WitchChasingState ChasingState = new WitchChasingState();
    public WitchSearchingState SearchingState = new WitchSearchingState();
    public Transform orientation;
    
    public WitchFov playerDetection;
    
    [Header("Idle Config")]
    public float idleRotationSpeed = 30f;

    [Header("Searching Config")]
    // time witch takes to search after losing player in seconds
    public float searchingTime = 10f;
    
    
    private void Start()
    {
        _currentState = IdleState;
        _currentState.EnterState(this);
        playerDetection = GetComponent<WitchFov>();
    }

    private void Update()
    {
        _currentState.UpdateState(this);
    }

    public void SwitchState(WitchBaseState state)
    {
        _currentState = state;
        state.EnterState(this);
    }
}
