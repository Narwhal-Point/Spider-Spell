using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float walkSpeed;
        public float sprintSpeed;
        private float _moveSpeed = 10;
        public float slideSpeed;

        private float _desiredMoveSpeed;
        private float _lastDesiredMoveSpeed;
        
        public float speedIncreaseMultiplier;
        public float slopeIncreaseMultiplier;
    
        public float groundDrag;

        [Header("Jumping")]
        public float jumpForce;
        public float airMultiplier;
        public float jumpCooldown;
        private bool _readyToJump = true;

        [Header("Crouching")] 
        public float crouchSpeed;
        public float crouchYScale;
        private float _startYScale;
        
        [Header("Keybinds")] 
        public KeyCode jumpKey = KeyCode.Space;
        [FormerlySerializedAs("sprintKey")] public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode crouchKey = KeyCode.C;
    

        [Header("Ground Check")] 
        public float playerHeight;
        public LayerMask ground;
        private bool _grounded;


        [Header("Slope Handling")] 
        public float maxSlopeAngle;
        private RaycastHit _slopeHit;
        private bool _exitingSlope;
    
        
        public Transform orientation;
        
        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        private Rigidbody _rb;
        
        public MovementState state;
        public enum MovementState
        {
            Walking,
            Sprinting,
            Crouching,
            Sliding,
            Air
        }

        public bool sliding;

        // Start is called before the first frame update
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true; // stop character from falling over

            _readyToJump = true;

            _startYScale = transform.localScale.y;
        }

        // Update is called once per frame
        private void Update()
        {
            // check if player is on the ground
            _grounded = Physics.Raycast(transform.position, Vector3.down, 
                playerHeight * 0.5f + 0.2f, ground);
            
            GetInput();
            SpeedControl();
            StateHandler();

            // set ground drag based on if the character is grounded or not
            _rb.drag = _grounded ? groundDrag : 0;
        }
        
        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void GetInput()
        {
            // get keyboard input
            _horizontalInput = Input.GetAxisRaw("Horizontal"); // A + D
            _verticalInput = Input.GetAxisRaw("Vertical"); // W + S

            if (Input.GetKey(jumpKey) && _readyToJump && _grounded)
            {
                _readyToJump = false;
                Jump();
                
                Invoke(nameof(ResetJump), jumpCooldown);
            }

            // start crouch
            if (Input.GetKeyDown(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }

            // stop crouch
            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
            }
        }

        private void StateHandler()
        {
            if (sliding)
            {
                state = MovementState.Sliding;
            
                if (OnSlope() && _rb.velocity.y < 0.1f)
                    _desiredMoveSpeed = slideSpeed;
                else
                {
                    _desiredMoveSpeed = sprintSpeed;
                }
            }
            
            // Mode - Crouching
            else if (Input.GetKey(crouchKey))
            {
                state = MovementState.Crouching;
                _desiredMoveSpeed = crouchSpeed;
            }

            // Mode - Sprinting
            else if(_grounded && Input.GetKey(sprintKey))
            {
                state = MovementState.Sprinting;
                _desiredMoveSpeed = sprintSpeed;
            }

            // Mode - Walking
            else if (_grounded)
            {
                state = MovementState.Walking;
                _desiredMoveSpeed = walkSpeed;
            }

            // Mode - Air
            else
            {
                state = MovementState.Air;
            }
            
            // check if desiredMoveSpeed has changed a lot
            if (Mathf.Abs(_desiredMoveSpeed - _lastDesiredMoveSpeed) > 4f)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                _moveSpeed = _desiredMoveSpeed;
            }
            
            _lastDesiredMoveSpeed = _desiredMoveSpeed;
        }
        
        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            // smoothly lerp movementSpeed to desired value
            float time = 0;
            float difference = Mathf.Abs(_desiredMoveSpeed - _moveSpeed);
            float startValue = _moveSpeed;

            while (time < difference)
            {
                _moveSpeed = Mathf.Lerp(startValue, _desiredMoveSpeed, time / difference);

                if (OnSlope())
                {
                    float slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                    time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else
                    time += Time.deltaTime * speedIncreaseMultiplier;

                yield return null;
            }
            _moveSpeed = _desiredMoveSpeed;
        }
        
        
        
        private void MovePlayer()
        {
            // get the direction to move towards
            _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

            if (OnSlope() && !_exitingSlope)
            {
              _rb.AddForce(GetSlopeMoveDirection(_moveDirection) * _moveSpeed * 20f, ForceMode.Force);
              
              if (_rb.velocity.y > 0)
                  _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            
            // player on the ground
            else if (_grounded)
            {
                _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), 
                    ForceMode.Force); // move
            }
            else if(!_grounded) // in air
                _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), 
                    ForceMode.Force); // move
            
            // turn gravity off while on slope
            _rb.useGravity = !OnSlope();
        }

        private void SpeedControl()
        {
            // limit speed on slope
            if (OnSlope() && !_exitingSlope)
            {
                if (_rb.velocity.magnitude > _moveSpeed)
                    _rb.velocity = _rb.velocity.normalized * _moveSpeed;
            }
            else // limit speed on ground
            {
                Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

                // limit velocity if needed
                if (flatVel.magnitude > _moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                    _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
                }
            }
        }
        
        private void Jump()
        {
            _exitingSlope = true;
            
            // reset y velocity
            var velocity = _rb.velocity;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
            _rb.velocity = velocity;

            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            _readyToJump = true;
            _exitingSlope = false;
        }

        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
        }
        
        
        
    }
}
