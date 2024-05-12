using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    
    public Vector2 MoveInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool InteractInput { get; private set; }

    [SerializeField] private PlayerInput playerInput;

    private InputAction _moveAction;
    private InputAction _fireAction;
    private InputAction _interactAction;
    // private InputAction fallAction;
    // private InputAction menuOpenAction;
    // private InputAction rotateAction;


    public bool MenuOpenCloseInput { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        SetupActionInput();
    }

    private void Update()
    {
        // menuOpenCloseInput = menuOpenAction.WasPressedThisFrame();
        UpdateInputs();
    }
    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        FireInput = _fireAction.WasPressedThisFrame();
        InteractInput = _interactAction.WasPressedThisFrame();
        // FallPressed = fallAction.WasPressedThisFrame();
        // MenuPressed = menuOpenAction.WasPressedThisFrame();
        // RotatePressed = rotateAction.WasPressedThisFrame();
    }

    private void SetupActionInput()
    {
        // fallAction = playerInput.actions["Fall"];
        // menuOpenAction = playerInput.actions["MenuOpenClose"];
        _moveAction = playerInput.actions["Move"];
        _fireAction = playerInput.actions["Fire"];
        _interactAction = playerInput.actions["Interact"];
        // rotateAction = playerInput.actions["Rotate"];
    }

}
