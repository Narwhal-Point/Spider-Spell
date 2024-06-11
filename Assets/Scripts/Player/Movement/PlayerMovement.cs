using Audio;
using Player.Movement.State_Machine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour, IDataPersistence
    {
        #region EditorValuesAndReferences

        [Header("Movement")] public float walkSpeed = 10f;
        public float sprintSpeed = 10f;
        public float swingSpeed = 20;
        public float crouchSpeed = 3.5f;
        public float groundDrag = 5f;
        public float airSpeed = 6f;
        [SerializeField] private float rotationSpeed = 10f;

        public float DesiredMoveSpeed { get; set; } = 10;
        public float lastDesiredMoveSpeed { get; set; } = 10;
        public float MoveSpeed { get; set; } = 10;

        [Header("Crouching")] public float crouchYScale = 0.5f;

        [Header("Jumping")] public float jumpForce = 10;
        public float airMultiplier = 0.001f;
        public float jumpCooldown = 0.5f;

        [Header("sliding")] public float maxSlideTime = 0.75f;
        public float slideForce = 150f;

        public float slideYScale = 0.5f;

        [Header("Dashing")] [SerializeField] private float dashDuration = 0.25f;
        [SerializeField] private float dashForce = 20f;
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private float DashUpwardForce = 5f;
        public bool IsDashing { get; set; }

        [Header("Ground Check")] public float playerHeight = 2;
        public LayerMask ground;
        public bool Grounded { get; private set; }
        public bool EdgeFound { get; private set; }

        [Header("Slope Handling")] public float maxSlopeAngle;

        [Header("References")] public Transform orientation;
        public Transform swingOrigin;
        public Transform playerObj;
        public Transform cam;
        public PlayerCam camScript;
        private PlayerSwingHandler _swing;

        public Vector3 MoveDirection { get; set; }
        public Rigidbody Rb { get; private set; }
        private Collider _collider;
        public float StartYScale { get; private set; } // default height of character

        // public AudioSource crouchSound;
        // public AudioSource uncrouchSound;

        // sfx for spider
        // public AudioSource webShootSound;
        // public AudioSource landingSound;
        // public AudioSource walkingSound;
        // public AudioSource jumpingSound;

        public AudioManager audioManager;
        // public AudioSource midAirSound;
        // public bool jumpAnimation;


        public TMP_Text text;
        public TMP_Text speed_text;

        [SerializeField] private GameObject dustVFX;

        // input booleans
        public Vector2 InputDirection { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsFiring { get; private set; }

        public bool IsSliding { get; private set; }

        public bool IsCrouching { get; private set; }

        public bool IsAiming { get; private set; }

        public bool IsJumping { get; private set; } = false;

        public bool IsSnapping { get; set; } = false;

        public bool MenuOpenCloseInput { get; private set; }

        private InputAction _menuOpenCloseAction;

        private PlayerInput _playerInput;

        // enum to display active state on screen
        public MovementState movementState;

        public enum MovementState
        {
            Idle,
            Walking,

            // Sprinting,
            // Crouching,
            Sliding,
            Jumping,
            Falling,
            Swinging
        }

        #endregion

        #region wallclimbing and rotation

        [Header("wall climbing")] [SerializeField]
        private float spherecastRadius;

        [SerializeField] private float spherecastDistance;
        [SerializeField] private float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;
        public bool WallInFront { get; private set; }
        public bool WallInFrontLow { get; private set; }
        public bool IsHeadHit { get; private set; }
        public RaycastHit groundHit;
        public RaycastHit headHit;
        public RaycastHit angleHit;
        public RaycastHit wallHit;
        public RaycastHit lowWallHit;

        public (float, float) facingAngles;

        #endregion

        #region Player Movement States

        private PlayerMovementStateManager _manager;
        public PlayerMovementStateIdle IdleState { get; private set; }

        public PlayerMovementStateWalking WalkingState { get; private set; }

        // public PlayerMovementStateSprinting SprintingState { get; private set; }
        // public PlayerMovementStateCrouching CrouchingState { get; private set; }
        public PlayerMovementStateJumping JumpingState { get; private set; }
        public PlayerMovementStateFalling FallingState { get; private set; }
        public PlayerMovementStateSliding SlidingState { get; private set; }
        public PlayerMovementStateSwinging SwingingState { get; private set; }
        public PlayerMovementStateDashing DashingState { get; private set; }

        public int jumpCount;
        private bool canIncrementJumpCount = true;
        public float jumpCountCooldown = 0.5f; // Adjust the cooldown duration as needed
        private float jumpCountCooldownTimer = 0f;

        #endregion

        #region puddleEffect

        public delegate void PlayerInPuddle();

        public static PlayerInPuddle onPlayerInPuddle;
        public static PlayerInPuddle onPlayerLeftPuddle;

        [Header("Death puddle")]
        // after delay player dies and needs to be respawned
        [Tooltip("value Rigidbody velocity is divided by.")]
        [SerializeField]
        private float puddleSpeedReduction = 2f;

        private float OriginalDesiredMoveSpeed;
        private bool _enteredPuddle;

        private void PuddleEffects()
        {
            if (groundHit.collider && groundHit.collider.CompareTag("DeathPuddle"))
            {
                if (!_enteredPuddle)
                {
                    _enteredPuddle = true;
                    onPlayerInPuddle?.Invoke();
                }

                Slowdown(puddleSpeedReduction);
            }
            else if (groundHit.collider && !groundHit.collider.CompareTag("DeathPuddle") && _enteredPuddle == true &&
                     (_manager.CurrentState != JumpingState || _manager.CurrentState != FallingState))
            {
                _enteredPuddle = false;
                onPlayerLeftPuddle?.Invoke();
            }
        }

        private void Slowdown(float speedReduction)
        {
            // slowdown
            Vector3 velocity = Rb.velocity;
            velocity.x /= speedReduction;
            velocity.z /= speedReduction;
            Rb.velocity = velocity;
        }

        #endregion

        #region Cameras

        public GameObject camera;

        private FreeLookCamera freeLookCamera;

        // private CameraComponentsAdjuster alterCam;
        public GameObject[] cameras;
        public GameObject usedCam;
        public Transform mainCamera;

        enum CamerasEnum
        {
            followCamera,
            mainCamera,
            freelookCamera
        }

        #endregion

        #region Movement direction variables

        public Vector3 movementForward;
        public Vector3 movementRight;

        #endregion

        #region Loading and Saving

        public void LoadData(GameData data)
        {
            Rb.position = data.position;
            Rb.rotation = data.rotation;

            //camScript.RecenterCam();
        }

        public void SaveData(GameData data)
        {
            data.position = Rb.position;
            data.rotation = Rb.rotation;
        }

        #endregion

        #region Delay Player Direction On Transitioning

        bool IsTransitioned = false;

        #endregion

        private void Awake()
        {
            _manager = new PlayerMovementStateManager();

            IdleState = new PlayerMovementStateIdle(_manager, this);
            WalkingState = new PlayerMovementStateWalking(_manager, this);
            // SprintingState = new PlayerMovementStateSprinting(_manager, this);
            // CrouchingState = new PlayerMovementStateCrouching(_manager, this);
            JumpingState = new PlayerMovementStateJumping(_manager, this);
            FallingState = new PlayerMovementStateFalling(_manager, this);
            SlidingState = new PlayerMovementStateSliding(_manager, this);
            SwingingState = new PlayerMovementStateSwinging(_manager, this, GetComponent<PlayerSwingHandler>(),
                GetComponent<TrailRenderer>());
            DashingState = new PlayerMovementStateDashing(_manager, this, dashDuration, dashForce, dashCooldown,
                DashUpwardForce);
            Rb = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _menuOpenCloseAction = _playerInput.actions["MenuOpenClose"];
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            _swing = GetComponent<PlayerSwingHandler>();
            _collider = GetComponent<Collider>();
            //initiate Camera componenets 
            // alterCam = camera.GetComponent<CameraComponentsAdjuster>();
            freeLookCamera = GetComponent<FreeLookCamera>();
        }

        private void Start()
        {
            Rb.freezeRotation = true; // stop character from falling over
            StartYScale = transform.localScale.y;

            _manager.Initialize(IdleState);
            GetComponent<TrailRenderer>().enabled = false;

            //Start with freeLookCamera 
            // alterCam.FreeLook();
        }

        private void Update()
        {
            PuddleEffects();

            // wake up the rigidbody when it's sleeping so collisions keep working.
            // This can affect performance, but it should be fine to at least have it on the player.
            if (Rb.IsSleeping())
                Rb.WakeUp();

            MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
            // Debug.Log("Rigidbody Velocity: " + Rb.velocity.magnitude);
            speed_text.text = MoveSpeed + "/" + DesiredMoveSpeed;
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
            
            
            if (!IsTransitioned && _manager.CurrentState != JumpingState)
            {
                CalculatePlayerVMovement();
            }

            HandleRotation();
        }

        private void SetPlayerDirection()
        {
            movementForward = transform.forward;
            movementRight = Vector3.Cross(transform.up, movementForward);
        }

        public Vector3 CalculateMoveDirection(float angle, RaycastHit hit)
        {
            Quaternion facingRotation = Quaternion.Euler(0f, angle, 0f);
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion combinedRotation = surfaceRotation * facingRotation;
            Vector3 moveDirection = combinedRotation * Vector3.forward;
            return moveDirection;
        }

        private void ResetCameraPositionBelowPlane()
        {
            // Calculate the distance between the camera and the plane
            Vector3 cameraToPlane = transform.position - cam.transform.position;
            float distanceToPlane = Vector3.Dot(cameraToPlane, transform.up);

            // If the camera is below the plane
            if (distanceToPlane > 0)
            {
                // Calculate the new camera position
                Vector3 newCameraPosition = cam.transform.position + transform.up * distanceToPlane;

                // Set the new camera position
                cam.transform.position = newCameraPosition;
            }
        }

        private void CalculatePlayerVMovement()
        {
            float rayAngle = Vector3.Angle(movementForward, movementRight);
            Vector3 rightOrigin = cam.transform.position + cam.transform.right * 50f;
            Vector3 upOrigin = cam.transform.position + cam.transform.up * 50f;
            Plane fPlane = new Plane(transform.up, transform.position);
            Plane rPlane = new Plane(transform.up, transform.position);

            Ray rRay = new Ray(rightOrigin, cam.transform.forward * 100);
            Ray uRay = new Ray(upOrigin, cam.transform.forward * 100);

            Vector3 cam2Player = transform.position - cam.transform.position;
            float upOrDown = Vector3.Dot(cam2Player, transform.up);

            float angle = Vector3.Angle(uRay.direction, fPlane.normal);
            /*Debug.Log(angle);*/
            if (angle >= 100)
            {
                if (fPlane.Raycast(uRay, out float uEnter))
                {
                    Vector3 fPoint = uRay.GetPoint(uEnter);
                    Debug.DrawLine(upOrigin, fPoint, Color.red);
                    movementForward = fPoint - transform.position;
                    Debug.DrawLine(transform.position,
                        transform.position + movementForward.normalized * ((upOrDown > 0) ? -2 : 2), Color.red);
                }

                if (rPlane.Raycast(rRay, out float rEnter))
                {
                    movementRight = Vector3.Cross(transform.up, movementForward);
                    Debug.DrawLine(transform.position,
                        transform.position + movementRight.normalized * ((upOrDown > 0) ? -2 : 2), Color.green);
                }
            } 
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            //Check if there has been a hit yet
            if (Grounded)
            {
                //Draw a Ray forward from GameObject toward the hit
                Gizmos.DrawRay(transform.position, -transform.up * groundHit.distance);
                //Draw a cube that extends to where the hit exists
                // Gizmos.DrawWireCube(transform.position + -transform.up * groundHit.distance, new Vector3(0.5f, 0.1f, 0.7f) * 2);
                Gizmos.DrawWireCube(-transform.up * groundHit.distance, new Vector3(0.5f, 0.1f, 0.7f) * 2);
            }
            //If there hasn't been a hit yet, draw the ray at the maximum distance
            else
            {
                //Draw a Ray forward from GameObject toward the maximum distance
                Gizmos.DrawRay(transform.position, -transform.up * (playerHeight * 0.5f + 0.2f));
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(-transform.up * (playerHeight * 0.5f + 0.2f), new Vector3(0.5f, 0.1f, 0.7f) * 2);
                // Gizmos.DrawWireCube(transform.position + -transform.up * (playerHeight * 0.5f + 0.2f), new Vector3(0.5f, 0.1f, 0.7f) * 2);
            }
        }

        private void SurfaceCheck() // written with the help of google gemini. https://g.co/gemini/share/8d280f3a447f
        {
            if (IsDashing)
                return;
            // check if player is on the ground
            // Grounded = Physics.Raycast(transform.position, playerObj.TransformDirection(Vector3.down), out groundHit,
            //     playerHeight * 0.5f + 0.2f, ground);
            Vector3 halfExtents = _collider.bounds.extents;
            Grounded = Physics.BoxCast(transform.position, new Vector3(0.5f, 0.1f, 0.7f), -transform.up, out groundHit,
                transform.rotation, playerHeight * 0.5f + 0.2f, ground);

            // wall check
            Vector3 wallCastHeight = playerObj.up * 0.4f;
            float wallCastDistance = 1f;
            WallInFront = Physics.Raycast(transform.position + wallCastHeight, playerObj.forward,
                out wallHit, (wallCastDistance), ground);
            WallInFrontLow = Physics.Raycast(transform.position + -wallCastHeight, playerObj.forward, out lowWallHit,
                wallCastDistance, ground);
            Debug.DrawRay(transform.position + -wallCastHeight,
                playerObj.forward * wallCastDistance, Color.red);

            IsHeadHit = Physics.Raycast(transform.position, playerObj.up, out headHit,
                playerHeight * 0.5f + 0.2f, ground);

            // check if an angled surface is in front of the player
            float edgeCastDistance = 1.5f;
            EdgeFound = Physics.Raycast(transform.position + (playerObj.forward) + (playerObj.up * .5f),
                -playerObj.up + (0.45f * -playerObj.forward), out angleHit, edgeCastDistance, ground);

#if UNITY_EDITOR
            // debug ray drawings
            // to the ground
            if (!Grounded)
                Debug.DrawRay(transform.position,
                    playerObj.TransformDirection(Vector3.down) * (playerHeight * 0.5f + 0.2f), Color.red);
            else
                Debug.DrawRay(transform.position,
                    playerObj.TransformDirection(Vector3.down) * (playerHeight * 0.5f + 0.2f), Color.green);
            // to the front
            if (WallInFront)
            {
                Debug.DrawRay(transform.position + wallCastHeight,
                    playerObj.forward * wallCastDistance, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position + wallCastHeight,
                    playerObj.forward * wallCastDistance, Color.red);
            }

            // top
            Debug.DrawRay(transform.position, playerObj.up * (playerHeight * 0.5f + 0.2f), Color.magenta);

            // angled in the front
            Debug.DrawRay(transform.position + (playerObj.forward) + (playerObj.up * 0.5f),
                -playerObj.up + -playerObj.forward * (0.45f * edgeCastDistance), Color.yellow);
#endif
        }

        private void IsTransitionedChanger()
        {
            if (IsTransitioned)
            {
                IsTransitioned = false;
                return;
            }

            IsTransitioned = true;
        }

        private void HandleRotation()
        {
            float angle = 0;
            RaycastHit hit = new RaycastHit();

            float cos70 = Mathf.Cos(70 * Mathf.Deg2Rad);

            // get the dot product of the ground normal and the angleHit normal to check the angle between them.
            float dotProduct = Vector3.Dot(groundHit.normal.normalized, angleHit.normal.normalized);

            if (_manager.CurrentState == JumpingState)
                return;

            facingAngles = GetFacingAngle(InputDirection);

            if (WallInFront && InputDirection != Vector2.zero && _manager.CurrentState != SwingingState)
            {
                angle = facingAngles.Item1;
                hit = wallHit;
                if (hit.transform.up == -Vector3.up)
                {
                    angle = -facingAngles.Item1;
                    TransformUponAngle(hit, angle);
                }
                else
                {
                    TransformUponAngle(hit, angle);
                }

                IsTransitioned = true;
                SetPlayerDirection();
            }
            else if (WallInFrontLow && InputDirection != Vector2.zero && _manager.CurrentState != SwingingState)
            {
                angle = facingAngles.Item1;
                hit = lowWallHit;
                TransformUponAngle(hit, angle);
                IsTransitioned = true;
                SetPlayerDirection();
            }
            // if an edge is found and the angle between the normals is 90 degrees or more align the player with the new surface
            else if (EdgeFound && InputDirection != Vector2.zero && dotProduct <= cos70 &&
                     _manager.CurrentState != SwingingState)
            {
                EdgeTransformation();
                IsTransitioned = true;
                SetPlayerDirection();
            }
            // TODO: Change camera player rotation
            else if (Grounded && InputDirection != Vector2.zero || _manager.CurrentState == SwingingState)
            {
                IsTransitioned = false;
                if (_manager.CurrentState != SwingingState && groundHit.collider.CompareTag("smoothObject"))
                {
                    angle = facingAngles.Item1;
                    hit = groundHit;
                    TransformUponAngle(hit, angle);
                    // smooth turn
                    transform.rotation = Quaternion.Slerp(playerObj.rotation, orientation.rotation,
                        Time.deltaTime * rotationSpeed);
                }

                else
                {
                    TurnPlayer();
                }
            }
            else if (IsHeadHit && _manager.CurrentState != SwingingState)
            {
                angle = facingAngles.Item1;
                hit = headHit;
                TransformUponAngle(hit, angle);
                IsTransitioned = true;
                SetPlayerDirection();
            }
            else if (InputDirection != Vector2.zero)
            {
                IsTransitioned = true;
                SetPlayerDirection();
                orientation.rotation = Quaternion.Euler(0f, facingAngles.Item2, 0f);
                transform.rotation = orientation.rotation;
            }
        }

        private void DirectionInverse()
        {
            if (transform.up == -Vector3.up)
            {
                movementForward = -transform.forward;
                movementRight = Vector3.Cross(transform.up, -movementForward);
            }
        }

        private void TransformUponAngle(RaycastHit hit, float angle)
        {
            Quaternion cameraRotation = Quaternion.Euler(0f, angle, 0f);
            Quaternion surfaceAlignment = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion combinedRotation = surfaceAlignment * cameraRotation;
            orientation.rotation = combinedRotation;
            transform.rotation = orientation.rotation;
        }

        private void EdgeTransformation()
        {
            Quaternion oldOrientation = transform.rotation;
            Quaternion rotation = Quaternion.FromToRotation(groundHit.normal, angleHit.normal);
            Quaternion newOrientation = rotation * oldOrientation;

            orientation.rotation = newOrientation;
            transform.rotation = newOrientation;

            Vector3 newPlayerPos = angleHit.point;
            Vector3 offset = (playerHeight - 1) * 0.5f * angleHit.normal;

            transform.position = newPlayerPos + offset;

            Rb.velocity = Vector3.zero;
        }

        private void TurnPlayer()
        {
            Vector3 forward = movementForward.normalized;
            Vector3 right = movementRight.normalized;

            Vector3 forwardRelativeInput = InputDirection.y * forward;
            Vector3 rightRelativeInput = InputDirection.x * right;
            // Calculate the combined movement direction relative to the camera
            Vector3 combinedMovement = forwardRelativeInput + rightRelativeInput;

            // Project the combined movement onto the horizontal plane
            combinedMovement = Vector3.ProjectOnPlane(combinedMovement, transform.up).normalized;

            // Check if movement is negligible or zero
            if (combinedMovement == Vector3.zero || Vector3.Angle(combinedMovement, transform.forward) < Mathf.Epsilon)
            {
                return;
            }

            // Project the combined movement onto the horizontal plane again (not normalized this time)
            combinedMovement = Vector3.ProjectOnPlane(combinedMovement, transform.up);

            // Rotate towards the combined movement direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(combinedMovement, transform.up), 15f);
        }

        private (float, float) GetFacingAngle(Vector2 direction)
        {
            // Target angle based on camera
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
            // Angle to face before reaching target to make it smoother
            float angle = Mathf.SmoothDampAngle(cam.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                turnSmoothTime);
            return (targetAngle, angle);
        }

        // public void ChangeMomentum(float speedIncreaseMultiplier)
        // {
        //     StopAllCoroutines();
        //     StartCoroutine(SmoothlyLerpMoveSpeed(speedIncreaseMultiplier));
        // }
        //
        // private IEnumerator SmoothlyLerpMoveSpeed(float speedIncreaseMultiplier)
        // {
        //     // smoothly lerp movementSpeed to desired value
        //     float time = 0;
        //     float difference = Mathf.Abs(DesiredMoveSpeed - MoveSpeed);
        //     float startValue = MoveSpeed;
        //
        //     while (time < difference && Rb.velocity.magnitude > 0.5f)
        //     {
        //         MoveSpeed = Mathf.Lerp(startValue, DesiredMoveSpeed, time / difference);
        //
        //         time += Time.deltaTime * speedIncreaseMultiplier;
        //
        //         yield return null;
        //     }
        //
        //     MoveSpeed = DesiredMoveSpeed;
        // }


        // input callbacks
        public void OnMove(InputValue value)
        {
            InputDirection = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            IsJumping = value.isPressed;
            if (IsJumping && (_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState))
            {
                // isJumping = true;
                _manager.SwitchState(JumpingState);
                // StartCoroutine(DelayedStateSwitch(JumpingState, .5f)); // 50 frames delay
            }
        }

        // private IEnumerator DelayedStateSwitch(PlayerMovementBaseState nextState, float frameCount)
        // {
        //     float elapsedTime = 0f;
        //     while (elapsedTime < frameCount)
        //     {
        //         yield return null;
        //         elapsedTime += Time.deltaTime;
        //     }
        //     
        //     _manager.SwitchState(nextState);
        //     isJumping = false;
        // }


        // public void OnSprint(InputValue value)
        // {
        //     IsSprinting = value.isPressed;
        //     
        //     // might be better to handle this in the current state using a special function for checking if a state switch is logical
        //     
        //     if (_manager.CurrentState == WalkingState && IsSprinting)
        //     {
        //         _manager.SwitchState(SprintingState);
        //     }
        //     else if (_manager.CurrentState == SprintingState && !IsSprinting)
        //     {
        //         _manager.SwitchState(WalkingState);
        //     }
        // }

        // public void OnDash()
        // {
        //     if(_manager.CurrentState != SwingingState && _manager.CurrentState != SlidingState)
        //         _manager.SwitchState(DashingState);
        // }

        public void OnFire(InputValue value)
        {
            IsFiring = value.isPressed;

            if (_swing.CanSwing)
                _manager.SwitchState(SwingingState);
        }

        // public void OnCrouch(InputValue value)
        // {
        //     IsCrouching = value.isPressed;
        //     
        //     if (IsCrouching && (_manager.CurrentState == IdleState || _manager.CurrentState == WalkingState))
        //         _manager.SwitchState(CrouchingState);
        //     else if (!IsCrouching && _manager.CurrentState == CrouchingState)
        //     {
        //         if (InputDirection != Vector2.zero)
        //         {
        //             _manager.SwitchState(WalkingState);
        //         }
        //         else
        //         {
        //             _manager.SwitchState(IdleState);
        //         }
        //     }
        // }

        // public void OnSlide(InputValue value)
        // {
        //     IsSliding = value.isPressed;
        //     if (IsSliding && (_manager.CurrentState == WalkingState /*|| _manager.CurrentState == SprintingState*/
        //                       || (Grounded && _manager.CurrentState == JumpingState)
        //                       || Grounded && _manager.CurrentState == FallingState))
        //         _manager.SwitchState(SlidingState);
        //     else if (!IsSliding && _manager.CurrentState == SlidingState)
        //     {
        //         if (Grounded && InputDirection == Vector2.zero)
        //             _manager.SwitchState(IdleState);
        //         else if (InputDirection != Vector2.zero)
        //         {
        //             // if (IsSprinting)
        //             //     _manager.SwitchState(SprintingState);
        //             // else
        //             _manager.SwitchState(WalkingState);
        //         }
        //     }
        // }

        public void OnAim(InputValue value)
        {
            IsAiming = value.isPressed;
        }

        public void PlayLandVFX()
        {
            ParticleSystem particleSystem = dustVFX.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // Play the particle system
                particleSystem.Play();
            }
        }
    }
}