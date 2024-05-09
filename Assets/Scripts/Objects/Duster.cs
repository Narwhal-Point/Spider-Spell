using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duster : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float maxRotationAngle = 4f;
    [SerializeField] float moveSpeed = 15f;
    private bool movingToStartPoint = false;
    private bool rotateRight = false;
    private float step;
    private float rotateValue;

    private Vector3 startEulerAngles;
    [SerializeField] float rotationSpeed =25f;


    // Start is called before the first frame update
    void Start()
    {
        startEulerAngles = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        rotateValue = rotationSpeed * Time.deltaTime;
        if (rotateRight)
        {
            transform.Rotate(0, 0, rotateValue);
            if (transform.eulerAngles.z - startEulerAngles.z > maxRotationAngle)
            {
                rotateRight = false;
            }
        }
        else 
        {
            transform.Rotate(0, 0, -rotateValue);
            if (startEulerAngles.z - transform.eulerAngles.z > maxRotationAngle)
            {
                rotateRight = true;
            }
        }

        step = moveSpeed * Time.deltaTime;
        if (movingToStartPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, step);
            if (Vector3.Distance(transform.position, startPoint.position) < 0.01f)
            {
                movingToStartPoint = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
            if (Vector3.Distance(transform.position, endPoint.position) < 0.01f)
            {
                movingToStartPoint = true;
            }
        }
    }
}
