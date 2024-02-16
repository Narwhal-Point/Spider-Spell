using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementStateManager : MonoBehaviour
{
    [Header("Movement")]
        public float walkSpeed = 7;
        public float sprintSpeed = 10;
        public float MoveSpeed { get; set; }
        public float slideSpeed = 30;

        public float DesiredMoveSpeed { get; set; }
        public float LastDesiredMoveSpeed { get; set; }
        
        public float speedIncreaseMultiplier = 1.5f;
        public float slopeIncreaseMultiplier = 2.5f;
    
        public float groundDrag = 5f;

        [Header("Jumping")]
        public float jumpForce = 6;
        public float airMultiplier = 0.001f;
        public float jumpCooldown = 0.5f;
        public bool ReadyToJump { get; set; }

        [Header("Crouching")] 
        public float crouchSpeed = 3.5f;
        public float crouchYScale = 0.5f;
        public float StartYScale { get; set; }
        
        [Header("sliding")] 
        public float maxSlideTime;
        public float slideForce;
        public float SlideTimer { get; set; }
        public bool Sliding { get; set; }

        public float slideYScale;
        
        [Header("Keybinds")] 
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode crouchKey = KeyCode.C;
        public KeyCode slideKey = KeyCode.LeftControl;

        [Header("Ground Check")] 
        public float playerHeight = 2;
        public LayerMask ground;
        public bool Grounded { get; private set; }

        [Header("Slope Handling")] 
        public float maxSlopeAngle;
        public bool ExitingSlope { get; set; }
        
        [Header("References")] 
        public Transform orientation;
        
        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }

        public Vector3 MoveDirection { get; set; }

        public Rigidbody Rb { get; private set; }
        
        // enum to display active state on screen
        [FormerlySerializedAs("state")] public MovementState movementState;
        public enum MovementState
        {
            Idle,
            Walking,
            Sprinting,
            Crouching,
            Sliding,
            Air
        }

        private PlayerMovementBaseState _currentState;
        public PlayerMovementStateSprinting sprintingState = new PlayerMovementStateSprinting();
        public PlayerMovementStateWalking walkingState = new PlayerMovementStateWalking();
        public playerMovementStateCrouching crouchingState = new playerMovementStateCrouching();
        public playerMovementStateJumping jumpingState = new playerMovementStateJumping();
        public PlayerMovementStateSliding slidingState = new PlayerMovementStateSliding();
        public PlayerMovementStateIdle idleState = new PlayerMovementStateIdle();
    
    // Start is called before the first frame update
    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Rb.freezeRotation = true; // stop character from falling over
        ReadyToJump = true;
        StartYScale = transform.localScale.y;

        _currentState = idleState;
        _currentState.EnterState(this);
    }

    // Update is called once per frame
    private void Update()
    {
        Grounded = Physics.Raycast(transform.position, Vector3.down, 
            playerHeight * 0.5f + 0.2f, ground);
        
        // get keyboard input
        HorizontalInput = Input.GetAxisRaw("Horizontal"); // A + D
        VerticalInput = Input.GetAxisRaw("Vertical"); // W + S
        
        _currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        _currentState.FixedUpdateState(this);
    }

    public void SwitchState(PlayerMovementBaseState state)
    {
        _currentState = state;
        state.EnterState(this);
    }

    

}
