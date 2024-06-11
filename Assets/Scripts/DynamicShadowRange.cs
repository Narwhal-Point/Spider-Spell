using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class DynamicShadowRange : MonoBehaviour
{
    private DecalProjector _projector;
    [SerializeField] private float maxProjectionRange = 5f;
    [SerializeField] private LayerMask collisionLayer;
    
    private void Start()
    {
        _projector = GetComponent<DecalProjector>();
    }
    
    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, maxProjectionRange, collisionLayer))
        {
            Vector3 size = _projector.size;
            Vector3 pivot = _projector.pivot;
            size.z = Vector3.Distance(transform.position, hit.point) + 0.2f;

            // Normalize the size.z value to the range [0, 1]
            float normalizedSize = Mathf.Clamp01(size.z / maxProjectionRange);

            // Interpolate pivot.z from 0 to 2.5 based on the normalized size
            pivot.z = Mathf.Lerp(0f, maxProjectionRange / 2, normalizedSize);

            // set the size and pivot
            _projector.size = size;
            _projector.pivot = pivot;

            #if UNITY_EDITOR
            Debug.DrawLine(transform.position, hit.point, Color.blue);
            #endif
        }
    }
}
