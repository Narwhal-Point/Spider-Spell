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

        enum CameraStyle
        {
            Normal,
            Aiming
        }

        private CameraStyle _currentCamera = CameraStyle.Normal;
        
        
        
        
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
            _currentCamera = _playerMovement.Aiming ? CameraStyle.Aiming : CameraStyle.Normal;
            
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            if (_currentCamera == CameraStyle.Normal)
            {
                cameras[1].gameObject.SetActive(false);
                cameras[0].gameObject.SetActive(true);
                if(crosshair.activeSelf)
                    crosshair.SetActive(false);
                // input direction
                Vector2 viewDirection = _playerMovement.Moving;
            
                // set direction of player
                Vector3 inputDir = orientation.forward * viewDirection.y + orientation.right * viewDirection.x;
            
                // Debug.Log("inputDir: " + inputDir);
            
                // rotate player to direction
                if (inputDir != Vector3.zero)
                    playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
            
            else if (_currentCamera == CameraStyle.Aiming)
            {
                if(!crosshair.activeSelf)
                    crosshair.SetActive(true);
                cameras[0].gameObject.SetActive(false);
                cameras[1].gameObject.SetActive(true);
                Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
                orientation.forward = dirToCombatLookAt.normalized;

                playerObj.forward = dirToCombatLookAt.normalized;
            }
        }
    }
}
