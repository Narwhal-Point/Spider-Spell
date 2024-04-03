using UnityEngine;

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
        public float maxSwingAimDistance = 25f;

        // maximum length of line. Line will shorten if player is further away than this value
        public float maxSwingDistance = 12f;
        public Vector3 SwingPoint { get; set; }
        public SpringJoint Joint { get; set; }
        public Vector3 CurrentGrapplePosition { get; set; }

        [Header("Prediction")] 
        public RaycastHit predictionHit;
        [SerializeField] private Transform predicitionPoint;

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
            if (Joint != null || camScript.CurrentCamera == PlayerCam.CameraStyle.Normal)
                return;
            
            // TODO: Change to create the ray once and then just change the positions
            Physics.Raycast(cam.position, cam.forward, out var raycastHit, maxSwingAimDistance, whatIsGrappleable);

            // draw direction of raycast
            Debug.DrawRay(cam.position, cam.forward * maxSwingAimDistance, Color.yellow);

            Vector3 realHitPoint;

            // direct hit
            if (raycastHit.point != Vector3.zero)
            {
                realHitPoint = raycastHit.point;
            }
            else
                realHitPoint = Vector3.zero;

            if (realHitPoint != Vector3.zero)
            {
                predicitionPoint.gameObject.SetActive(true);
                predicitionPoint.position = realHitPoint;
            }
            else
                predicitionPoint.gameObject.SetActive(false);

            predictionHit = raycastHit;
        }

        public void DestroyJoint()
        {
            Destroy(Joint);
        }
    }
}