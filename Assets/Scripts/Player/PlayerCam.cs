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
        [SerializeField] private Transform combatLookAt;

        [SerializeField] private CinemachineVirtualCameraBase[] cameras;
        [SerializeField] private GameObject crosshair;
        
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
            CurrentCamera = _playerMovement.IsAiming ? CameraStyle.Aiming : CameraStyle.Normal;

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

        }
    }
}