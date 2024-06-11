using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FanSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1000f;
    Vector3 startRotation;
    private float z;

    void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
        z = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        z += rotationSpeed * Time.deltaTime;
        if (z > 360f)
        {
            z = 0.0f;
        }
        transform.localRotation = Quaternion.Euler(startRotation.x, startRotation.y, z);
    }
}
