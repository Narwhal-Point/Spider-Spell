using System.Collections;
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

        [SerializeField] private CinemachineFreeLook[] cameras;
        [SerializeField] private GameObject crosshair;
        
        public enum CameraStyle
        {
            Normal,
            Aiming
        }

        public CameraStyle CurrentCamera { get; private set; } = CameraStyle.Normal;
        private CameraStyle PrevCamera { get; set; } = CameraStyle.Normal;

        // Start is called before the first frame update
        void Start()
        {
            // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            
        }

        // hacky way of aligning the camera with the player on startup.
        // should probably be hidden behind a quick loading animation or something
        // TODO: Loading animation screen
        IEnumerator disableCamRecenter()
        {
            yield return new WaitForSeconds(1);
            
            cameras[0].m_RecenterToTargetHeading.m_enabled = false;
        }

        public void RecenterCam()
        {
            cameras[0].m_RecenterToTargetHeading.m_RecenteringTime = 0;
            cameras[0].m_RecenterToTargetHeading.m_WaitTime = 0;
            cameras[0].m_RecenterToTargetHeading.m_enabled = true;
            
            StartCoroutine(disableCamRecenter());
        }

        // Update is called once per frame
        void Update()
        {
            CurrentCamera = _playerMovement.IsAiming ? CameraStyle.Aiming : CameraStyle.Normal;

            if (CurrentCamera == CameraStyle.Normal)
            {
                if (PrevCamera == CameraStyle.Aiming)
                {
                    cameras[0].m_YAxis = cameras[1].m_YAxis;
                    cameras[0].m_XAxis = cameras[1].m_XAxis;
                    PrevCamera = CameraStyle.Normal;
                }

                cameras[1].gameObject.SetActive(false);
                cameras[0].gameObject.SetActive(true);
                if (crosshair.activeSelf)
                    crosshair.SetActive(false);
            }

            else if (CurrentCamera == CameraStyle.Aiming)
            {
                if (PrevCamera == CameraStyle.Normal)
                {
                    cameras[1].m_YAxis = cameras[0].m_YAxis;
                    cameras[1].m_XAxis = cameras[0].m_XAxis;
                    PrevCamera = CameraStyle.Aiming;
                }

                if (!crosshair.activeSelf)
                    crosshair.SetActive(true);
                cameras[0].gameObject.SetActive(false);
                cameras[1].gameObject.SetActive(true);
            }

        }
    }
}