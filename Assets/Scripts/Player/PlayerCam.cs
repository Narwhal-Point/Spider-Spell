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

        // Update is called once per frame
        void Update()
        {
            HandleCameraSwitch();
            
            // check if player used recenter key
            if (InputManager.instance.RecenterInput)
            {
                // recenter the current camera
                RecenterCam(_playerMovement.IsAiming ? cameras[1] : cameras[0]);
            }
        }

        private void HandleCameraSwitch()
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
        
        // hacky way of aligning the camera with the player on startup.
        // Hidden behind the spider spawn in screen from the death effect.
        private static IEnumerator DisableCamRecenter(CinemachineFreeLook cam, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            cam.m_RecenterToTargetHeading.m_enabled = false;
            cam.m_YAxisRecentering.m_enabled = false;
            // cam.m_RecenterToTargetHeading.m_RecenteringTime = 1;
            // cam.m_RecenterToTargetHeading.m_WaitTime = 2;
        }

        public void InstantRecenterCam()
        {
            cameras[0].m_RecenterToTargetHeading.m_RecenteringTime = 0;
            cameras[0].m_RecenterToTargetHeading.m_WaitTime = 0;
            cameras[0].m_RecenterToTargetHeading.m_enabled = true;
            
            StartCoroutine(DisableCamRecenter(cameras[0], 1f));
        }

        private void RecenterCam(CinemachineFreeLook cam)
        {
            cam.m_RecenterToTargetHeading.m_WaitTime = 0;
            cam.m_RecenterToTargetHeading.m_RecenteringTime = 1;
            
            cam.m_YAxisRecentering.m_WaitTime = 0;
            cam.m_YAxisRecentering.m_RecenteringTime = 1;
            
            cam.m_RecenterToTargetHeading.m_enabled = true;
            cam.m_YAxisRecentering.m_enabled = true;
            
            StartCoroutine(DisableCamRecenter(cam, 2.5f));
        }
    }
}