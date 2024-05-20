using UnityEngine;
using UnityEngine.UI;

namespace Player.Movement
{
    public class PlayerGrappleHandler : MonoBehaviour
    {
        [Header("References")] 
        public LineRenderer lr;
        public Transform grappleOrigin;
        public Transform cam;
        [SerializeField] private PlayerCam camScript;
        public LayerMask whatIsGrappleable;
        
        [Header("Grappling")]
        public float maxGrappleDistance = 45f;
        public Vector3 GrapplePoint { get; set; }
        public Vector3 CurrentGrapplePosition { get; set; }
        
        public bool CanGrapple { get; private set; }
        public bool IsGrappling { get; set; }
        
        [Header("Prediction")] 
        public RaycastHit predictionHit;
        [SerializeField] private Transform predicitionPoint;
        [SerializeField] private Transform crosshair;
        
        private readonly Image[] _crosshairImages = new Image[5];
        
        private void Start()
        {
            lr.positionCount = 2;
            predicitionPoint.gameObject.SetActive(false);

            for (int i = 0; i < crosshair.childCount; i++)
            {
                GameObject childObject = crosshair.GetChild(i).gameObject;
                _crosshairImages[i] = childObject.GetComponent<Image>();

            }
        }
        
        private void Update()
        {
            CheckForGrapplePoints();
        }

        private void LateUpdate()
        {
            DrawRope();
        }
        
        void DrawRope()
        {
            // if not grappling don't draw rope
            if (!IsGrappling)
                return;
            
            // make it so the line doesn't appear instantly (doesn't work)
            CurrentGrapplePosition = Vector3.Lerp(CurrentGrapplePosition, GrapplePoint, Time.deltaTime * 1f);

            lr.SetPosition(0, grappleOrigin.position);
            lr.SetPosition(1, GrapplePoint);
        }
        
        private void CheckForGrapplePoints()
        {
            if (IsGrappling)
            {
                // set the color back to white while swinging
                if (_crosshairImages[4].color != Color.white)
                {
                    foreach (var image in _crosshairImages)
                    {
                        image.color = Color.white;
                    }
                }
                return;
            }
            
            Physics.Raycast(cam.position, cam.forward, out var raycastHit, maxGrappleDistance, whatIsGrappleable);

            // draw direction of raycast
            Debug.DrawRay(cam.position, cam.forward * maxGrappleDistance, Color.yellow);

            // direct hit
            if (raycastHit.point != Vector3.zero)
            {
                foreach (var image in _crosshairImages)
                {
                    image.color = Color.white;
                }

                CanGrapple = true;
                predicitionPoint.position = raycastHit.point;
            }
            else
            {
                CanGrapple = false;
                foreach (var image in _crosshairImages)
                {
                    image.color = Color.red;
                }
            }

            predictionHit = raycastHit;
        }
        
        
        
        
    }
}