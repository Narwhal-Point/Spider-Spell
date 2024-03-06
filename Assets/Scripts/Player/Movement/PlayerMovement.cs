using Player.Movement.Movement_Logic.Idle;
using Player.Movement.State_Machine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] 

        public float swingSpeed = 20;
        public float slideSpeed = 30;

        public float groundDrag = 5f;

        [Header("Jumping")] 
        public float jumpForce = 10;
        public float airMultiplier = 0.001f;
        public float jumpCooldown = 0.5f;

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

        [Header("Swinging")] public KeyCode swingKey = KeyCode.Mouse0;
        public float horizontalThrustForce = 200f;
        public float forwardThrustForce = 300f;
        public float extendCableSpeed = 20f;
        public PlayerSwinging Swing { get; private set; }

        [Header("References")] public Transform orientation;
        public Transform swingOrigin;
        public Transform playerObj;
        public Vector3 MoveDirection { get; set; }
        public Rigidbody Rb { get; private set; }
        public float StartYScale { get; private set; } // default height of character

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
        
        public Quaternion targetRotation { get; private set; }

        #region Player Movement States
        
        private PlayerMovementStateManager _manager;
        public PlayerMovementStateIdle IdleState { get; private set; }
        public PlayerMovementStateWalking WalkingState { get; private set; }
        public PlayerMovementStateSprinting SprintingState { get; private set; }
        public PlayerMovementStateCrouching CrouchingState { get; private set; }
        public PlayerMovementStateJumping JumpingState { get; private set; }
        public PlayerMovementStateFalling FallingState { get; private set; }
        public PlayerMovementStateSliding SlidingState { get; private set; }
        public PlayerMovementStateSwinging SwingingState { get; private set; }
        
        #endregion


        [FormerlySerializedAs("MovementIdleBase")]
        [Header("Scriptable Objects")]
        #region Scriptable objcts Variables

        [SerializeField] private MovementIdleSOBASE movementIdleBase;
        [SerializeField] private MovementWalkingSOBASE movementWalkingBase;
        [SerializeField] private MovementSprintingSOBASE movementSprintingBase;
        [SerializeField] private MovementCrouchingSOBASE movementCrouchingBase;

        public MovementIdleSOBASE MovementIdleBaseInstance { get; set; }
        public MovementWalkingSOBASE MovementWalkingBaseInstace { get; set; }
        public MovementSprintingSOBASE MovementSprintingBaseInstance { get; set; }
        public MovementCrouchingSOBASE MovementCrouchingBaseInstance { get; set; }
        
        
        #endregion

        private void Awake()
        {
            MovementIdleBaseInstance = Instantiate(movementIdleBase);
            MovementWalkingBaseInstace = Instantiate(movementWalkingBase);
            MovementSprintingBaseInstance = Instantiate(movementSprintingBase);
            MovementCrouchingBaseInstance = Instantiate(movementCrouchingBase);
            
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

            MovementIdleBaseInstance.Initialize(gameObject, this);
            MovementWalkingBaseInstace.Initialize(gameObject, this);
            MovementSprintingBaseInstance.Initialize(gameObject, this);
            MovementCrouchingBaseInstance.Initialize(gameObject, this);
            
            _manager.Initialize(IdleState);
        }

        private void Update()
        {
            // print the current movement state on the screen
            text.text = movementState.ToString();
            
            // check if player is on the ground
            Grounded = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down),
                playerHeight * 0.5f + 0.2f, ground);
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 5, Color.magenta);

            // transform.up = groundHit.normal;

            Debug.DrawRay(transform.position, transform.up * 5, Color.blue);

            WallCheck();
            
            _manager.CurrentState.UpdateState();
        }

        private void FixedUpdate()
        {
            _manager.CurrentState.FixedUpdateState();
        }
        private void WallCheck()
        {
            // wall check
            RaycastHit frontWallHit;
            bool wallInFront = Physics.Raycast(transform.position, playerObj.forward,
                out frontWallHit,
                (playerHeight * 0.5f + 0.2f), ground);
            
            bool wallInBack = Physics.Raycast(transform.position, -playerObj.forward, out var backWallHit, (playerHeight * 0.5f), ground);
            
            // written with the help of google gemini. https://g.co/gemini/share/8d280f3a447f
            if (wallInFront)
            {
                Debug.DrawRay(transform.position,
                    playerObj.forward * (playerHeight * 0.5f + 0.2f + 10f), Color.green);
                
                // Project the wall normal onto the xz-plane
                Vector3 projectedNormal = Vector3.ProjectOnPlane(frontWallHit.normal, Vector3.up);

                // Calculate a target rotation based on the projected normal:
                targetRotation = Quaternion.FromToRotation(transform.up, projectedNormal);

                // Rotate the player towards the wall (with optional smoothing)
                transform.rotation = targetRotation; //Quaternion.Slerp(transform.rotation, targetRotation, 20 * Time.deltaTime);
            }
            else if (wallInBack)
            {
                // Project the wall normal onto the xz-plane
                Vector3 projectedNormal = Vector3.ProjectOnPlane(backWallHit.normal, Vector3.up);

                // Calculate a target rotation based on the projected normal:
                targetRotation = Quaternion.FromToRotation(transform.up, projectedNormal);

                // Rotate the player towards the wall (with optional smoothing)
                transform.rotation = targetRotation; //Quaternion.Slerp(transform.rotation, targetRotation, 20 * Time.deltaTime);
            }
            else
            {
                Debug.DrawRay(transform.position,
                    playerObj.forward * (playerHeight * 0.5f + 0.2f + 10f), Color.red);
            }
        }
        
        // is this stupid? Yes.
        // does it work? Also Yes.
        public void DestroyJoint()
        {
            Destroy(Swing.joint);
        }
        
                // input callbacks
        public void OnMove(InputValue value)
        {
            Moving = value.Get<Vector2>();
        }

        public void OnJump()
        {
            if (_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState
                                                   || _manager.CurrentState == SprintingState)
            {
                _manager.SwitchState(JumpingState);
            }
        }

        public void OnSprint(InputValue value)
        {
            Sprinting = value.isPressed;

            if (_manager.CurrentState == WalkingState)
            {
                _manager.SwitchState(SprintingState);
            }
            else if (_manager.CurrentState == SprintingState)
            {
                _manager.SwitchState(WalkingState);
            }
        }

        public void OnFire(InputValue value)
        {
            Firing = value.isPressed;
        }

        public void OnCrouch(InputValue value)
        {
            if (_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState)
                _manager.SwitchState(CrouchingState);
            else if (_manager.CurrentState == CrouchingState)
            {
                if (Moving != Vector2.zero)
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

        public void OnSlide(InputValue value)
        {
            Sliding = value.isPressed;
            if (_manager.CurrentState == WalkingState || _manager.CurrentState == SprintingState
                                                      || (Grounded && _manager.CurrentState == JumpingState)
                                                      || Grounded && _manager.CurrentState == FallingState)
                _manager.SwitchState(SlidingState);
        }


        // function needed to stop sliding when slide button is released
        private void OnSlideRelease()
        {
            // only switch states if released while in sliding state
            if (_manager.CurrentState == SlidingState)
            {
                if (Grounded && Moving == Vector2.zero)
                    _manager.SwitchState(IdleState);
                else if (Moving != Vector2.zero)
                {
                    if (Sprinting)
                        _manager.SwitchState(SprintingState);
                    else
                        _manager.SwitchState(WalkingState);
                }
            }
        }
    }
}