using UnityEngine;

namespace Interaction
{
    public class GetObjectInteraction : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Destroy(gameObject);
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}
