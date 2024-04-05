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
        
        [SerializeField] private PlayerMovement _playerMovement;

        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private Transform combatLookAt;

        [SerializeField] private CinemachineVirtualCameraBase[] cameras;
        [SerializeField] private GameObject crosshair;

        [SerializeField] private float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity;
        public bool Rotating { get; private set; }
        
        public enum CameraStyle
        {
            Normal,
            Aiming
        }

        public CameraStyle CurrentCamera { get; private set; } = CameraStyle.Normal;


        // Start is called before the first frame update
        void Start()
        {
            // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
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
                    Quaternion.FromToRotation(Vector3.up, _playerMovement.currentHit.normal);
                Quaternion combinedRotation = surfaceAlignment * cameraRotation;
                orientation.rotation = combinedRotation;
                transform.rotation = orientation.rotation;
            }
            else if (_playerMovement.Grounded && _playerMovement.Moving != Vector2.zero)
            {
                
                Quaternion cameraRotation = Quaternion.Euler(0f, _playerMovement.facingAngles.Item1, 0f);
                Quaternion surfaceAlignment =
                    Quaternion.FromToRotation(Vector3.up, _playerMovement.currentHit.normal);
                Quaternion combinedRotation = surfaceAlignment * cameraRotation;
                orientation.rotation = combinedRotation;
                
                // slerp the rotation to the turning smooth
                playerObj.rotation = Quaternion.Slerp(playerObj.rotation, orientation.rotation, Time.deltaTime * rotationSpeed);
            }
            else if(_playerMovement.Moving != Vector2.zero)
            {
                orientation.rotation = Quaternion.Euler(0f, _playerMovement.facingAngles.Item2, 0f);
                playerObj.rotation = orientation.rotation;
            }

            

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