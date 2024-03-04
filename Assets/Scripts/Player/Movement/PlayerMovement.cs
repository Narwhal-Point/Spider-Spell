using Player.Movement.State_Machine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] 
        public float walkSpeed = 7;
        public float sprintSpeed = 10;
        public float swingSpeed = 20;
        public float slideSpeed = 30;

        public float speedIncreaseMultiplier = 1.5f;
        public float slopeIncreaseMultiplier = 2.5f;

        public float groundDrag = 5f;

        [Header("Jumping")] 
        public float jumpForce = 10;
        public float airMultiplier = 0.001f;
        public float jumpCooldown = 0.5f;

        [Header("Crouching")] 
        public float crouchSpeed = 3.5f;
        public float crouchYScale = 0.5f;
        public float StartYScale { get; private set; }

        [Header("sliding")] 
        public float maxSlideTime = 0.75f;
        public float slideForce = 150f;

        public float slideYScale = 0.5f;

        [Header("Ground Check")] 
        public float playerHeight = 2;
        public LayerMask ground;
        public bool Grounded { get; private set; }

        [Header("Slope Handling")] 
        public float maxSlopeAngle;
        public bool ExitingSlope { get; set; }

        [Header("Swinging")] 
        public KeyCode swingKey = KeyCode.Mouse0;
        public float horizontalThrustForce = 200f;
        public float forwardThrustForce = 300f;
        public float extendCableSpeed = 20f;
        public PlayerSwinging Swing { get; private set; }

        [Header("References")] 
        public Transform orientation, swingOrigin;

        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }
        public Vector3 MoveDirection { get; set; }

        public Rigidbody Rb { get; private set; }

        public AudioSource crouchSound;
        public AudioSource uncrouchSound;
    
        public TMP_Text text;

        public Vector2 Moving { get; private set; }
        public bool Sprinting { get; private set; }
        public bool Firing { get; private set; }
        
        public bool Sliding { get; private set; }
        
        public bool Crouching { get; private set; }
        
        // enum to display active state on screen
        public MovementState movementState;
        public enum MovementState
        {
            Idle,
            Walking,
            Sprinting,
            Crouching,
            Sliding,
            Jumping,
            Falling,
            Swinging
        }

        private PlayerMovementStateManager _manager;
        public PlayerMovementStateIdle IdleState { get; private set; }
        public PlayerMovementStateWalking WalkingState { get; private set; }
        public PlayerMovementStateSprinting SprintingState { get; private set; }
        public PlayerMovementStateCrouching CrouchingState { get; private set; }
        public PlayerMovementStateJumping JumpingState { get; private set; }
        public PlayerMovementStateFalling FallingState { get; private set; }
        public PlayerMovementStateSliding SlidingState { get; private set; }
        public PlayerMovementStateSwinging SwingingState { get; private set; }

        private void Awake()
        {
            _manager = new PlayerMovementStateManager();
        
            IdleState = new PlayerMovementStateIdle(_manager, this);
            WalkingState = new PlayerMovementStateWalking(_manager, this);
            SprintingState = new PlayerMovementStateSprinting(_manager, this);
            CrouchingState = new PlayerMovementStateCrouching(_manager, this);
            JumpingState = new PlayerMovementStateJumping(_manager, this);
            FallingState = new PlayerMovementStateFalling(_manager, this);
            SlidingState = new PlayerMovementStateSliding(_manager, this);
            SwingingState = new PlayerMovementStateSwinging(_manager, this);
        }

        private void Start()
        {
            Swing = GetComponent<PlayerSwinging>();
            Rb = GetComponent<Rigidbody>();
            Rb.freezeRotation = true; // stop character from falling over
            StartYScale = transform.localScale.y;

            _manager.Initialize(IdleState);
        }

        private void Update()
        {
            // print the current movement state on the screen
            text.text = movementState.ToString();
        
            // check if player is on the ground
            Grounded = Physics.Raycast(transform.position, Vector3.down,
                playerHeight * 0.5f + 0.2f, ground);
        
            _manager.CurrentState.UpdateState();
        }

        private void FixedUpdate()
        {
            _manager.CurrentState.FixedUpdateState();
        }
    
        // is this stupid? Yes.
        // Do I care? No.
        public void DestroyJoint()
        {
            Destroy(Swing.joint);
        }
        // input callbacks
        private void OnMove(InputValue value)
        {
            Moving = value.Get<Vector2>();
        }

        private void OnJump()
        {
            if (_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState
                                                   || _manager.CurrentState == SprintingState)
            {
                _manager.SwitchState(JumpingState);
            }
        }

        private void OnSprint(InputValue value)
        {
            Sprinting = value.isPressed;

            if(_manager.CurrentState == WalkingState)
                _manager.SwitchState(SprintingState);
            else if (_manager.CurrentState == SprintingState)
            {
                _manager.SwitchState(WalkingState);
            }
        }

        private void OnFire(InputValue value)
        {
            Firing = value.isPressed;
        }

        private void OnCrouch(InputValue value)
        {
            if(_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState)
                _manager.SwitchState(CrouchingState);
            else if(_manager.CurrentState == CrouchingState)
            {
                if(Moving != Vector2.zero)
                {
                    _manager.SwitchState(WalkingState);
                }
                else
                {
                    _manager.SwitchState(IdleState);
                }
            }

            Crouching = value.isPressed;
        }

        private void OnSlide(InputValue value)
        {
            Sliding = value.isPressed;
            if(_manager.CurrentState == WalkingState || _manager.CurrentState == SprintingState 
                                                     || (Grounded && _manager.CurrentState == JumpingState) 
                                                     || Grounded && _manager.CurrentState == FallingState)
            _manager.SwitchState(SlidingState);
            else if (_manager.CurrentState == SlidingState)
            {
                if(Grounded && Moving == Vector2.zero)
                    _manager.SwitchState(IdleState);
                else if (Moving != Vector2.zero)
                {
                    if(Sprinting)
                        _manager.SwitchState(SprintingState);
                    else
                        _manager.SwitchState(WalkingState);
                }
            }
        }


    }
}