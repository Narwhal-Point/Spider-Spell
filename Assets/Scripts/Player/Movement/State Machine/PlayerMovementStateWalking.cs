using UnityEngine;

namespace Player.Movement.State_Machine
{
    public class PlayerMovementStateWalking : PlayerMovementBaseState
    {
        private RaycastHit _slopeHit;

        public PlayerMovementStateWalking(PlayerMovementStateManager manager, PlayerMovement player) : base(manager,
            player)
        {
        }

        public override void EnterState()
        {
            player.walkingSound.Play();
            player.Rb.useGravity = false;
            player.movementState = PlayerMovement.MovementState.Walking;

            player.lastDesiredMoveSpeed = player.DesiredMoveSpeed;
            player.DesiredMoveSpeed = player.walkSpeed;

            if (Mathf.Abs(player.DesiredMoveSpeed - player.lastDesiredMoveSpeed) > 4f && player.MoveSpeed != 0)
                player.ChangeMomentum(2f);
            else
                player.MoveSpeed = player.DesiredMoveSpeed;
            

            player.Rb.drag = player.groundDrag;
            
        }

        public override void UpdateState()
        {
            SpeedControl();
            if (!player.Grounded)
                manager.SwitchState(player.FallingState);
            else if (player.InputDirection == Vector2.zero)
                manager.SwitchState(player.IdleState);
        }

        public override void FixedUpdateState()
        {
            CalculatePlayerVMovement();
            Vector3 direction = new Vector3(player.InputDirection.x, 0f, player.InputDirection.y).normalized;
            //HandleVectorRotation();
            if (direction.magnitude >= 0.1f)
            {
                MovePlayer();
                TurnPlayer();
            }
        }
        public Vector3 movementForward;
        public Vector3 movementRight;
        private void TurnPlayer()
        {
            Vector3 forward = movementForward.normalized;
            Vector3 right = movementRight.normalized;

            Vector3 forwardRelativeInput = player.InputDirection.y * forward;
            Vector3 rightRelativeInput = player.InputDirection.x * right;
            // Calculate the combined movement direction relative to the camera
            Vector3 combinedMovement = forwardRelativeInput + rightRelativeInput;

            // Project the combined movement onto the horizontal plane
            combinedMovement = Vector3.ProjectOnPlane(combinedMovement, player.transform.up).normalized;

            // Check if movement is negligible or zero
            if (combinedMovement == Vector3.zero || Vector3.Angle(combinedMovement, player.transform.forward) < Mathf.Epsilon)
            {
                return;
            }

            // Project the combined movement onto the horizontal plane again (not normalized this time)
            combinedMovement = Vector3.ProjectOnPlane(combinedMovement, player.transform.up);

            // Rotate towards the combined movement direction
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, Quaternion.LookRotation(combinedMovement, player.transform.up), 15f);
        }


        private void MovePlayer()
        {           
            Vector3 forward = movementForward.normalized;
            Vector3 right = movementRight.normalized;

            Vector3 forwardRelativeInput = player.InputDirection.y * forward;
            Vector3 rightRelativeInput = player.InputDirection.x * right;

            Vector3 planeRelativeMovement = forwardRelativeInput + rightRelativeInput;
            planeRelativeMovement = Vector3.ProjectOnPlane(planeRelativeMovement, player.groundHit.normal).normalized;
            Vector3 movementWithSpeed = planeRelativeMovement * player.MoveSpeed * Time.deltaTime;
            player.MoveDirection = movementWithSpeed;

            player.Rb.AddForce(-player.playerObj.up * 10f, ForceMode.Force);
            player.Rb.AddForce(player.MoveDirection.normalized * (player.MoveSpeed * 10f), ForceMode.Force);
        }
        private Vector3 CalculateMoveDirection(float angle, RaycastHit hit)
        {
            Quaternion facingRotation = Quaternion.Euler(movementForward.x, 0f, movementForward.y);
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            //Quaternion combinedRotation = surfaceRotation * facingRotation;
            Vector3 moveDirection = movementForward * player.InputDirection.y + movementRight * player.InputDirection.x;
            return moveDirection;
        }       
        private void CalculatePlayerVMovement()
        {
            GameObject obj = GameObject.Find("FollowPlayer");
            Vector3 rightOrigin = player.cam.position + player.cam.transform.right * 50f;
            Vector3 upOrigin = player.cam.position + player.cam.transform.up * 50f;
            Plane fPlane = new Plane(player.transform.up, player.transform.position);
            Plane rPlane = new Plane(player.transform.up, player.transform.position);

            Ray rRay = new Ray(rightOrigin, player.cam.forward * 100);
            Ray uRay = new Ray(upOrigin, player.cam.forward * 100);

            Vector3 cam2Player = player.transform.position - player.cam.position;
            float upOrDown = Vector3.Dot(cam2Player, player.transform.up);

            if (fPlane.Raycast(uRay, out float uEnter))
            {
                Vector3 fPoint = uRay.GetPoint(uEnter);
                Debug.DrawLine(upOrigin, fPoint, Color.red);
                movementForward = fPoint - player.transform.position;
                Debug.DrawLine(player.transform.position, player.transform.position + movementForward.normalized * ((upOrDown > 0) ? -2 : 2), Color.red);
            }

            if (rPlane.Raycast(rRay, out float rEnter))
            {
                Vector3 fPoint = rRay.GetPoint(rEnter);
                Debug.DrawLine(rightOrigin, fPoint, Color.red);
                movementRight = fPoint - player.transform.position;
                Debug.DrawLine(player.transform.position, player.transform.position + movementRight.normalized * ((upOrDown > 0) ? -2 : 2), Color.green);
            }
        }
        private void SpeedControl()
        {
            Vector3 flatVel = player.Rb.velocity;

            // limit velocity if needed
            if (flatVel.magnitude > player.MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * player.MoveSpeed;
                player.Rb.velocity = limitedVel;
            }
        }

        public override void ExitState()
        {
            player.walkingSound.Stop();
        }
    }
}