using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private float interactRange = 2f;
        private void Update()
        {
            if (InputManager.instance.InteractInput)
            {
                IInteractable interactable = GetInteractableObject();
                if(interactable != null)
                    interactable.Interact();
            }
        }

        public IInteractable GetInteractableObject()
        {
            List<IInteractable> interactableList = new List<IInteractable>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))
                {
                    interactableList.Add(interactable);
                }
            }

            IInteractable closestInteractable = null;
            foreach (var interactable in interactableList)
            {
                if (closestInteractable == null)
                {
                    closestInteractable = interactable;
                }
                else if(Vector3.Distance(transform.position, interactable.GetTransform().position) < Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
                {
                    closestInteractable = interactable;
                }
            }
            
            return closestInteractable;
        }
        
        
    }
}
