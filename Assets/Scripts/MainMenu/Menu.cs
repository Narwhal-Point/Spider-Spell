using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    [Header("First Selected Button")] 
    [SerializeField] private Button firstSelected;

    private bool isFirstSelectedSet = false;

    protected virtual void OnEnable()
    {
        // Check immediately if a gamepad is connected
        isFirstSelectedSet = false;
        CheckForGamepad();
    }

    void Update()
    {
        // Periodically check for gamepad presence until the first selected button is set
            CheckForGamepad();
    }

    private void CheckForGamepad()
    {
        if (Gamepad.current != null && !isFirstSelectedSet)
        {
            SetFirstSelected(firstSelected);
            isFirstSelectedSet = true; // Mark as set to avoid redundant checks
        }
    }

    public void SetFirstSelected(Button firstSelectedButton)
    {
        // Set the first selected button
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }
}
