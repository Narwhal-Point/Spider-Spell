using UnityEngine;

public class WitchMovement : MonoBehaviour
{
    public bool moving = false;
    
    [Header("Movement")]
    public float moveSpeed = 8;

    public float groundDrag = 5;
    
    [Header("Ground Check")] 
    public float characterHeight = 2;
    public LayerMask ground;
    private bool _grounded;
    
    public Transform orientation;
    
    private Vector3 _moveDirection;

    private Rigidbody _rb;

    private WitchFov _fov;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // stop character from falling over

        _fov = GetComponent<WitchFov>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if npc is on the ground
        _grounded = Physics.Raycast(transform.position, Vector3.down, 
            characterHeight * 0.5f + 0.2f, ground);

        // set ground drag based on if the npc is grounded or not
        _rb.drag = _grounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        if(moving)
            MoveCharacter();
    }

    private void MoveCharacter()
    {
        _moveDirection = orientation.forward;
        _rb.AddForce(_moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
    }
}
