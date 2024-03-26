using Cinemachine;
using Player.Movement;
using UnityEngine;

namespace Player
{
    public class PlayerCam : MonoBehaviour
    {
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform player;
        [SerializeField] private Transform playerObj;

        // I don't like having to include this but it's the only way I figured out on how to get the move direction in this script
        [SerializeField] private GameObject playerObject;
        private PlayerMovement _playerMovement;

        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private Transform combatLookAt;

        [SerializeField] private CinemachineVirtualCameraBase[] cameras;
        [SerializeField] private GameObject crosshair;

        [SerializeField] float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;


        public enum CameraStyle
        {
            Normal,
            Aiming
        }

        public CameraStyle CurrentCamera { get; private set; } = CameraStyle.Normal;


        // Start is called before the first frame update
        void Start()
        {
            _playerMovement = playerObject.GetComponent<PlayerMovement>();
            // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            // CurrentCamera = _playerMovement.Aiming ? CameraStyle.Aiming : CameraStyle.Normal;
            //
            // Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            // orientation.forward = viewDir.normalized;
            //
            // if (CurrentCamera == CameraStyle.Normal)
            // {
            //     cameras[1].gameObject.SetActive(false);
            //     cameras[0].gameObject.SetActive(true);
            //     if(crosshair.activeSelf)
            //         crosshair.SetActive(false);
            //     // input direction
            //     Vector2 viewDirection = _playerMovement.Moving;
            //
            //     // set direction of player
            //     Vector3 inputDir = orientation.forward * viewDirection.y + orientation.right * viewDirection.x;
            //
            //     // Debug.Log("inputDir: " + inputDir);
            //
            //     // rotate player to direction
            //     if (inputDir != Vector3.zero)
            //         playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            // }
            //
            // else if (CurrentCamera == CameraStyle.Aiming)
            // {
            //     if(!crosshair.activeSelf)
            //         crosshair.SetActive(true);
            //     cameras[0].gameObject.SetActive(false);
            //     cameras[1].gameObject.SetActive(true);
            //     Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            //     orientation.forward = dirToCombatLookAt.normalized;
            //
            //     playerObj.forward = dirToCombatLookAt.normalized;
            // }

            CurrentCamera = _playerMovement.Aiming ? CameraStyle.Aiming : CameraStyle.Normal;

            if (CurrentCamera == CameraStyle.Normal)
            {
                cameras[1].gameObject.SetActive(false);
                cameras[0].gameObject.SetActive(true);
                if (crosshair.activeSelf)
                    crosshair.SetActive(false);
            }

            else if (CurrentCamera == CameraStyle.Aiming)
            {
                if (!crosshair.activeSelf)
                    crosshair.SetActive(true);
                cameras[0].gameObject.SetActive(false);
                cameras[1].gameObject.SetActive(true);
            }
            
            _playerMovement.facingAngles = GetFacingAngle(_playerMovement.Moving);
            if (_playerMovement.wallInFront && _playerMovement.Moving != Vector2.zero)
            {
                Quaternion cameraRotation = Quaternion.Euler(0f, _playerMovement.facingAngles.Item1, 0f);
                Quaternion surfaceAlignment =
                    Quaternion.FromToRotation(Vector3.up, _playerMovement.frontWallHit.normal);
                Quaternion combinedRotation = surfaceAlignment * cameraRotation;
                orientation.rotation = combinedRotation;
            }
            else if (_playerMovement.Grounded && _playerMovement.Moving != Vector2.zero)
            {
                Quaternion cameraRotation = Quaternion.Euler(0f, _playerMovement.facingAngles.Item1, 0f);
                Quaternion surfaceAlignment =
                    Quaternion.FromToRotation(Vector3.up, _playerMovement.groundHit.normal);
                Quaternion combinedRotation = surfaceAlignment * cameraRotation;
                orientation.rotation = combinedRotation;
            }
            else if(_playerMovement.Moving != Vector2.zero)
            {
                orientation.rotation = Quaternion.Euler(0f, _playerMovement.facingAngles.Item2, 0f);
            }

            playerObj.rotation = orientation.rotation;
        }

        private (float, float) GetFacingAngle(Vector2 direction)
        {
            // Target angle based on camera
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
            // Angle to face before reaching target to make it smoother
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                turnSmoothTime);
            return (targetAngle, angle);
        }
    }
}