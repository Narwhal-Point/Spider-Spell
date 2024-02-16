using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")] 
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody _rb;
    private PlayerMovement _pm;

    [Header("_sliding")] 
    public float maxSlideTime;
    public float slideForce;
    private float _slideTimer;

    public float slideYScale;
    private float _startYScale;

    [Header("Input")] 
    public KeyCode slideKey = KeyCode.LeftControl;
    private float _horizontalInput;
    private float _verticalInput;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<PlayerMovement>();

        _startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        // get keyboard input
        _horizontalInput = Input.GetAxisRaw("Horizontal"); // A + D
        _verticalInput = Input.GetAxisRaw("Vertical"); // W + S
        
        if(Input.GetKeyDown(slideKey) && (_horizontalInput != 0 || _verticalInput != 0))
            StartSlide();
        if(Input.GetKeyUp(slideKey) && _pm.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if(_pm.sliding)
            _slidingMovement();
    }

    private void StartSlide()
    {
        _pm.sliding = true;

        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = maxSlideTime;

    }

    private void _slidingMovement()
    {
        // sliding normally
        Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (!_pm.OnSlope() || _rb.velocity.y > -0.1f)
        {
            _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            _slideTimer -= Time.deltaTime;
        }
        else // sliding down a slope
        {
            _rb.AddForce(_pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if(_slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        _pm.sliding = false;
        transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
    }
}
