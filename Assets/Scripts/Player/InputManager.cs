// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class InputManager : MonoBehaviour
// {
//     public static InputManager instance;
//     
//     public Vector2 MoveInput { get; private set; }
//     
//     public bool JumpJustPressed { get; private set; }
//     public bool JumpBeingHeld { get; private set; }
//     public bool JumpReleased { get; private set; }
//     
//     public bool AimJustPressed { get; private set; }
//     public bool AimBeingHeld { get; private set; }
//     public bool AimReleased { get; private set; }
//     
//     public bool FireJustPressed { get; private set; }
//     public bool FireBeingHeld { get; private set; }
//     public bool FireReleased { get; private set; }
//     public bool MenuOpenCloseInput { get; private set; }
//     
//     private PlayerInput _playerInput;
//     
//     private InputAction _moveAction;
//     private InputAction _jumpAction;
//     private InputAction _fireAction;
//     private InputAction _aimAction;
//     private InputAction _menuOpenCloseAction;
//
//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//         }
//
//         _playerInput = GetComponent<PlayerInput>();
//         SetupInputActions();
//     }
//
//     // Update is called once per frame
//     private void Update()
//     {
//         UpdateInputs();
//     }
//
//     private void SetupInputActions()
//     {
//         // _moveAction = _playerInput.actions["Move"];
//         // _jumpAction = _playerInput.actions["Jump"];
//         // _fireAction = _playerInput.actions["Fire"];
//         // _aimAction = _playerInput.actions["Aim"];
//         _menuOpenCloseAction = _playerInput.actions["MenuOpenClose"];
//     }
//
//     private void UpdateInputs()
//     {
//         // MoveInput = _moveAction.ReadValue<Vector2>();
//         // JumpJustPressed = _jumpAction.WasPressedThisFrame();
//         // JumpBeingHeld = _jumpAction.IsPressed();
//         // JumpReleased = _jumpAction.WasReleasedThisFrame();
//         // AimJustPressed = _aimAction.WasPressedThisFrame();
//         // AimBeingHeld = _aimAction.IsPressed();
//         // AimReleased = _aimAction.WasReleasedThisFrame();
//         // FireJustPressed = _fireAction.WasPressedThisFrame();
//         // FireBeingHeld = _fireAction.IsPressed();
//         // FireReleased = _fireAction.WasReleasedThisFrame();
//         
//         MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
//     }
// }
