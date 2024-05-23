using System;
using System.Collections;
using Player.Movement;
using UnityEngine;

public class SpringTrap : MonoBehaviour
{
    private Vector3 _startEulerAngles;
    
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] private float rotationSpeedUp = 100f;
    [SerializeField] private float rotationSpeedDown = 1f;
    
    private float _rotateValue;
    
    [SerializeField] private float waitDurationTop = 1f;
    [SerializeField] private float waitDurationBottom = 4f;
    [SerializeField] private float springStrength = 4f;
    
    private float _waitTimer = 0f;
    private bool _movingUp = false;
    private bool _waitingTop = false;
    private bool _waitingBottom = true;
    private bool _shot = false;
    
    [SerializeField] private AudioSource boingSFX;
    [SerializeField] private Collider meshCollider;
    [SerializeField] private Transform normalObject;
    [SerializeField] private Transform movingObject;
    
    void Start()
    {
        _startEulerAngles = transform.localEulerAngles;
    }
    
    void Update()
    {
        if (_waitingBottom)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitDurationBottom)
            {
                _waitingBottom = false;
                _movingUp = true;
                _shot = false;
                _waitTimer = 0f;
                boingSFX.Play();
            }
        }
        else if (_waitingTop)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitDurationTop)
            {
                _waitingTop = false;
                _waitTimer = 0f;
            }
        }
        else if (_movingUp)
        {
            _rotateValue = rotationSpeedUp * Time.deltaTime;
            movingObject.Rotate(0, 0, _rotateValue, Space.Self);

            if (movingObject.eulerAngles.z - _startEulerAngles.z > maxRotationAngle)
            {
                _shot = true;
                _movingUp = false;
                _waitingTop = true;
            }
        }
        else // if moving down
        {
            Quaternion targetRotation = Quaternion.Euler(_startEulerAngles);
            float step = rotationSpeedDown * Time.deltaTime;
            movingObject.localRotation = Quaternion.RotateTowards(movingObject.localRotation, targetRotation, step);
            if (Quaternion.Angle(movingObject.localRotation, targetRotation) < 0.1f)
            {
                _waitingBottom = true;
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (_movingUp && !_shot)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerMovement movement = collision.gameObject.GetComponent<PlayerMovement>();
                Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
                if (otherRb != null)
                {
                    // don't ask why the movement script needs to be disabled for .2 seconds. If I don't do this the script won't work.
                    movement.enabled = false;
                    otherRb.drag = 0f;
                    otherRb.velocity = new Vector3(0,0,0);
                    otherRb.AddForce(normalObject.up * springStrength, ForceMode.Impulse);
                    _shot = true;
                    
                    StartCoroutine(delayEnable(movement));
                }
            }
        }
    }
    
    IEnumerator delayEnable(PlayerMovement movement)
    {
        yield return new WaitForSeconds(0.2f);
        movement.enabled = true;

    }
    
    
}