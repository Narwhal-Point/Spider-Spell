using Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Wind : MonoBehaviour
{

    [SerializeField] Vector3 windDirection;
    [SerializeField] float windStrength;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("blowing");
            PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
            PlayerSwingHandler swingHandler = other.gameObject.GetComponent<PlayerSwingHandler>();
            Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
            PlayerInput playerInput = other.gameObject.GetComponent<PlayerInput>();
            if (otherRb != null)
            {
                // playerInput.enabled = false;
                InputManager.instance.DisableAllInputsButMenu();
                swingHandler.lr.positionCount = 0;
                swingHandler.DestroyJoint();
                otherRb.velocity = Vector3.zero;
                otherRb.drag = 0f;
                movement.enabled = false;
                otherRb.AddForce(windDirection * windStrength, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerSwingHandler swingHandler = other.gameObject.GetComponent<PlayerSwingHandler>();
            Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                swingHandler.lr.positionCount = 0;
                swingHandler.DestroyJoint();
                otherRb.AddForce(windDirection * windStrength, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
        Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
        PlayerInput playerInput = other.gameObject.GetComponent<PlayerInput>();
        if (otherRb != null)
        {
            // playerInput.enabled = true;
            InputManager.instance.EnableAllInputs();
            movement.enabled = true;
            otherRb.AddForce(windDirection * windStrength, ForceMode.Impulse);
        }
    }

}
