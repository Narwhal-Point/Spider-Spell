using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Movement
{
    public class PlayerSwingHandler : MonoBehaviour
    {
        [Header("References")] 
        public LineRenderer lr;
        public Transform swingOrigin, cam;
        [SerializeField] private PlayerCam camScript;
        public LayerMask whatIsGrappleable;

        [Header("Swinging")] 
        public float maxSwingDistance = 45f;
        
        public Vector3 SwingPoint { get; set; }
        public SpringJoint Joint { get; set; }
        public Vector3 CurrentGrapplePosition { get; set; }
        
        public bool CanSwing { get; private set; }

        [Header("Prediction")] 
        public RaycastHit predictionHit;
        [SerializeField] private Transform predicitionPoint;
        [SerializeField] private Transform crosshair;

        private Image[] crosshairImages = new Image[5];

        private void Start()
        {
            predicitionPoint.gameObject.SetActive(false);

            for (int i = 0; i < crosshair.childCount; i++)
            {
                GameObject childObject = crosshair.GetChild(i).gameObject;
                crosshairImages[i] = childObject.GetComponent<Image>();

            }
        }

        private void Update()
        {
            CheckForSwingPoints();
        }

        private void LateUpdate()
        {
            DrawRope();
        }

        void DrawRope()
        {
            // if not grappling don't draw rope
            if (!Joint)
                return;

            // make it so the line doesn't appear instantly (doesn't work)
            CurrentGrapplePosition = Vector3.Lerp(CurrentGrapplePosition, SwingPoint, Time.deltaTime * 1f);

            lr.SetPosition(0, swingOrigin.position);
            lr.SetPosition(1, SwingPoint);
        }

        private void CheckForSwingPoints()
        {
            if (Joint)
            {
                // set the color back to white while swinging
                if (crosshairImages[4].color != Color.white)
                {
                    foreach (var image in crosshairImages)
                    {
                        image.color = Color.white;
                    }
                }
                return;
            }

            // TODO: Change to create the ray once and then just change the positions
            Physics.Raycast(cam.position, cam.forward, out var raycastHit, maxSwingDistance, whatIsGrappleable);

            // draw direction of raycast
            Debug.DrawRay(cam.position, cam.forward * maxSwingDistance, Color.yellow);

            // direct hit
            if (raycastHit.point != Vector3.zero)
            {
                foreach (var image in crosshairImages)
                {
                    image.color = Color.white;
                }

                CanSwing = true;
                predicitionPoint.position = raycastHit.point;
            }
            else
            {
                CanSwing = false;
                foreach (var image in crosshairImages)
                {
                    image.color = Color.red;
                }
            }

            predictionHit = raycastHit;
        }

        public void DestroyJoint()
        {
            Destroy(Joint);
        }
    }
}