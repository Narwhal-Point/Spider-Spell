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

        [SerializeField] private CinemachineFreeLook[] cameras;
        [SerializeField] private GameObject crosshair;

        public enum CameraStyle
        {
            Normal,
            Aiming
        }

        public CameraStyle CurrentCamera { get; private set; } = CameraStyle.Normal;
        private CameraStyle prevStyle = CameraStyle.Normal;
        



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
            CurrentCamera = _playerMovement.Aiming ? CameraStyle.Aiming : CameraStyle.Normal;
            
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            if (CurrentCamera == CameraStyle.Normal)
            {
                // set the position of the aiming camera as the position of the normal camera
                if (prevStyle == CameraStyle.Aiming)
                {
                    prevStyle = CameraStyle.Normal;
                    cameras[0].m_YAxis.Value = cameras[1].m_YAxis.Value;
                }

                UseNormalCamera();
            }
            
            else if (CurrentCamera == CameraStyle.Aiming)
            {
                // set the position of the normal camera as the position of the aiming camera
                // fixes the weird issue where the cameras would look at a completely different direction from each other
                if (prevStyle == CameraStyle.Normal)
                {
                    prevStyle = CameraStyle.Aiming;
                    cameras[1].m_YAxis.Value = cameras[0].m_YAxis.Value;
                }
                
                UseAimingCamera();
            }
        }

        private void UseNormalCamera()
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

        private void UseAimingCamera()
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
