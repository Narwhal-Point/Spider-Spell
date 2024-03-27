using Player.Movement.State_Machine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] public float walkSpeed = 7;
        public float sprintSpeed = 10f;
        public float swingSpeed = 20;
        public float crouchSpeed = 3.5f;
        public float groundDrag = 5f;

        [Header("Crouching")] public float crouchYScale = 0.5f;

        [Header("Jumping")] public float jumpForce = 10;
        public float airMultiplier = 0.001f;
        public float jumpCooldown = 0.5f;

        [Header("sliding")] public float maxSlideTime = 0.75f;
        public float slideForce = 150f;

        public float slideYScale = 0.5f;

        [Header("Ground Check")] public float playerHeight = 2;
        public LayerMask ground;
        public bool Grounded { get; private set; }

        [Header("Slope Handling")] public float maxSlopeAngle;
        public bool ExitingSlope { get; set; }

        [Header("References")] public Transform orientation;
        public Transform swingOrigin;
        public Transform playerObj;
        public PlayerCam camScript;

        private PlayerSwingHandler Swing { get; set; }
        public Vector3 MoveDirection { get; set; }
        public Rigidbody Rb { get; private set; }
        public float StartYScale { get; private set; } // default height of character

        public AudioSource crouchSound;
        public AudioSource uncrouchSound;

        public TMP_Text text;

        // input booleans
        public Vector2 Moving { get; private set; }
        public bool Sprinting { get; private set; }
        public bool Firing { get; private set; }

        public bool Sliding { get; private set; }

        public bool Crouching { get; private set; }

        public bool Aiming { get; private set; }

        [FormerlySerializedAs("_turnSmoothTime")] [Header("turning")]

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

        #region wallclimbing and rotation

        public bool wallInFront { get; private set; }
        public RaycastHit currentHit;
        public (float, float) facingAngles;

        #endregion

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
            SwingingState = new PlayerMovementStateSwinging(_manager, this, GetComponent<PlayerSwingHandler>());
        }

        private void Start()
        {
            Swing = GetComponent<PlayerSwingHandler>();
            Rb = GetComponent<Rigidbody>();
            Rb.freezeRotation = true; // stop character from falling over
            StartYScale = transform.localScale.y;

            _manager.Initialize(IdleState);
        }

        private void Update()
        {
            Debug.Log(Rb.velocity);
            // print the current movement state on the screen
            text.text = movementState.ToString();

            // raycasts to check if a surface has been hit
            SurfaceCheck();
            
            // state update
            _manager.CurrentState.UpdateState();
        }

        private void FixedUpdate()
        {
            _manager.CurrentState.FixedUpdateState();
        }


        private void SurfaceCheck() // written with the help of google gemini. https://g.co/gemini/share/8d280f3a447f
        {
            // check if player is on the ground
            Grounded = Physics.Raycast(transform.position, playerObj.TransformDirection(Vector3.down), out var groundHit,
                playerHeight * 0.5f + 0.2f, ground);
            if (!Grounded)
                Debug.DrawRay(transform.position, playerObj.TransformDirection(Vector3.down) * (playerHeight* 0.5f + 0.2f), Color.red);
            else
                Debug.DrawRay(transform.position, playerObj.TransformDirection(Vector3.down) * (playerHeight* 0.5f + 0.2f), Color.green);
            
            // wall check
            wallInFront = Physics.Raycast(transform.position, playerObj.forward,
                out var frontWallHit, (playerHeight * 0.5f + 0.2f), ground);

            if (wallInFront)
            {
                currentHit = frontWallHit;
                Debug.DrawRay(transform.position,
                    playerObj.forward * (playerHeight * 0.5f + 0.2f), Color.green);
            }
            else
            {
                currentHit = groundHit;
                Debug.DrawRay(transform.position,
                    playerObj.forward * (playerHeight * 0.5f + 0.2f), Color.red);
            }
        }

        public Vector3 CalculateMoveDirection(float angle, RaycastHit hit)
        {
            Quaternion facingRotation = Quaternion.Euler(0f, angle, 0f);
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion combinedRotation = surfaceRotation * facingRotation;
            Vector3 moveDirection = combinedRotation * Vector3.forward;
            return moveDirection;
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

            // might be better to handle this in the current state using a special function for checking if a state switch is logical

            if (_manager.CurrentState == WalkingState && Sprinting)
            {
                _manager.SwitchState(SprintingState);
            }
            else if (_manager.CurrentState == SprintingState && !Sprinting)
            {
                _manager.SwitchState(WalkingState);
            }
        }

        public void OnFire(InputValue value)
        {
            Firing = value.isPressed;

            if (_manager.CurrentState == IdleState && camScript.CurrentCamera == PlayerCam.CameraStyle.Aiming)
                _manager.SwitchState(SwingingState);
        }

        public void OnCrouch(InputValue value)
        {
            Crouching = value.isPressed;

            if (Crouching && (_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState))
                _manager.SwitchState(CrouchingState);
            else if (!Crouching && _manager.CurrentState == CrouchingState)
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
        }

        public void OnSlide(InputValue value)
        {
            Sliding = value.isPressed;
            if (Sliding && (_manager.CurrentState == WalkingState || _manager.CurrentState == SprintingState
                                                                  || (Grounded && _manager.CurrentState == JumpingState)
                                                                  || Grounded && _manager.CurrentState == FallingState))
                _manager.SwitchState(SlidingState);
            else if (!Sliding && _manager.CurrentState == SlidingState)
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

        public void OnAim(InputValue value)
        {
            Aiming = value.isPressed;
        }
    }
}