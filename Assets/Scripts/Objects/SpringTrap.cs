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
    [SerializeField] float springStrength = 4f;
    private float waitTimer = 0f;
    private bool movingUp = false;
    private bool waitingTop = false;
    private bool waitingBottom = true;
    private bool shot = false;
    [SerializeField] AudioSource boingSFX;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] Transform normalObject;

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
                movingUp = true;
                shot = false;
                waitTimer = 0f;
                boingSFX.Play();
                int ignoreGround = LayerMask.NameToLayer("Default");
                gameObject.layer = ignoreGround;
                
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
                transform.Rotate(0, 0, rotateValue, Space.Self);

                if (transform.eulerAngles.z - startEulerAngles.z > maxRotationAngle)
                {
                    shot = true;
                    movingUp = false;
                    waitingTop = true;
                    meshCollider.enabled = true;
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(startEulerAngles);
                float step = rotationSpeedDown * Time.deltaTime;
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, step);
                if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
                {
                    waitingBottom = true;
                    int ground = LayerMask.NameToLayer("Ground");
                    gameObject.layer = ground;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (movingUp && !shot)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                    if (otherRigidbody != null)
                    {
                        Debug.Log("Shit");
                        meshCollider.enabled = false;
                        otherRigidbody.AddForce(normalObject.transform.up * springStrength, ForceMode.Impulse);
                        shot = true;
                    }
                }
            }
        } 
    }
}
