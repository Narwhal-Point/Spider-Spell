using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] private PlayerInput playerInput;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool AimInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool MenuInput { get; private set; }
    public bool QuestLogInput { get; private set; }
    public bool RecenterInput { get; private set; }

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _fireAction;
    private InputAction _jumpAction;
    private InputAction _aimAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;
    private InputAction _menuAction;
    private InputAction _logAction;
    private InputAction _recenterAction;
    
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
        UpdateInputs();
    }
    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        LookInput = _lookAction.ReadValue<Vector2>();
        FireInput = _fireAction.WasPressedThisFrame();
        JumpInput = _jumpAction.WasPressedThisFrame();
        AimInput = _aimAction.WasPressedThisFrame();
        SprintInput = _sprintAction.WasPressedThisFrame();
        InteractInput = _interactAction.WasPressedThisFrame();
        MenuInput = _menuAction.WasPressedThisFrame();
        QuestLogInput = _logAction.WasPressedThisFrame();
        RecenterInput = _recenterAction.WasPressedThisFrame();
    }

    private void SetupActionInput()
    {
        _moveAction = playerInput.actions["Move"];
        _lookAction = playerInput.actions["Look"];
        _fireAction = playerInput.actions["Fire"];
        _jumpAction = playerInput.actions["Jump"];
        _aimAction = playerInput.actions["Aim"];
        _sprintAction = playerInput.actions["Sprint"];
        _interactAction = playerInput.actions["Interact"];
        _menuAction = playerInput.actions["MenuOpenClose"];
        _logAction = playerInput.actions["QuestLog"];
        _recenterAction = playerInput.actions["Recenter"];
    }

    public void EnableMovement()
    {
        _moveAction.Enable();
    }
    public void DisableMovement()
    {
        _moveAction.Disable();
    }

    public void EnableLook()
    {
        _lookAction.Enable();
    }
    
    public void DisableLook()
    {
        _lookAction.Disable();
    }
    
    public void EnableFire()
    {
        _lookAction.Enable();
    }
    
    public void DisableFire()
    {
        _lookAction.Disable();
    }

    public void EnableQuestLog()
    {
        _logAction.Enable();
    }
    
    public void DisableQuestLog()
    {
        _logAction.Disable();
    }

    public void EnableInteract()
    {
        _interactAction.Enable();
    }

    public void DisableInteract()
    {
        _interactAction.Disable();
    }
    
    // disable all inputs in the action map "Player".
    // this is better than just disabling playerinput, because this way that can still be
    // used for checking what input device is used as an example
    public void DisableAllInputs()
    {
        string actionMapName = "Player"; // replace with your action map name
        var actionMap = playerInput.actions.FindActionMap(actionMapName);
        if (actionMap != null)
        {
            foreach (var action in actionMap)
            {
                action.Disable();
            }
        }
    }

    // Enable all inputs in the action map "Player".
    // this is better than just disabling playerinput, because this way that can still be
    // used for checking what input device is used as an example
    public void EnableAllInputs()
    {
        string actionMapName = "Player"; // replace with your action map name
        var actionMap = playerInput.actions.FindActionMap(actionMapName);
        if (actionMap != null)
        {
            foreach (var action in actionMap)
            {
                action.Enable();
            }
        }
    }
    
    // Disable all inputs in the action map "Player" but enable menu. So the player can still open the menu.
    // this is better than just disabling playerinput, because this way that can still be
    // used for checking what input device is used as an example
    public void DisableAllInputsButMenu()
    {
        string actionMapName = "Player"; // replace with your action map name
        var actionMap = playerInput.actions.FindActionMap(actionMapName);
        if (actionMap != null)
        {
            foreach (var action in actionMap)
            {
                action.Disable();
            }
        }
        _menuAction.Enable();
        
    }

}
