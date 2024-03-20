using UnityEngine;

namespace Player.Movement
{
    public class PlayerSwingHandler : MonoBehaviour
    {
        [Header("References")] 
        public LineRenderer lr;
        public Transform swingOrigin, cam, player;
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
        [SerializeField] private float predictionSphereCastRadius;
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
            if(!Joint)
                return;
        
            // make it so the line doesn't appear instantly (doesn't work)
            CurrentGrapplePosition = Vector3.Lerp(CurrentGrapplePosition, SwingPoint, Time.deltaTime * 8f);
        
            lr.SetPosition(0, swingOrigin.position);
            lr.SetPosition(1, SwingPoint);
        }

        private void CheckForSwingPoints()
        {
            if(Joint != null)
                return;

            Vector3 castPosition = new Vector3(player.position.x, cam.position.y, player.position.z);
        
            Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, 
                out var sphereCastHit, maxSwingAimDistance, whatIsGrappleable);
            
            // draw direction of spherecast
            Debug.DrawRay(cam.position, cam.forward * maxSwingAimDistance, Color.cyan);
            
            Physics.Raycast(cam.position, cam.forward, out var raycastHit, maxSwingAimDistance, whatIsGrappleable);
            
            // draw direction of raycast
            Debug.DrawRay(cam.position, cam.forward * maxSwingAimDistance, Color.yellow);

            Vector3 realHitPoint;

            // direct hit
            if (raycastHit.point != Vector3.zero)
            {
                Debug.Log("Direct");
                realHitPoint = raycastHit.point;
            }

            // indirect (predicted) hit
            else if (sphereCastHit.point != Vector3.zero)
            {
                Debug.Log("Indirect");
                realHitPoint = sphereCastHit.point;
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

            predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
        }
        
        public void DestroyJoint()
        {
            Destroy(Joint);
        }
    }
}