using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FanStuck : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float speedMultiplierBack = 0.6f;
    [SerializeField] private float maxRotation = 15f;
    Vector3 startRotation;
    private bool spinClockwise = true;
    private float z;

    void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
        z = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (spinClockwise)
        {
            z += rotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(startRotation.x, startRotation.y, z);
            if (z > maxRotation)
            {
                spinClockwise = false;
            }
        }
        else 
        {
            z -= rotationSpeed * speedMultiplierBack * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(startRotation.x, startRotation.y, z);
            if (z < 0.0f )
            {
                spinClockwise = true;
            }
        }
        
    }
}
