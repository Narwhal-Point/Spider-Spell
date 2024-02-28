using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSwinging : MonoBehaviour
{
    [Header("References")] 
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    
    [Header("Swinging")]
    public float maxSwingDistance = 15f;
    public Vector3 swingPoint { get; set; }
    public SpringJoint joint { get; set; }
    public Vector3 currentGrapplePosition { get; set; }
    
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("Prediction")] 
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predicitionPoint;
    
    private void Start()
    {
        
    }

    private void Update()
    {
        // if (Input.GetKeyDown(swingKey))
        // {
        //     StartSwing();
        // }
        //
        // if(Input.GetKeyUp(swingKey))
        //     StopSwing();
        
        CheckForSwingPoints();
    }

    private void LateUpdate()
    {
        drawRope();
    }
    void drawRope()
    {
        // if not grappling don't draw rope
        if(!joint)
            return;
        
        // make it so the line doesn't appear instantly
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    private void CheckForSwingPoints()
    {
        if(joint != null)
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