using UnityEngine;

namespace Interaction
{
    public interface IInteractable
    {
        public void Interact();

        public Transform GetTransform();
    }
}