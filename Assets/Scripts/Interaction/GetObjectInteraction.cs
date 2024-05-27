using UnityEngine;

namespace Interaction
{
    public class GetObjectInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField] private string text;
        public void Interact()
        {
            Destroy(gameObject);
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public string GetInteractionText()
        {
            return text;
        }
    }
}
