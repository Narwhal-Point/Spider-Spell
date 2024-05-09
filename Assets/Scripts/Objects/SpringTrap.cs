using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpringTrap : MonoBehaviour
{
    private Vector3 startEulerAngles;
    [SerializeField] float maxRotationAngle = 45f;
    [SerializeField] float rotationSpeedUp = 100f;
    [SerializeField] float rotationSpeedDown = 1f;
    private float rotateValue;
    [SerializeField] float waitDurationTop = 1f;
    [SerializeField] float waitDurationBottom = 4f;
    private float waitTimer = 0f;
    private bool movingUp = true;
    private bool waitingTop = false;
    private bool waitingBottom = true;
    [SerializeField] AudioSource boingSFX;
    [SerializeField] Rigidbody testBody;

    // Start is called before the first frame update
    void Start()
    {
        startEulerAngles = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingBottom)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDurationBottom)
            {
                waitingBottom = false;
                waitTimer = 0f;
                boingSFX.Play();
            }
        }
        else if (waitingTop)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDurationTop)
            {
                waitingTop = false;
                waitTimer = 0f;
            }
        }
        else
        {
            if (movingUp)
            {
                rotateValue = rotationSpeedUp * Time.deltaTime;
                transform.Rotate(0, 0, rotateValue);

                if (transform.eulerAngles.z - startEulerAngles.z > maxRotationAngle)
                {
                    movingUp = false;
                    waitingTop = true;
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(startEulerAngles);
                float step = rotationSpeedDown * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f) // Check if rotation is almost complete
                {
                    movingUp = true;
                    waitingBottom = true;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (movingUp)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.gameObject.CompareTag("Player")) // Replace "YourTag" with the tag of the object you want to collide with
                {
                    Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                    if (otherRigidbody != null)
                    {
                        // Add force to the collided object
                        testBody.AddForce(contact.normal * 100f, ForceMode.Force);
                    }
                }
            }
        } 
    }
}
