using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Movement
{
    public class PlayerSwingHandler : MonoBehaviour
    {
        [Header("References")] 
        public LineRenderer lr;
        public Transform swingOrigin, cam, player;
        public LayerMask whatIsGrappleable;
    
        [Header("Swinging")]
        public float maxSwingDistance = 15f;
        public Vector3 SwingPoint { get; set; }
        public SpringJoint Joint { get; set; }
        public Vector3 CurrentGrapplePosition { get; set; }

        [Header("Prediction")] 
        public RaycastHit predictionHit;
        public float predictionSphereCastRadius;
        public Transform predicitionPoint;

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
            if(!Joint)
                return;
        
            // make it so the line doesn't appear instantly
            CurrentGrapplePosition = Vector3.Lerp(CurrentGrapplePosition, SwingPoint, Time.deltaTime * 8f);
        
            lr.SetPosition(0, swingOrigin.position);
            lr.SetPosition(1, SwingPoint);
        }

        private void CheckForSwingPoints()
        {
            if(Joint != null)
                return;

            RaycastHit sphereCastHit;

            Vector3 castPosition = new Vector3(player.position.x, cam.position.y, player.position.z);
        
            Physics.SphereCast(castPosition, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance,
                whatIsGrappleable);

            RaycastHit raycastHit;
            Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

            Vector3 realHitPoint;

            // direct hit
            if (raycastHit.point != Vector3.zero)
                realHitPoint = raycastHit.point;
        
            // indirect (predicted) hit
            else if (sphereCastHit.point != Vector3.zero)
                realHitPoint = sphereCastHit.point;
            else
                realHitPoint = Vector3.zero;

            if (realHitPoint != Vector3.zero)
            {
                predicitionPoint.gameObject.SetActive(true);
                predicitionPoint.position = realHitPoint;
            }
            else 
                predicitionPoint.gameObject.SetActive(false);

            predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
        }
    }
}